using FsCheck;
using FsCheck.Xunit;
using HRManagement.Domain.Entities;
using HRManagement.Domain.Enums;
using Xunit;
namespace HRManagement.UnitTests;
public class DomainEntityPropertiesTests
{
    [Property(MaxTest = 100)]
    public bool EachEmployeeHasExactlyOneRole(int roleIndex)
    {
        var roles = new[] { Role.Employee, Role.Manager, Role.HR };
        var role = roles[Math.Abs(roleIndex) % roles.Length];
        var employee = CreateTestEmployee(role);
        var hasRole = Enum.IsDefined(typeof(Role), employee.Role);
        var isValidRole = employee.Role == Role.Employee || 
                         employee.Role == Role.Manager || 
                         employee.Role == Role.HR;
        return hasRole && isValidRole;
    }
    [Property(MaxTest = 100)]
    public bool EmployeesHaveZeroOrOneManager(bool hasManager)
    {
        var managerId = hasManager ? Guid.NewGuid() : (Guid?)null;
        var employee = CreateTestEmployee(Role.Employee, managerId);
        if (employee.ManagerId == null)
        {
            return employee.Manager == null;
        }
        else
        {
            return employee.ManagerId != Guid.Empty && 
                   (employee.Manager == null || employee.Manager is Employee);
        }
    }
    private static Employee CreateTestEmployee(Role role, Guid? managerId = null)
    {
        var id = Guid.NewGuid();
        return new Employee
        {
            Id = id,
            Name = $"Employee{id.ToString().Substring(0, 8)}",
            Email = $"employee{id.ToString().Substring(0, 8)}@test.com",
            Role = role,
            ManagerId = managerId,
            Manager = null, // Navigation property not loaded in tests
            TeamId = null,
            Team = null, // Navigation property not loaded in tests
            Status = EmploymentStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            DirectReports = new List<Employee>(),
            LeaveRequests = new List<LeaveRequest>()
        };
    }
}
