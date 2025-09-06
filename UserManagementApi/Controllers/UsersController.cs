using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.Contracts.Auth;
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
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] DTO.LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.UserName) || string.IsNullOrWhiteSpace(req.Password))
            {
                return ValidationProblem(new ValidationProblemDetails
                {
                    Title = "Validation error",
                    Detail = "Username and password are required.",
                    Status = StatusCodes.Status400BadRequest,
                    Errors = new Dictionary<string, string[]>
                    {
                        ["userName"] = new[] { "Required" },
                        ["password"] = new[] { "Required" }
                    }
                });
            }

            var user = await _db.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == req.UserName);

            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
            {                
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid credentials",
                    Detail = "Username or password is incorrect.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }
        
   
            var dto = await BuildPermissionsForUser(user.Id);
                        
            var token = GenerateJwt(user, dto.Categories, out var expiresAtUtc);

            // Get the same permissions tree you already expose
            var permissions = await BuildPermissionsForUser(user.Id);

            return Ok(new AuthResponse(user.Id, user.UserName, token, expiresAtUtc));
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
                
        private string GenerateJwt(AppUser user, List<CategoryDto> Categories, out DateTime expiresAtUtc)
        {
            var keyBase64 = _jwt.Key;
            var keyPlain = Encoding.UTF8.GetString(Convert.FromBase64String(keyBase64));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyPlain));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };
            
            
            // optional: add claims (careful: keep token size reasonable)
            var categoriesJson = System.Text.Json.JsonSerializer.Serialize(Categories);
            claims.Add(new Claim("categories", categoriesJson));
            

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
