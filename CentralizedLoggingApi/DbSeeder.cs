using CentralizedLoggingApi.Data;
using CentralizedLoggingApi.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CentralizedLoggingApi
{
    public static class DbSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LoggingDbContext>();
            
            // Seed Applications
            if (!context.Applications.Any())
            {
                context.Applications.AddRange(
                    new Application { Name = "Library Service", Environment = "Production", ApiKey = Guid.NewGuid().ToString() },
                    new Application { Name = "User Management", Environment = "Staging", ApiKey = Guid.NewGuid().ToString() },
                    new Application { Name = "Reporting API", Environment = "Development", ApiKey = Guid.NewGuid().ToString() },
                    new Application { Name = "Integration Portal", Environment = "Development", ApiKey = Guid.NewGuid().ToString() }
                );
                context.SaveChanges();
            }

            // Seed ErrorLogs
            if (!context.ErrorLogs.Any())
            {
                var app1 = context.Applications.First(a => a.Name == "Library Service");
                var app2 = context.Applications.First(a => a.Name == "User Management");

                context.ErrorLogs.AddRange(
                    new ErrorLog
                    {
                        ApplicationId = app1.Id,
                        Severity = "Error",
                        Message = "Null reference exception in payment processing",
                        StackTrace = "at LibraryService.Process()...",
                        Source = "LibrarytService",
                        UserId = "user123",
                        RequestId = Guid.NewGuid().ToString(),
                        LoggedAt = DateTime.UtcNow
                    },
                    new ErrorLog
                    {
                        ApplicationId = app2.Id,
                        Severity = "Warning",
                        Message = "Login attempt failed due to invalid credentials",
                        Source = "AuthController",
                        UserId = "user456",
                        RequestId = Guid.NewGuid().ToString(),
                        LoggedAt = DateTime.UtcNow
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
