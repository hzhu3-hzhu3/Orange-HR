using HRManagement.Domain.Entities;
namespace HRManagement.Domain.Interfaces;
public interface IAuditLogRepository
{
    Task<IEnumerable<AuditLog>> GetAllAsync(int pageSize = 100, int page = 1);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, Guid entityId);
    Task CreateAsync(AuditLog log);
}
