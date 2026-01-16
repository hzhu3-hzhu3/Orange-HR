using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace HRManagement.Infrastructure.Data;
public class HRManagementDbContextFactory : IDesignTimeDbContextFactory<HRManagementDbContext>
{
    public HRManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HRManagementDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=HRManagementDb;Username=postgres;Password=postgres");
        return new HRManagementDbContext(optionsBuilder.Options);
    }
}
