using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace HRManagement.Infrastructure.Data;
public class HRManagementDbContextFactory : IDesignTimeDbContextFactory<HRManagementDbContext>
{
    public HRManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HRManagementDbContext>();
        
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        string connectionString;
        
        if (!string.IsNullOrEmpty(databaseUrl))
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            connectionString = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={databaseUri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
        }
        else
        {
            connectionString = "Host=localhost;Port=5432;Database=HRManagementDb;Username=postgres;Password=postgres";
        }
        
        optionsBuilder.UseNpgsql(connectionString);
        return new HRManagementDbContext(optionsBuilder.Options);
    }
}
