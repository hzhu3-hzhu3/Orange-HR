using HRManagement.Application.DTOs;
using HRManagement.Application.Interfaces;
using HRManagement.Domain.Enums;
using HRManagement.Domain.Interfaces;
namespace HRManagement.Application.Services;
public class DashboardService : IDashboardService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly ILeaveBalanceRepository _leaveBalanceRepository;
    public DashboardService(
        IEmployeeRepository employeeRepository,
        ILeaveRequestRepository leaveRequestRepository,
        ILeaveBalanceRepository leaveBalanceRepository)
    {
        _employeeRepository = employeeRepository;
        _leaveRequestRepository = leaveRequestRepository;
        _leaveBalanceRepository = leaveBalanceRepository;
    }
    public async Task<EmployeeDashboardStatsDto> GetEmployeeStatsAsync(Guid employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
        {
            throw new ArgumentException("Employee not found");
        }
        var allRequests = await _leaveRequestRepository.GetByEmployeeIdAsync(employeeId);
        var requestsList = allRequests.ToList();
        var totalRequests = requestsList.Count;
        var pendingRequests = requestsList.Count(r => r.Status == RequestStatus.Pending);
        var approvedRequests = requestsList.Count(r => r.Status == RequestStatus.Approved);
        var rejectedRequests = requestsList.Count(r => r.Status == RequestStatus.Rejected);
        var today = DateTime.UtcNow.Date;
        var upcomingLeave = requestsList
            .Where(r => r.Status == RequestStatus.Approved && r.StartDate >= today)
            .OrderBy(r => r.StartDate)
            .Take(5)
            .Select(r => MapToDto(r, employee.Name))
            .ToList();
        var currentYear = DateTime.UtcNow.Year;
        var balance = await _leaveBalanceRepository.GetByEmployeeAndYearAsync(employeeId, currentYear);
        LeaveBalanceDto? balanceDto = null;
        if (balance != null)
        {
            balanceDto = new LeaveBalanceDto(
                balance.Id,
                balance.EmployeeId,
                balance.Year,
                balance.TotalDays,
                balance.UsedDays,
                balance.PendingDays,
                balance.RemainingDays
            );
        }
        return new EmployeeDashboardStatsDto(
            totalRequests,
            pendingRequests,
            approvedRequests,
            rejectedRequests,
            balanceDto,
            upcomingLeave
        );
    }
    public async Task<ManagerDashboardStatsDto> GetManagerStatsAsync(Guid managerId)
    {
        var manager = await _employeeRepository.GetByIdAsync(managerId);
        if (manager == null || manager.Role != Role.Manager)
        {
            throw new ArgumentException("Manager not found");
        }
        var pendingApprovals = await _leaveRequestRepository.GetPendingForManagerAsync(managerId);
        var pendingCount = pendingApprovals.Count();
        var teamMembers = await _employeeRepository.GetDirectReportsAsync(managerId);
        var teamMembersList = teamMembers.ToList();
        var teamMembersCount = teamMembersList.Count;
        var today = DateTime.UtcNow.Date;
        var teamOnLeaveToday = 0;
        var teamOnLeaveThisWeek = 0;
        var endOfWeek = today.AddDays(7);
        foreach (var member in teamMembersList)
        {
            var memberRequests = await _leaveRequestRepository.GetByEmployeeIdAsync(member.Id);
            var approvedRequests = memberRequests.Where(r => r.Status == RequestStatus.Approved);
            foreach (var request in approvedRequests)
            {
                if (request.StartDate <= today && request.EndDate >= today)
                {
                    teamOnLeaveToday++;
                }
                if (request.StartDate <= endOfWeek && request.EndDate >= today)
                {
                    teamOnLeaveThisWeek++;
                }
            }
        }
        var recentTeamRequests = new List<LeaveRequestDto>();
        foreach (var member in teamMembersList.Take(10))
        {
            var memberRequests = await _leaveRequestRepository.GetByEmployeeIdAsync(member.Id);
            foreach (var request in memberRequests.OrderByDescending(r => r.SubmittedAt).Take(2))
            {
                recentTeamRequests.Add(MapToDto(request, member.Name));
            }
        }
        recentTeamRequests = recentTeamRequests.OrderByDescending(r => r.SubmittedAt).Take(10).ToList();
        var currentYear = DateTime.UtcNow.Year;
        var balance = await _leaveBalanceRepository.GetByEmployeeAndYearAsync(managerId, currentYear);
        LeaveBalanceDto? balanceDto = null;
        if (balance != null)
        {
            balanceDto = new LeaveBalanceDto(
                balance.Id,
                balance.EmployeeId,
                balance.Year,
                balance.TotalDays,
                balance.UsedDays,
                balance.PendingDays,
                balance.RemainingDays
            );
        }
        return new ManagerDashboardStatsDto(
            pendingCount,
            teamMembersCount,
            teamOnLeaveToday,
            teamOnLeaveThisWeek,
            recentTeamRequests,
            balanceDto
        );
    }
    public async Task<HRDashboardStatsDto> GetHRStatsAsync()
    {
        var allEmployees = await _employeeRepository.GetAllAsync();
        var employeesList = allEmployees.ToList();
        var totalEmployees = employeesList.Count;
        var activeEmployees = employeesList.Count(e => e.Status == EmploymentStatus.Active);
        var pendingHRApprovals = await _leaveRequestRepository.GetPendingForHRAsync();
        var pendingHRCount = pendingHRApprovals.Count();
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var allRequests = new List<Domain.Entities.LeaveRequest>();
        foreach (var employee in employeesList)
        {
            var employeeRequests = await _leaveRequestRepository.GetByEmployeeIdAsync(employee.Id);
            allRequests.AddRange(employeeRequests);
        }
        var thisMonthRequests = allRequests.Where(r => r.SubmittedAt >= startOfMonth).ToList();
        var totalRequestsThisMonth = thisMonthRequests.Count;
        var approvedThisMonth = thisMonthRequests.Count(r => r.Status == RequestStatus.Approved);
        var rejectedThisMonth = thisMonthRequests.Count(r => r.Status == RequestStatus.Rejected);
        var today = DateTime.UtcNow.Date;
        var employeesOnLeaveToday = 0;
        foreach (var employee in employeesList)
        {
            var employeeRequests = await _leaveRequestRepository.GetByEmployeeIdAsync(employee.Id);
            var approvedRequests = employeeRequests.Where(r => r.Status == RequestStatus.Approved);
            foreach (var request in approvedRequests)
            {
                if (request.StartDate <= today && request.EndDate >= today)
                {
                    employeesOnLeaveToday++;
                    break;
                }
            }
        }
        var recentRequests = allRequests
            .OrderByDescending(r => r.SubmittedAt)
            .Take(10)
            .Select(r =>
            {
                var employee = employeesList.FirstOrDefault(e => e.Id == r.EmployeeId);
                return MapToDto(r, employee?.Name ?? "Unknown");
            })
            .ToList();
        return new HRDashboardStatsDto(
            totalEmployees,
            activeEmployees,
            pendingHRCount,
            totalRequestsThisMonth,
            approvedThisMonth,
            rejectedThisMonth,
            employeesOnLeaveToday,
            recentRequests
        );
    }
    private static LeaveRequestDto MapToDto(Domain.Entities.LeaveRequest request, string employeeName)
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
}
