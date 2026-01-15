using HRManagement.Domain.Entities;
using HRManagement.Domain.Enums;
using HRManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace HRManagement.Web.Authorization;
public class CanViewEmployeeHandler : AuthorizationHandler<CanViewEmployeeRequirement>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly UserManager<Employee> _userManager;
    public CanViewEmployeeHandler(
        IEmployeeRepository employeeRepository,
        UserManager<Employee> userManager)
    {
        _employeeRepository = employeeRepository;
        _userManager = userManager;
    }
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanViewEmployeeRequirement requirement)
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
        if (currentUser.Role == Role.HR)
        {
            context.Succeed(requirement);
            return;
        }
        if (userId == requirement.TargetEmployeeId)
        {
            context.Succeed(requirement);
            return;
        }
        if (currentUser.Role == Role.Manager)
        {
            var targetEmployee = await _employeeRepository.GetByIdAsync(requirement.TargetEmployeeId);
            if (targetEmployee != null && targetEmployee.ManagerId == userId)
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
