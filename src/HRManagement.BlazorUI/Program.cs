using HRManagement.Application.Interfaces;
using HRManagement.Application.Services;
using HRManagement.BlazorUI.Components;
using HRManagement.Domain.Entities;
using HRManagement.Domain.Interfaces;
using HRManagement.Infrastructure.Data;
using HRManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    var databaseUri = new Uri(databaseUrl);
    var userInfo = databaseUri.UserInfo.Split(':');
    connectionString = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={databaseUri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string 'DefaultConnection' not found.");
}
var isPostgres = connectionString.Contains("Host=") || connectionString.Contains("Server=") && connectionString.Contains("Port=");
builder.Services.AddDbContext<HRManagementDbContext>(options =>
{
    if (isPostgres)
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<ILeaveBalanceRepository, LeaveBalanceRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddIdentity<Employee, IdentityRole<Guid>>(options =>
{
    var passwordPolicy = builder.Configuration.GetSection("PasswordPolicy");
    options.Password.RequireDigit = passwordPolicy.GetValue<bool>("RequireDigit", true);
    options.Password.RequireLowercase = passwordPolicy.GetValue<bool>("RequireLowercase", true);
    options.Password.RequireUppercase = passwordPolicy.GetValue<bool>("RequireUppercase", true);
    options.Password.RequireNonAlphanumeric = passwordPolicy.GetValue<bool>("RequireNonAlphanumeric", true);
    options.Password.RequiredLength = passwordPolicy.GetValue<int>("RequiredLength", 8);
    options.Password.RequiredUniqueChars = passwordPolicy.GetValue<int>("RequiredUniqueChars", 1);
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.ClaimsIdentity.RoleClaimType = System.Security.Claims.ClaimTypes.Role;
    options.ClaimsIdentity.UserNameClaimType = System.Security.Claims.ClaimTypes.Name;
    options.ClaimsIdentity.UserIdClaimType = System.Security.Claims.ClaimTypes.NameIdentifier;
})
.AddEntityFrameworkStores<HRManagementDbContext>()
.AddDefaultTokenProviders()
.AddClaimsPrincipalFactory<HRManagement.BlazorUI.CustomUserClaimsPrincipalFactory>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, HRManagement.BlazorUI.IdentityRevalidatingAuthenticationStateProvider>();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<HRManagementDbContext>();
        var userManager = services.GetRequiredService<UserManager<Employee>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully.");
        logger.LogInformation("Seeding database with initial data...");
        var seeder = new DbSeeder(context, userManager, roleManager);
        await seeder.SeedAsync();
        logger.LogInformation("Database seeded successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();
