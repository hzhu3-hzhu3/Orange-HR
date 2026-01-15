using HRManagement.Application.DTOs;
using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;
using HRManagement.Domain.Enums;
using HRManagement.Domain.Interfaces;
namespace HRManagement.Application.Services;
public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IEmployeeRepository _employeeRepository;
    public AuditService(
        IAuditLogRepository auditLogRepository,
        IEmployeeRepository employeeRepository)
    {
        _auditLogRepository = auditLogRepository;
        _employeeRepository = employeeRepository;
    }
    public async Task LogActionAsync(Guid actorId, AuditAction action, string entityType, Guid entityId, string? details = null)
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorId = actorId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            Timestamp = DateTime.UtcNow
        };
        await _auditLogRepository.CreateAsync(auditLog);
    }
    public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(int pageSize = 100, int page = 1)
    {
        var logs = await _auditLogRepository.GetAllAsync(pageSize, page);
        var dtos = new List<AuditLogDto>();
        foreach (var log in logs)
        {
            var actor = await _employeeRepository.GetByIdAsync(log.ActorId);
            dtos.Add(MapToDto(log, actor?.Name ?? "Unknown"));
        }
        return dtos;
    }
    public async Task<IEnumerable<AuditLogDto>> GetEntityAuditLogsAsync(string entityType, Guid entityId)
    {
        var logs = await _auditLogRepository.GetByEntityAsync(entityType, entityId);
        var dtos = new List<AuditLogDto>();
        foreach (var log in logs)
        {
            var actor = await _employeeRepository.GetByIdAsync(log.ActorId);
            dtos.Add(MapToDto(log, actor?.Name ?? "Unknown"));
        }
        return dtos;
    }
    private static AuditLogDto MapToDto(AuditLog log, string actorName)
    {
        return new AuditLogDto(
            log.Id,
            actorName,
            log.Action,
            log.EntityType,
            log.EntityId,
            log.Details,
            log.Timestamp
        );
    }
}
