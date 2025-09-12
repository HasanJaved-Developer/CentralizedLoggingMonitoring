using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CentralizedLoggingApi.Infrastructure
{
    public static class MigrationWithLockExtensions
    {
        /// <summary>
        /// Acquires a SQL app-lock, then runs EF Migrate() and your seed delegate.
        /// Use the SAME lock name across ALL services hitting the same SQL Server.
        /// </summary>
        public static async Task MigrateAndSeedWithSqlLockAsync<TContext>(
            this IHost app,
            string connectionStringName,                       // e.g. "Default"
            string globalLockName,                             // e.g. "IMIS_GLOBAL_MIGRATE_SEED"
            Func<IServiceProvider, CancellationToken, Task> seedAsync,
            CancellationToken ct = default)
            where TContext : DbContext
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var cfg = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var db = services.GetRequiredService<TContext>();

            // Build a **master** connection string on the same server
            var csb = new SqlConnectionStringBuilder(cfg.GetConnectionString(connectionStringName));
            var master = new SqlConnectionStringBuilder(csb.ConnectionString) { InitialCatalog = "master"}.ToString();

            await SqlAppLock.WithExclusiveLockAsync(master, globalLockName, async token =>
            {
                logger.LogInformation("Applying EF migrations for DB '{Db}'...", csb.InitialCatalog);
                await db.Database.MigrateAsync(token);   // creates DB if missing, applies migrations
                logger.LogInformation("Migrations OK for '{Db}'. Seeding...", csb.InitialCatalog);

                await seedAsync(services, token);        // call your existing seeder here

                logger.LogInformation("Seed completed for '{Db}'.", csb.InitialCatalog);
            }, ct: ct);
        }
    }
}
