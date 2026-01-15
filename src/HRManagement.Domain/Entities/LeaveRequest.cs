using HRManagement.Domain.Enums;
namespace HRManagement.Domain.Entities;
public class LeaveRequest
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime SubmittedAt { get; set; }
    public Guid? ReviewedByManagerId { get; set; }
    public DateTime? ManagerReviewedAt { get; set; }
    public Guid? ApprovedByHRId { get; set; }
    public DateTime? HRApprovedAt { get; set; }
    public bool IsReadOnly => Status == RequestStatus.Approved;
}
