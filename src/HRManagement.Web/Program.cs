using FluentValidation;
using FluentValidation.AspNetCore;
using HRManagement.Application.Interfaces;
using HRManagement.Application.Services;
using HRManagement.Application.Validators;
using HRManagement.Domain.Entities;
using HRManagement.Domain.Enums;
using HRManagement.Domain.Interfaces;
using HRManagement.Infrastructure.Data;
using HRManagement.Infrastructure.Repositories;
using HRManagement.Web.Authorization;
using HRManagement.Web.Extensions;
using HRManagement.Web.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateLeaveRequestValidator>();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string 'DefaultConnection' not found.");
}
builder.Services.AddDbContext<HRManagementDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
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
})
.AddEntityFrameworkStores<HRManagementDbContext>()
.AddDefaultTokenProviders();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HROnly", policy =>
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst(c => c.Type == ClaimTypes.Role || c.Type == "Role");
            return roleClaim != null && roleClaim.Value == Role.HR.ToString();
        }));
    options.AddPolicy("ManagerOrHR", policy =>
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            var roleClaim = user.FindFirst(c => c.Type == ClaimTypes.Role || c.Type == "Role");
            return roleClaim != null && 
                   (roleClaim.Value == Role.Manager.ToString() || 
                    roleClaim.Value == Role.HR.ToString());
        }));
});
builder.Services.AddScoped<IAuthorizationHandler, CanViewEmployeeHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanApproveLeaveRequestHandler>();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    await app.InitializeDatabaseAsync();
}
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
