using HRManagement.Domain.Entities;
namespace HRManagement.Domain.Interfaces;
public interface ILeaveBalanceRepository
{
    Task<LeaveBalance?> GetByEmployeeAndYearAsync(Guid employeeId, int year);
    Task<LeaveBalance> CreateAsync(LeaveBalance balance);
    Task UpdateAsync(LeaveBalance balance);
    Task<LeaveBalance> GetOrCreateForCurrentYearAsync(Guid employeeId, int annualQuota);
}
