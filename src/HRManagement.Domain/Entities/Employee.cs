using HRManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
namespace HRManagement.Domain.Entities;
public class Employee : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public Role Role { get; set; }
    public Guid? TeamId { get; set; }
    public Team? Team { get; set; }
    public Guid? ManagerId { get; set; }
    public Employee? Manager { get; set; }
    public EmploymentStatus Status { get; set; }
    public int AnnualLeaveQuota { get; set; } = 20;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
}
