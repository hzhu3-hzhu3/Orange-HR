using Microsoft.AspNetCore.Authorization;
namespace HRManagement.Web.Authorization;
public class CanApproveLeaveRequestRequirement : IAuthorizationRequirement
{
    public Guid LeaveRequestId { get; }
    public CanApproveLeaveRequestRequirement(Guid leaveRequestId)
    {
        LeaveRequestId = leaveRequestId;
    }
}
