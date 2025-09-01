using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementApi.Data;
using UserManagementApi.DTO;

namespace UserManagementApi.Controllers
{

    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) => _db = db;

        // GET: api/users/{userId}/permissions
        [HttpGet("{userId:int}/permissions")]
        public async Task<ActionResult<UserPermissionsDto>> GetPermissions(int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound($"User {userId} not found.");

            // Build a single LINQ query that filters functions to those reachable
            // via the user's roles (no client-side filtering).
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
                      .Where(md => md.Functions.Any()) // keep only modules with at least one permitted function
                      .ToList()
                ))
                .Where(cd => cd.Modules.Any()) // keep only categories with at least one permitted module
                .ToListAsync();

            var dto = new UserPermissionsDto(user.Id, user.UserName, categories);
            return Ok(dto);
        }
    }
