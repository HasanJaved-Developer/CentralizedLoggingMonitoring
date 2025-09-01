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

            var user = await _db.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == req.UserName);

            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
            {
                return Unauthorized("Invalid credentials.");
            }

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
            // Fetch the user (for name in DTO)
            var user = await _db.Users.AsNoTracking().FirstAsync(u => u.Id == userId);

            // 1) Get all (Category, Module, Function) triples the user is allowed to access.
            //    This is fully translatable SQL: joins + where.
            var triples = await (
                from ur in _db.UserRoles.AsNoTracking()
                where ur.UserId == userId
                join rf in _db.RoleFunctions.AsNoTracking() on ur.RoleId equals rf.RoleId
                join f in _db.Functions
                            .Include(x => x.Module)
                                .ThenInclude(m => m.Category)
                            .AsNoTracking()
                      on rf.FunctionId equals f.Id
                select new
                {
                    CategoryId = f.Module.Category.Id,
                    CategoryName = f.Module.Category.Name,
                    ModuleId = f.Module.Id,
                    ModuleName = f.Module.Name,
                    f.Module.Area,
                    f.Module.Controller,
                    f.Module.Action,
                    FunctionId = f.Id,
                    f.Code,
                    f.DisplayName
                }
            ).ToListAsync();

            // 2) Group in memory to shape the hierarchical DTOs.
            var categoryDtos = triples
                .GroupBy(t => new { t.CategoryId, t.CategoryName })
                .Select(cg => new CategoryDto(
                    cg.Key.CategoryId,
                    cg.Key.CategoryName,
                    cg.GroupBy(t => new { t.ModuleId, t.ModuleName, t.Area, t.Controller, t.Action })
                      .Select(mg => new ModuleDto(
                          mg.Key.ModuleId,
                          mg.Key.ModuleName,
                          mg.Key.Area,
                          mg.Key.Controller,
                          mg.Key.Action,
                          mg.GroupBy(x => new { x.FunctionId, x.Code, x.DisplayName }) // distinct functions
                            .Select(g => new FunctionDto(g.Key.FunctionId, g.Key.Code, g.Key.DisplayName))
                            .OrderBy(f => f.Code)
                            .ToList()
                      ))
                      .OrderBy(m => m.Name)
                      .ToList()
                ))
                .OrderBy(c => c.Name)
                .ToList();

            return new UserPermissionsDto(user.Id, user.UserName, categoryDtos);
        }

        private string GenerateJwt(AppUser user, IEnumerable<string> roles, IEnumerable<string> functionCodes, out DateTime expiresAtUtc)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.KeyBase64));
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
