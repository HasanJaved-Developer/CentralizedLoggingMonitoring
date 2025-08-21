using CentralizedLoggingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CentralizedLoggingApi.Data
{
    public class LoggingDbContext : DbContext
    {
        public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options) { }

        public DbSet<Application> Applications { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Application
            modelBuilder.Entity<Application>()
                .HasIndex(a => a.ApiKey)
                .IsUnique();

            modelBuilder.Entity<Application>()
                .Property(a => a.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Application>()
                .Property(a => a.Environment)
                .HasMaxLength(50)
                .IsRequired();

            // ErrorLog
            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.Severity)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<ErrorLog>()
                .HasOne(e => e.Application)
                .WithMany(a => a.ErrorLogs)
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
