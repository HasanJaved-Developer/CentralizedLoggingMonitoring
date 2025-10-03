using Microsoft.EntityFrameworkCore;
using UserManagementApi.Contracts.Models;
using UserManagementApi.Data;


namespace UserManagementApi
{
    public static class DbSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Only seed once
            if (context.Users.Any() || context.Roles.Any() || context.Categories.Any())
                return;

            // ----- Categories -----
            var adminCat = new Category { Name = "Administration" };
            var opsCat = new Category { Name = "Operations" };
            context.Categories.AddRange(adminCat, opsCat);
            context.SaveChanges(); // get IDs

            // ----- Modules (link via navigation) -----
            var modUsers = new Module { Name = "User Management", Area = "Admin", Controller = "Users", Action = "Index", Category = adminCat };
            var modRoles = new Module { Name = "Role Management", Area = "Admin", Controller = "Roles", Action = "Index", Category = adminCat };
            var modPay = new Module { Name = "Library", Area = "Ops", Controller = "Library", Action = "Index", Category = opsCat };
            var modLog = new Module { Name = "Logs", Area = "Admin", Controller = "Error", Action = "Index", Category = adminCat };
            context.Modules.AddRange(modUsers, modRoles, modPay, modLog);
            context.SaveChanges();

            // ----- Functions (link via navigation) -----
            var fUsersView = new Function { Module = modUsers, Code = "Users.View", DisplayName = "View Users" };
            var fUsersEdit = new Function { Module = modUsers, Code = "Users.Edit", DisplayName = "Edit Users" };
            var fRolesView = new Function { Module = modRoles, Code = "Roles.View", DisplayName = "View Roles" };
            var fRolesAssign = new Function { Module = modRoles, Code = "Roles.Assign", DisplayName = "Assign Roles" };
            var fPayView = new Function { Module = modPay, Code = "Library.View", DisplayName = "View Library" };
            var fLogView = new Function { Module = modLog, Code = "Logs.View", DisplayName = "View Logs" };
            context.Functions.AddRange(fUsersView, fUsersEdit, fRolesView, fRolesAssign, fPayView, fLogView);
            context.SaveChanges();

            // ----- Roles -----
            var adminRole = new Role { Name = "Admin" };
            var operatorRole = new Role { Name = "Operator" };
            context.Roles.AddRange(adminRole, operatorRole);
            context.SaveChanges();

            // ----- Users (hashed passwords) -----
            var alice = new AppUser { UserName = "alice", Password = BCrypt.Net.BCrypt.HashPassword("alice") };
            var bob = new AppUser { UserName = "bob", Password = BCrypt.Net.BCrypt.HashPassword("bob") };
            context.Users.AddRange(alice, bob);
            context.SaveChanges();

            // ----- User ↔ Role (use entity refs, not IDs) -----
            context.UserRoles.AddRange(
                new UserRole { User = alice, Role = adminRole },
                new UserRole { User = bob, Role = operatorRole }
            );
            context.SaveChanges();

            // ----- Role ↔ Function (use refs) -----
            // Admin → all
            context.RoleFunctions.AddRange(
                new RoleFunction { Role = adminRole, Function = fUsersView },
                new RoleFunction { Role = adminRole, Function = fUsersEdit },
                new RoleFunction { Role = adminRole, Function = fRolesView },
                new RoleFunction { Role = adminRole, Function = fRolesAssign },
                new RoleFunction { Role = adminRole, Function = fPayView },
                new RoleFunction { Role = adminRole, Function = fLogView }
            );
            // Operator → limited
            context.RoleFunctions.AddRange(
                new RoleFunction { Role = operatorRole, Function = fUsersView },
                new RoleFunction { Role = operatorRole, Function = fPayView }
            );

            context.SaveChanges();
        }
    }
}
