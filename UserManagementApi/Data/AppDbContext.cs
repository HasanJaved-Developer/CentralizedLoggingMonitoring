using Microsoft.EntityFrameworkCore;
using UserManagementApi.Models;

namespace UserManagementApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<Function> Functions => Set<Function>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RoleFunction> RoleFunctions => Set<RoleFunction>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Keys for join entities
            b.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
            b.Entity<RoleFunction>().HasKey(x => new { x.RoleId, x.FunctionId });

            // Relationships
            b.Entity<Module>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Function>()
                .HasOne(f => f.Module)
                .WithMany(m => m.Functions)
                .HasForeignKey(f => f.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            b.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            b.Entity<RoleFunction>()
                .HasOne(rf => rf.Role)
                .WithMany(r => r.RoleFunctions)
                .HasForeignKey(rf => rf.RoleId);

            b.Entity<RoleFunction>()
                .HasOne(rf => rf.Function)
                .WithMany(f => f.RoleFunctions)
                .HasForeignKey(rf => rf.FunctionId);

            // ---------- Seed Data ----------
            // Categories
            b.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Administration" },
                new Category { Id = 2, Name = "Operations" }
            );

            // Modules
            b.Entity<Module>().HasData(
                new Module { Id = 1, Name = "User Management", Area = "Admin", Controller = "Users", Action = "Index", CategoryId = 1 },
                new Module { Id = 2, Name = "Role Management", Area = "Admin", Controller = "Roles", Action = "Index", CategoryId = 1 },
                new Module { Id = 3, Name = "Payments", Area = "Ops", Controller = "Payments", Action = "Index", CategoryId = 2 }
            );

            // Functions
            b.Entity<Function>().HasData(
                new Function { Id = 1, ModuleId = 1, Code = "Users.View", DisplayName = "View Users" },
                new Function { Id = 2, ModuleId = 1, Code = "Users.Edit", DisplayName = "Edit Users" },
                new Function { Id = 3, ModuleId = 2, Code = "Roles.View", DisplayName = "View Roles" },
                new Function { Id = 4, ModuleId = 2, Code = "Roles.Assign", DisplayName = "Assign Roles" },
                new Function { Id = 5, ModuleId = 3, Code = "Payments.View", DisplayName = "View Payments" }
            );

            // Roles
            b.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Operator" }
            );

            // Users
            b.Entity<AppUser>().HasData(
                new AppUser { Id = 1, UserName = "alice" },
                new AppUser { Id = 2, UserName = "bob" }
            );

            // User ↔ Role
            b.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1 }, // alice → Admin
                new UserRole { UserId = 2, RoleId = 2 }  // bob → Operator
            );

            // Role ↔ Function
            b.Entity<RoleFunction>().HasData(
                // Admin gets everything
                new RoleFunction { RoleId = 1, FunctionId = 1 },
                new RoleFunction { RoleId = 1, FunctionId = 2 },
                new RoleFunction { RoleId = 1, FunctionId = 3 },
                new RoleFunction { RoleId = 1, FunctionId = 4 },
                new RoleFunction { RoleId = 1, FunctionId = 5 },

                // Operator gets limited
                new RoleFunction { RoleId = 2, FunctionId = 1 }, // Users.View
                new RoleFunction { RoleId = 2, FunctionId = 5 }  // Payments.View
            );
        }
    }
}
