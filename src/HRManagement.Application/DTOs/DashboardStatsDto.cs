namespace HRManagement.Application.DTOs;
public record EmployeeDashboardStatsDto(
    int TotalLeaveRequests,
    int PendingRequests,
    int ApprovedRequests,
    int RejectedRequests,
    LeaveBalanceDto? CurrentYearBalance,
    IEnumerable<LeaveRequestDto> UpcomingLeave
);
public record ManagerDashboardStatsDto(
    int PendingApprovals,
    int TeamMembersCount,
    int TeamOnLeaveToday,
    int TeamOnLeaveThisWeek,
    IEnumerable<LeaveRequestDto> RecentTeamRequests,
    LeaveBalanceDto? MyBalance
);
public record HRDashboardStatsDto(
    int TotalEmployees,
    int ActiveEmployees,
    int PendingHRApprovals,
    int TotalRequestsThisMonth,
    int ApprovedThisMonth,
    int RejectedThisMonth,
    int EmployeesOnLeaveToday,
    IEnumerable<LeaveRequestDto> RecentRequests
);
