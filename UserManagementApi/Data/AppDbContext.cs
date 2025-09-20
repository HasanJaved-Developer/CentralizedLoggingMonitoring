using Microsoft.EntityFrameworkCore;
using UserManagementApi.Contracts.Models;


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
        }
    }
}
