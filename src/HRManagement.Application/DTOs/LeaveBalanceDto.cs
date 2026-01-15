namespace HRManagement.Application.DTOs;
public record LeaveBalanceDto(
    Guid Id,
    Guid EmployeeId,
    int Year,
    int TotalDays,
    int UsedDays,
    int PendingDays,
    int RemainingDays
);
