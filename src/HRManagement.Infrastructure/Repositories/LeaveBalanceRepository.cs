using HRManagement.Domain.Entities;
using HRManagement.Domain.Interfaces;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace HRManagement.Infrastructure.Repositories;
public class LeaveBalanceRepository : ILeaveBalanceRepository
{
    private readonly HRManagementDbContext _context;
    public LeaveBalanceRepository(HRManagementDbContext context)
    {
        _context = context;
    }
    public async Task<LeaveBalance?> GetByEmployeeAndYearAsync(Guid employeeId, int year)
    {
        return await _context.LeaveBalances
            .FirstOrDefaultAsync(lb => lb.EmployeeId == employeeId && lb.Year == year);
    }
    public async Task<LeaveBalance> CreateAsync(LeaveBalance balance)
    {
        _context.LeaveBalances.Add(balance);
        await _context.SaveChangesAsync();
        return balance;
    }
    public async Task UpdateAsync(LeaveBalance balance)
    {
        _context.LeaveBalances.Update(balance);
        await _context.SaveChangesAsync();
    }
    public async Task<LeaveBalance> GetOrCreateForCurrentYearAsync(Guid employeeId, int annualQuota)
    {
        var currentYear = DateTime.UtcNow.Year;
        var balance = await GetByEmployeeAndYearAsync(employeeId, currentYear);
        if (balance == null)
        {
            balance = new LeaveBalance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                Year = currentYear,
                TotalDays = annualQuota,
                UsedDays = 0,
                PendingDays = 0,
                CreatedAt = DateTime.UtcNow
            };
            await CreateAsync(balance);
        }
        return balance;
    }
}
