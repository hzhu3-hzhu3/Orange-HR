using HRManagement.Application.DTOs;
using HRManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace HRManagement.Web.Controllers;
[ApiController]
[Route("api/leave-requests")]
[Authorize] // Requirement 1.1: All endpoints require authentication
public class LeaveRequestController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;
    public LeaveRequestController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetEmployeeRequests()
    {
        var employeeId = GetCurrentUserId();
        var requests = await _leaveRequestService.GetEmployeeRequestsAsync(employeeId);
        return Ok(requests);
    }
    [HttpGet("pending")]
    [Authorize(Policy = "ManagerOrHR")]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetPendingRequests()
    {
        var managerId = GetCurrentUserId();
        var requests = await _leaveRequestService.GetPendingRequestsForManagerAsync(managerId);
        return Ok(requests);
    }
    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> SubmitLeaveRequest([FromBody] CreateLeaveRequestDto request)
    {
        var employeeId = GetCurrentUserId();
        var leaveRequest = await _leaveRequestService.SubmitLeaveRequestAsync(request, employeeId);
        return CreatedAtAction(nameof(GetEmployeeRequests), new { id = leaveRequest.Id }, leaveRequest);
    }
    [HttpPut("{id}/approve")]
    [Authorize(Policy = "ManagerOrHR")]
    public async Task<IActionResult> ApproveRequest(Guid id)
    {
        var approverId = GetCurrentUserId();
        var userRole = User.FindFirst(c => c.Type == ClaimTypes.Role || c.Type == "Role")?.Value;
        if (userRole == "HR")
        {
            await _leaveRequestService.ApproveRequestAsHRAsync(id, approverId);
        }
        else
        {
            await _leaveRequestService.ApproveRequestAsManagerAsync(id, approverId);
        }
        return NoContent();
    }
    [HttpPut("{id}/reject")]
    [Authorize(Policy = "ManagerOrHR")]
    public async Task<IActionResult> RejectRequest(Guid id, [FromBody] RejectLeaveRequestRequest request)
    {
        var managerId = GetCurrentUserId();
        await _leaveRequestService.RejectRequestAsManagerAsync(id, managerId, request.Reason);
        return NoContent();
    }
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in claims");
        }
        return userId;
    }
}
public record RejectLeaveRequestRequest(string Reason);
