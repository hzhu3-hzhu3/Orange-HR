using HRManagement.Application.DTOs;
using HRManagement.Application.Exceptions;
using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;
using HRManagement.Domain.Enums;
using HRManagement.Domain.Interfaces;
namespace HRManagement.Application.Services;
public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILeaveBalanceRepository _leaveBalanceRepository;
    private readonly IAuditService _auditService;
    public LeaveRequestService(
        ILeaveRequestRepository leaveRequestRepository,
        IEmployeeRepository employeeRepository,
        ILeaveBalanceRepository leaveBalanceRepository,
        IAuditService auditService)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _employeeRepository = employeeRepository;
        _leaveBalanceRepository = leaveBalanceRepository;
        _auditService = auditService;
    }
    public async Task<LeaveRequestDto> SubmitLeaveRequestAsync(CreateLeaveRequestDto request, Guid employeeId)
    {
        if (request.EndDate < request.StartDate)
        {
            throw new ArgumentException("End date cannot be before start date");
        }
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
        {
            throw new ArgumentException("Employee not found");
        }
        var requestedDays = (request.EndDate - request.StartDate).Days + 1;
        var balance = await _leaveBalanceRepository.GetOrCreateForCurrentYearAsync(
            employeeId, 
            employee.AnnualLeaveQuota);
        if (!balance.HasSufficientBalance(requestedDays))
        {
            throw new InvalidOperationException(
                $"Insufficient leave balance. You have {balance.RemainingDays} days remaining, but requested {requestedDays} days.");
        }
        var leaveRequest = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason,
            Status = RequestStatus.Pending,
            SubmittedAt = DateTime.UtcNow
        };
        await _leaveRequestRepository.CreateAsync(leaveRequest);
        balance.ReserveDays(requestedDays);
        await _leaveBalanceRepository.UpdateAsync(balance);
        await _auditService.LogActionAsync(
            employeeId,
            AuditAction.LeaveRequestSubmitted,
            nameof(LeaveRequest),
            leaveRequest.Id,
            $"Leave request submitted: {request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd} ({requestedDays} days)"
        );
        return MapToDto(leaveRequest, employee.Name);
    }
    public async Task<IEnumerable<LeaveRequestDto>> GetEmployeeRequestsAsync(Guid employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
        {
            throw new ArgumentException("Employee not found");
        }
        var requests = await _leaveRequestRepository.GetByEmployeeIdAsync(employeeId);
        return requests.Select(r => MapToDto(r, employee.Name));
    }
    public async Task<IEnumerable<LeaveRequestDto>> GetPendingRequestsForManagerAsync(Guid managerId)
    {
        var manager = await _employeeRepository.GetByIdAsync(managerId);
        if (manager == null || manager.Role != Role.Manager)
        {
            throw new UnauthorizedException("Only managers can view pending requests");
        }
        var requests = await _leaveRequestRepository.GetPendingForManagerAsync(managerId);
        var dtos = new List<LeaveRequestDto>();
        foreach (var request in requests)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            dtos.Add(MapToDto(request, employee?.Name ?? "Unknown"));
        }
        return dtos;
    }
    public async Task<IEnumerable<LeaveRequestDto>> GetPendingRequestsForHRAsync(Guid hrUserId)
    {
        var hrUser = await _employeeRepository.GetByIdAsync(hrUserId);
        if (hrUser == null || hrUser.Role != Role.HR)
        {
            throw new UnauthorizedException("Only HR can view requests pending HR approval");
        }
        var pendingHRApproval = await _leaveRequestRepository.GetPendingForHRAsync();
        var dtos = new List<LeaveRequestDto>();
        foreach (var request in pendingHRApproval)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            dtos.Add(MapToDto(request, employee?.Name ?? "Unknown"));
        }
        return dtos;
    }
    public async Task ApproveRequestAsManagerAsync(Guid requestId, Guid managerId)
    {
        var manager = await _employeeRepository.GetByIdAsync(managerId);
        if (manager == null || manager.Role != Role.Manager)
        {
            throw new UnauthorizedException("Only managers can approve requests");
        }
        var request = await _leaveRequestRepository.GetByIdAsync(requestId);
        if (request == null)
        {
            throw new ArgumentException("Leave request not found");
        }
        if (request.EmployeeId == managerId)
        {
            throw new UnauthorizedException("Managers cannot approve their own leave requests");
        }
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        if (employee == null || employee.ManagerId != managerId)
        {
            throw new UnauthorizedException("You can only approve requests from your direct reports");
        }
        ValidateStateTransition(request.Status, RequestStatus.Approved, "approve");
        if (request.IsReadOnly)
        {
            throw new InvalidStateTransitionException("Cannot modify an approved request");
        }
        var requestedDays = (request.EndDate - request.StartDate).Days + 1;
        var requestYear = request.StartDate.Year;
        var balance = await _leaveBalanceRepository.GetByEmployeeAndYearAsync(request.EmployeeId, requestYear);
        if (balance != null)
        {
            balance.ConfirmUsage(requestedDays);
            await _leaveBalanceRepository.UpdateAsync(balance);
        }
        request.Status = RequestStatus.Approved;
        request.ReviewedByManagerId = managerId;
        request.ManagerReviewedAt = DateTime.UtcNow;
        await _leaveRequestRepository.UpdateAsync(request);
        await _auditService.LogActionAsync(
            managerId,
            AuditAction.LeaveRequestApprovedByManager,
            nameof(LeaveRequest),
            request.Id,
            $"Manager approved leave request for {employee.Name} ({requestedDays} days)"
        );
    }
    public async Task RejectRequestAsManagerAsync(Guid requestId, Guid managerId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Rejection reason is required");
        }
        var manager = await _employeeRepository.GetByIdAsync(managerId);
        if (manager == null || manager.Role != Role.Manager)
        {
            throw new UnauthorizedException("Only managers can reject requests");
        }
        var request = await _leaveRequestRepository.GetByIdAsync(requestId);
        if (request == null)
        {
            throw new ArgumentException("Leave request not found");
        }
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        if (employee == null || employee.ManagerId != managerId)
        {
            throw new UnauthorizedException("You can only reject requests from your direct reports");
        }
        ValidateStateTransition(request.Status, RequestStatus.Rejected, "reject");
        if (request.IsReadOnly)
        {
            throw new InvalidStateTransitionException("Cannot modify an approved request");
        }
        var requestedDays = (request.EndDate - request.StartDate).Days + 1;
        var requestYear = request.StartDate.Year;
        var balance = await _leaveBalanceRepository.GetByEmployeeAndYearAsync(request.EmployeeId, requestYear);
        if (balance != null)
        {
            balance.ReleaseDays(requestedDays);
            await _leaveBalanceRepository.UpdateAsync(balance);
        }
        request.Status = RequestStatus.Rejected;
        request.RejectionReason = reason;
        request.ReviewedByManagerId = managerId;
        request.ManagerReviewedAt = DateTime.UtcNow;
        await _leaveRequestRepository.UpdateAsync(request);
        await _auditService.LogActionAsync(
            managerId,
            AuditAction.LeaveRequestRejectedByManager,
            nameof(LeaveRequest),
            request.Id,
            $"Manager rejected leave request for {employee.Name}: {reason}"
        );
    }
    public async Task<LeaveBalanceDto> GetEmployeeLeaveBalanceAsync(Guid employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
        {
            throw new ArgumentException("Employee not found");
        }
        var balance = await _leaveBalanceRepository.GetOrCreateForCurrentYearAsync(
            employeeId,
            employee.AnnualLeaveQuota);
        return new LeaveBalanceDto(
            balance.Id,
            balance.EmployeeId,
            balance.Year,
            balance.TotalDays,
            balance.UsedDays,
            balance.PendingDays,
            balance.RemainingDays
        );
    }
    public async Task ApproveRequestAsHRAsync(Guid requestId, Guid hrUserId)
    {
        var hrUser = await _employeeRepository.GetByIdAsync(hrUserId);
        if (hrUser == null || hrUser.Role != Role.HR)
        {
            throw new UnauthorizedException("Only HR can provide final approval");
        }
        var request = await _leaveRequestRepository.GetByIdAsync(requestId);
        if (request == null)
        {
            throw new ArgumentException("Leave request not found");
        }
        if (request.Status != RequestStatus.Approved)
        {
            throw new InvalidStateTransitionException($"HR can only approve requests that have been approved by a manager");
        }
        request.ApprovedByHRId = hrUserId;
        request.HRApprovedAt = DateTime.UtcNow;
        await _leaveRequestRepository.UpdateAsync(request);
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        await _auditService.LogActionAsync(
            hrUserId,
            AuditAction.LeaveRequestApprovedByHR,
            nameof(LeaveRequest),
            request.Id,
            $"HR provided final approval for {employee?.Name ?? "Unknown"}'s leave request"
        );
    }
    private static LeaveRequestDto MapToDto(LeaveRequest request, string employeeName)
    {
        return new LeaveRequestDto(
            request.Id,
            request.EmployeeId,
            employeeName,
            request.StartDate,
            request.EndDate,
            request.Reason,
            request.Status,
            request.RejectionReason,
            request.SubmittedAt,
            request.IsReadOnly
        );
    }
    private static void ValidateStateTransition(RequestStatus currentStatus, RequestStatus newStatus, string action)
    {
        var validTransitions = new Dictionary<RequestStatus, HashSet<RequestStatus>>
        {
            { RequestStatus.Pending, new HashSet<RequestStatus> { RequestStatus.Approved, RequestStatus.Rejected } },
            { RequestStatus.Approved, new HashSet<RequestStatus>() }, // Approved is terminal (read-only)
            { RequestStatus.Rejected, new HashSet<RequestStatus>() }  // Rejected is terminal
        };
        if (!validTransitions.ContainsKey(currentStatus))
        {
            throw new InvalidStateTransitionException($"Unknown status: {currentStatus}");
        }
        if (!validTransitions[currentStatus].Contains(newStatus))
        {
            throw new InvalidStateTransitionException(
                $"Cannot {action} a request with status {currentStatus}. " +
                $"Valid transitions from {currentStatus}: {string.Join(", ", validTransitions[currentStatus])}");
        }
    }
}
