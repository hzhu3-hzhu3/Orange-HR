using HRManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
namespace HRManagement.BlazorUI;
public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<Employee, IdentityRole<Guid>>
{
    public CustomUserClaimsPrincipalFactory(
        UserManager<Employee> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Employee user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        var roles = await UserManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
        return identity;
    }
}
