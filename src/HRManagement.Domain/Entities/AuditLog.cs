using HRManagement.Domain.Enums;
namespace HRManagement.Domain.Entities;
public class AuditLog
{
    public Guid Id { get; set; }
    public Guid ActorId { get; set; }
    public Employee Actor { get; set; } = null!;
    public AuditAction Action { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }
}
