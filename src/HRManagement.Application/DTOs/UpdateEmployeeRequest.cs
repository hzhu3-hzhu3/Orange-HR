using HRManagement.Domain.Enums;
namespace HRManagement.Application.DTOs;
public record UpdateEmployeeRequest(
    string? Name,
    string? Email,
    Role? Role,
    Guid? TeamId,
    Guid? ManagerId
);
