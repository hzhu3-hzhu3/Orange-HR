using HRManagement.Domain.Enums;
namespace HRManagement.Application.DTOs;
public record CreateEmployeeRequest(
    string Name,
    string Email,
    string Password,
    Role Role,
    Guid? TeamId,
    Guid? ManagerId
);
