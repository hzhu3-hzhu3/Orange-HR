using HRManagement.Domain.Entities;
namespace HRManagement.Domain.Interfaces;
public interface ILeaveRequestRepository
{
    Task<LeaveRequest?> GetByIdAsync(Guid id);
    Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId);
    Task<IEnumerable<LeaveRequest>> GetPendingForManagerAsync(Guid managerId);
    Task<IEnumerable<LeaveRequest>> GetPendingForHRAsync();
    Task<LeaveRequest> CreateAsync(LeaveRequest request);
    Task UpdateAsync(LeaveRequest request);
}
