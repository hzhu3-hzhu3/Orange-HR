using HRManagement.Domain.Entities;
using HRManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace HRManagement.Web.Extensions;
public static class DbInitializerExtensions
{
    public static async Task<IApplicationBuilder> InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<DbSeeder>>();
        try
        {
            var context = services.GetRequiredService<HRManagementDbContext>();
            var userManager = services.GetRequiredService<UserManager<Employee>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");
            logger.LogInformation("Seeding database with initial data...");
            var seeder = new DbSeeder(context, userManager, roleManager);
            await seeder.SeedAsync();
            logger.LogInformation("Database seeded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
        return app;
    }
}
