using Microsoft.EntityFrameworkCore;
using UserManagementApi.Data;
using UserManagementApi.Models;

namespace UserManagementApi
{
    public static class DbSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Apply any pending migrations
            context.Database.Migrate();

            // Only seed if DB is empty (idempotent)
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Id = 1, Name = "Administration" },
                    new Category { Id = 2, Name = "Operations" }
                };

                var modules = new List<Module>
                {
                    new Module { Id = 1, Name = "User Management", Area = "Admin", Controller = "Users", Action = "Index", CategoryId = 1 },
                    new Module { Id = 2, Name = "Role Management", Area = "Admin", Controller = "Roles", Action = "Index", CategoryId = 1 },
                    new Module { Id = 3, Name = "Payments", Area = "Ops", Controller = "Payments", Action = "Index", CategoryId = 2 }
                };

                var functions = new List<Function>
                {
                    new Function { Id = 1, ModuleId = 1, Code = "Users.View", DisplayName = "View Users" },
                    new Function { Id = 2, ModuleId = 1, Code = "Users.Edit", DisplayName = "Edit Users" },
                    new Function { Id = 3, ModuleId = 2, Code = "Roles.View", DisplayName = "View Roles" },
                    new Function { Id = 4, ModuleId = 2, Code = "Roles.Assign", DisplayName = "Assign Roles" },
                    new Function { Id = 5, ModuleId = 3, Code = "Payments.View", DisplayName = "View Payments" }
                };

                var roles = new List<Role>
                {
                    new Role { Id = 1, Name = "Admin" },
                    new Role { Id = 2, Name = "Operator" }
                };

                var users = new List<AppUser>
                {
                    new AppUser { Id = 1, UserName = "alice", Password = "alice" },
                    new AppUser { Id = 2, UserName = "bob",   Password = "bob" }
                };

                var userRoles = new List<UserRole>
                {
                    new UserRole { UserId = 1, RoleId = 1 }, // alice → Admin
                    new UserRole { UserId = 2, RoleId = 2 }  // bob → Operator
                };

                var roleFunctions = new List<RoleFunction>
                {
                    // Admin gets everything
                    new RoleFunction { RoleId = 1, FunctionId = 1 },
                    new RoleFunction { RoleId = 1, FunctionId = 2 },
                    new RoleFunction { RoleId = 1, FunctionId = 3 },
                    new RoleFunction { RoleId = 1, FunctionId = 4 },
                    new RoleFunction { RoleId = 1, FunctionId = 5 },

                    // Operator gets limited
                    new RoleFunction { RoleId = 2, FunctionId = 1 }, // Users.View
                    new RoleFunction { RoleId = 2, FunctionId = 5 }  // Payments.View
                };

                // Add and save
                context.Categories.AddRange(categories);
                context.Modules.AddRange(modules);
                context.Functions.AddRange(functions);
                context.Roles.AddRange(roles);
                context.Users.AddRange(users);
                context.UserRoles.AddRange(userRoles);
                context.RoleFunctions.AddRange(roleFunctions);

                context.SaveChanges();
            }
        }
    }
}
