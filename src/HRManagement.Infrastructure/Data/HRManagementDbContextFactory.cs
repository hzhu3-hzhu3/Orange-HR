using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace HRManagement.Infrastructure.Data;
public class HRManagementDbContextFactory : IDesignTimeDbContextFactory<HRManagementDbContext>
{
    public HRManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HRManagementDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=HRManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true");
        return new HRManagementDbContext(optionsBuilder.Options);
    }
}
