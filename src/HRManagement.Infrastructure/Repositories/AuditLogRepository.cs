using HRManagement.Domain.Entities;
using HRManagement.Domain.Interfaces;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace HRManagement.Infrastructure.Repositories;
public class AuditLogRepository : IAuditLogRepository
{
    private readonly HRManagementDbContext _context;
    public AuditLogRepository(HRManagementDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<AuditLog>> GetAllAsync(int pageSize = 100, int page = 1)
    {
        return await _context.AuditLogs
            .Include(al => al.Actor)
            .OrderByDescending(al => al.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, Guid entityId)
    {
        return await _context.AuditLogs
            .Include(al => al.Actor)
            .Where(al => al.EntityType == entityType && al.EntityId == entityId)
            .OrderByDescending(al => al.Timestamp)
            .ToListAsync();
    }
    public async Task CreateAsync(AuditLog log)
    {
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
