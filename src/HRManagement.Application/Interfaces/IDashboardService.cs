using HRManagement.Application.DTOs;
namespace HRManagement.Application.Interfaces;
public interface IDashboardService
{
    Task<EmployeeDashboardStatsDto> GetEmployeeStatsAsync(Guid employeeId);
    Task<ManagerDashboardStatsDto> GetManagerStatsAsync(Guid managerId);
    Task<HRDashboardStatsDto> GetHRStatsAsync();
}
