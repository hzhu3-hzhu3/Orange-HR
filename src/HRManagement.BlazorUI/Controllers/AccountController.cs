using HRManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace HRManagement.BlazorUI.Controllers;
[Route("[controller]/[action]")]
public class AccountController : Controller
{
    private readonly SignInManager<Employee> _signInManager;
    private readonly UserManager<Employee> _userManager;
    public AccountController(SignInManager<Employee> signInManager, UserManager<Employee> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
    [HttpGet]
    public async Task<IActionResult> PerformLogin(string email, string password, bool rememberMe, string returnUrl = "/")
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Redirect($"/login?error=Invalid credentials");
        }
        if (user.Status != Domain.Enums.EmploymentStatus.Active)
        {
            return Redirect($"/login?error=Account is inactive");
        }
        await _signInManager.SignOutAsync();
        var result = await _signInManager.PasswordSignInAsync(
            user,
            password,
            rememberMe,
            lockoutOnFailure: true);
        if (result.Succeeded)
        {
            return Redirect(returnUrl);
        }
        else if (result.IsLockedOut)
        {
            return Redirect($"/login?error=Account is locked");
        }
        else
        {
            return Redirect($"/login?error=Invalid credentials");
        }
    }
    [HttpGet]
    public async Task<IActionResult> Logout(string returnUrl = "/login")
    {
        await _signInManager.SignOutAsync();
        return Redirect(returnUrl);
    }
}
