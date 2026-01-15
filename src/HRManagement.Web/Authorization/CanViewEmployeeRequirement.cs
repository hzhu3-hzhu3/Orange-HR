using Microsoft.AspNetCore.Authorization;
namespace HRManagement.Web.Authorization;
public class CanViewEmployeeRequirement : IAuthorizationRequirement
{
    public Guid TargetEmployeeId { get; }
    public CanViewEmployeeRequirement(Guid targetEmployeeId)
    {
        TargetEmployeeId = targetEmployeeId;
    }
}
