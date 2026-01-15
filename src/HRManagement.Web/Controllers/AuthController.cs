using HRManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace HRManagement.Web.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<Employee> _signInManager;
    private readonly UserManager<Employee> _userManager;
    public AuthController(
        SignInManager<Employee> signInManager,
        UserManager<Employee> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new { Message = "Invalid email or password" });
        }
        if (user.Status != Domain.Enums.EmploymentStatus.Active)
        {
            return Unauthorized(new { Message = "Account is inactive" });
        }
        var result = await _signInManager.PasswordSignInAsync(
            user, 
            request.Password, 
            isPersistent: request.RememberMe, 
            lockoutOnFailure: true);
        if (result.Succeeded)
        {
            return Ok(new LoginResponse(
                UserId: user.Id,
                Name: user.Name,
                Email: user.Email ?? string.Empty,
                Role: user.Role.ToString(),
                Message: "Login successful"
            ));
        }
        if (result.IsLockedOut)
        {
            return Unauthorized(new { Message = "Account is locked due to multiple failed login attempts" });
        }
        return Unauthorized(new { Message = "Invalid email or password" });
    }
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { Message = "Logout successful" });
    }
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = GetCurrentUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return NotFound(new { Message = "User not found" });
        }
        var result = await _userManager.ChangePasswordAsync(
            user, 
            request.CurrentPassword, 
            request.NewPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            return Ok(new { Message = "Password changed successfully" });
        }
        var errors = result.Errors.Select(e => e.Description).ToList();
        return BadRequest(new { Message = "Password change failed", Errors = errors });
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
public record LoginRequest(
    string Email,
    string Password,
    bool RememberMe = false
);
public record LoginResponse(
    Guid UserId,
    string Name,
    string Email,
    string Role,
    string Message
);
public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
