using HRManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
namespace HRManagement.BlazorUI.Pages;
public class LoginModel : PageModel
{
    private readonly SignInManager<Employee> _signInManager;
    private readonly ILogger<LoginModel> _logger;
    public LoginModel(SignInManager<Employee> signInManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }
    [BindProperty]
    public InputModel Input { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public class InputModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
    public void OnGet()
    {
    }
    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= "/";
        _logger.LogInformation("Login attempt started for email: {Email}", Input.Email);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogWarning("Validation error: {Error}", error.ErrorMessage);
            }
            return Page();
        }
        try
        {
            _logger.LogInformation("Attempting sign in for: {Email}", Input.Email);
            var result = await _signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );
            _logger.LogInformation("Sign in result - Succeeded: {Succeeded}, IsLockedOut: {IsLockedOut}, RequiresTwoFactor: {RequiresTwoFactor}", 
                result.Succeeded, result.IsLockedOut, result.RequiresTwoFactor);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} logged in successfully, redirecting to {ReturnUrl}", Input.Email, returnUrl);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {Email} account locked out", Input.Email);
                ErrorMessage = "Your account has been locked out. Please try again later.";
            }
            else
            {
                _logger.LogWarning("Invalid login attempt for {Email}", Input.Email);
                ErrorMessage = "Invalid email or password. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", Input.Email);
            ErrorMessage = "An error occurred during login. Please try again.";
        }
        return Page();
    }
}
