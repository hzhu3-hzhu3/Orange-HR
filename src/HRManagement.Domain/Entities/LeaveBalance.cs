namespace HRManagement.Domain.Entities;
public class LeaveBalance
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public int Year { get; set; }
    public int TotalDays { get; set; }
    public int UsedDays { get; set; }
    public int RemainingDays => TotalDays - UsedDays;
    public int PendingDays { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Employee Employee { get; set; } = null!;
    public bool HasSufficientBalance(int requestedDays)
    {
        return RemainingDays >= requestedDays;
    }
    public void ReserveDays(int days)
    {
        PendingDays += days;
    }
    public void ReleaseDays(int days)
    {
        PendingDays = Math.Max(0, PendingDays - days);
    }
    public void ConfirmUsage(int days)
    {
        UsedDays += days;
        PendingDays = Math.Max(0, PendingDays - days);
        UpdatedAt = DateTime.UtcNow;
    }
}
