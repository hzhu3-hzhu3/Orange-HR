namespace HRManagement.Domain.Enums;
public enum AuditAction
{
    EmployeeCreated,
    EmployeeUpdated,
    EmployeeDeactivated,
    EmployeeActivated,
    LeaveRequestSubmitted,
    LeaveRequestApprovedByManager,
    LeaveRequestRejectedByManager,
    LeaveRequestApprovedByHR,
    TeamAssignmentChanged,
    ManagerAssignmentChanged
}
