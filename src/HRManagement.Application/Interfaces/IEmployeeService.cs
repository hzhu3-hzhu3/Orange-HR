using HRManagement.Application.DTOs;
namespace HRManagement.Application.Interfaces;
public interface IEmployeeService
{
    Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id, Guid requestingUserId);
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(Guid requestingUserId);
    Task<IEnumerable<EmployeeDto>> GetDirectReportsAsync(Guid managerId);
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request, Guid createdBy);
    Task UpdateEmployeeAsync(Guid id, UpdateEmployeeRequest request, Guid updatedBy);
    Task DeactivateEmployeeAsync(Guid id, Guid deactivatedBy);
    Task ActivateEmployeeAsync(Guid id, Guid activatedBy);
}
