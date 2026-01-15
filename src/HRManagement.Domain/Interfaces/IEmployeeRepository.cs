using HRManagement.Domain.Entities;
namespace HRManagement.Domain.Interfaces;
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id);
    Task<Employee?> GetByEmailAsync(string email);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<IEnumerable<Employee>> GetDirectReportsAsync(Guid managerId);
    Task<Employee> CreateAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task<bool> ExistsAsync(Guid id);
}
