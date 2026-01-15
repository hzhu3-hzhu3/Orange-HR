using HRManagement.Application.DTOs;
using HRManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HRManagement.Web.Controllers;
[ApiController]
[Route("api/audit-logs")]
[Authorize(Policy = "HROnly")] // Requirement 10.3: Only HR can access audit logs
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;
    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogs(
        [FromQuery] int pageSize = 100, 
        [FromQuery] int page = 1)
    {
        var logs = await _auditService.GetAuditLogsAsync(pageSize, page);
        return Ok(logs);
    }
    [HttpGet("entity/{type}/{id}")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetEntityAuditLogs(string type, Guid id)
    {
        var logs = await _auditService.GetEntityAuditLogsAsync(type, id);
        return Ok(logs);
    }
}
