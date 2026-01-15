using HRManagement.Domain.Enums;
namespace HRManagement.Application.DTOs;
public record EmployeeDto(
    Guid Id,
    string Name,
    string Email,
    Role Role,
    Guid? TeamId,
    string? TeamName,
    Guid? ManagerId,
    string? ManagerName,
    EmploymentStatus Status
);
