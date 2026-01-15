using HRManagement.Domain.Entities;
using HRManagement.Domain.Enums;
using HRManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace HRManagement.Web.Authorization;
public class CanApproveLeaveRequestHandler : AuthorizationHandler<CanApproveLeaveRequestRequirement>
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly UserManager<Employee> _userManager;
    public CanApproveLeaveRequestHandler(
        ILeaveRequestRepository leaveRequestRepository,
        IEmployeeRepository employeeRepository,
        UserManager<Employee> userManager)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _employeeRepository = employeeRepository;
        _userManager = userManager;
    }
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanApproveLeaveRequestRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return;
        }
        var currentUser = await _employeeRepository.GetByIdAsync(userId);
        if (currentUser == null)
        {
            return;
        }
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(requirement.LeaveRequestId);
        if (leaveRequest == null)
        {
            return;
        }
        if (leaveRequest.EmployeeId == userId)
        {
            return;
        }
        if (currentUser.Role == Role.HR)
        {
            context.Succeed(requirement);
            return;
        }
        if (currentUser.Role == Role.Manager)
        {
            var requestingEmployee = await _employeeRepository.GetByIdAsync(leaveRequest.EmployeeId);
            if (requestingEmployee != null && requestingEmployee.ManagerId == userId)
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
