using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagementApi.Data;
using UserManagementApi.DTO;
using UserManagementApi.DTO.Auth;
using UserManagementApi.Models;

namespace UserManagementApi.Controllers
{

    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtOptions _jwt;

        public UsersController(AppDbContext db, IOptions<JwtOptions> jwtOptions)
        {
            _db = db;
            _jwt = jwtOptions.Value;
        }
        // --------- NEW: POST /api/users/authenticate ----------
        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] DTO.LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.UserName) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Username and password are required.");

            // Validate user (plain text as per your seed)
            var user = await _db.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == req.UserName && u.Password == BCrypt.Net.BCrypt.HashPassword(req.Password));

            if (user == null)
                return Unauthorized("Invalid credentials.");

            // Collect roles & function codes for claims (optional but handy)
            var roleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();

            var functionCodes = await _db.RoleFunctions
                .Where(rf => roleIds.Contains(rf.RoleId))
                .Select(rf => rf.Function.Code)
                .Distinct()
                .ToListAsync();

            var token = GenerateJwt(user, user.UserRoles.Select(ur => ur.Role.Name).Distinct().ToList(), functionCodes, out var expiresAtUtc);

            // Get the same permissions tree you already expose
            var permissions = await BuildPermissionsForUser(user.Id);

            return Ok(new AuthResponse(user.Id, user.UserName, token, expiresAtUtc, permissions));
        }

        // --------- (Existing) GET /api/users/{userId}/permissions ----------
        // Now protected by JWT; call with Bearer token returned by /authenticate
        [Authorize]
        [HttpGet("{userId:int}/permissions")]
        public async Task<ActionResult<UserPermissionsDto>> GetPermissions(int userId)
        {
            // Optional: you can enforce that a user can only view their own permissions
            // by comparing userId with the token's sub, if desired.

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound($"User {userId} not found.");

            var dto = await BuildPermissionsForUser(userId);
            return Ok(dto);
        }

        // ----- helpers -----

        private async Task<UserPermissionsDto> BuildPermissionsForUser(int userId)
        {
            var user = await _db.Users.FirstAsync(u => u.Id == userId);

            var categories = await _db.Categories
                .Select(c => new CategoryDto(
                    c.Id,
                    c.Name,
                    c.Modules
                      .Select(m => new ModuleDto(
                          m.Id, m.Name, m.Area, m.Controller, m.Action,
                          m.Functions
                            .Where(f => f.RoleFunctions
                                .Any(rf => rf.Role.UserRoles.Any(ur => ur.UserId == userId)))
                            .Select(f => new FunctionDto(f.Id, f.Code, f.DisplayName))
                            .ToList()
                      ))
                      .Where(md => md.Functions.Any())
                      .ToList()
                ))
                .Where(cd => cd.Modules.Any())
                .ToListAsync();

            return new UserPermissionsDto(user.Id, user.UserName, categories);
        }

        private string GenerateJwt(AppUser user, IEnumerable<string> roles, IEnumerable<string> functionCodes, out DateTime expiresAtUtc)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            // optional: add role claims
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // optional: add function claims (careful: keep token size reasonable)
            foreach (var fn in functionCodes)
                claims.Add(new Claim("perm", fn));

            var now = DateTime.UtcNow;
            expiresAtUtc = now.AddMinutes(_jwt.ExpiresMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAtUtc,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
