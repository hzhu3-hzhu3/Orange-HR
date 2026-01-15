namespace HRManagement.Application.DTOs;
public record CreateLeaveRequestDto(
    DateTime StartDate,
    DateTime EndDate,
    string Reason
);
