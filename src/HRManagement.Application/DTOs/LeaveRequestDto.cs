using HRManagement.Domain.Enums;
namespace HRManagement.Application.DTOs;
public record LeaveRequestDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    DateTime StartDate,
    DateTime EndDate,
    string Reason,
    RequestStatus Status,
    string? RejectionReason,
    DateTime SubmittedAt,
    bool IsReadOnly
);
