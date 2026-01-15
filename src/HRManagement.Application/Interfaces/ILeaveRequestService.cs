using HRManagement.Application.DTOs;
namespace HRManagement.Application.Interfaces;
public interface ILeaveRequestService
{
    Task<LeaveRequestDto> SubmitLeaveRequestAsync(CreateLeaveRequestDto request, Guid employeeId);
    Task<IEnumerable<LeaveRequestDto>> GetEmployeeRequestsAsync(Guid employeeId);
    Task<IEnumerable<LeaveRequestDto>> GetPendingRequestsForManagerAsync(Guid managerId);
    Task<IEnumerable<LeaveRequestDto>> GetPendingRequestsForHRAsync(Guid hrUserId);
    Task<LeaveBalanceDto> GetEmployeeLeaveBalanceAsync(Guid employeeId);
    Task ApproveRequestAsManagerAsync(Guid requestId, Guid managerId);
    Task RejectRequestAsManagerAsync(Guid requestId, Guid managerId, string reason);
    Task ApproveRequestAsHRAsync(Guid requestId, Guid hrUserId);
}
