using HRManagement.Domain.Enums;
namespace HRManagement.Application.DTOs;
public record AuditLogDto(
    Guid Id,
    string ActorName,
    AuditAction Action,
    string EntityType,
    Guid EntityId,
    string? Details,
    DateTime Timestamp
);
