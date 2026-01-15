using HRManagement.Application.DTOs;
using HRManagement.Domain.Enums;
namespace HRManagement.Application.Interfaces;
public interface IAuditService
{
    Task LogActionAsync(Guid actorId, AuditAction action, string entityType, Guid entityId, string? details = null);
    Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(int pageSize = 100, int page = 1);
    Task<IEnumerable<AuditLogDto>> GetEntityAuditLogsAsync(string entityType, Guid entityId);
}
