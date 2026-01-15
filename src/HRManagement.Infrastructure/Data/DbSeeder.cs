using HRManagement.Domain.Entities;
using HRManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace HRManagement.Infrastructure.Data;
public class DbSeeder
{
    private readonly HRManagementDbContext _context;
    private readonly UserManager<Employee> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    public DbSeeder(HRManagementDbContext context, UserManager<Employee> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        if (await _context.Employees.AnyAsync())
        {
            await EnsureRolesAssignedAsync();
            await EnsureLeaveBalancesAsync();
            return;
        }
        var teams = await SeedTeamsAsync();
        var employees = await SeedEmployeesAsync(teams);
        await SeedLeaveBalancesAsync(employees);
        await SeedLeaveRequestsAsync(employees);
        await _context.SaveChangesAsync();
    }
    private async Task EnsureRolesAssignedAsync()
    {
        var employees = await _context.Employees.ToListAsync();
        foreach (var employee in employees)
        {
            var currentRoles = await _userManager.GetRolesAsync(employee);
            string targetRole = employee.Role switch
            {
                Role.HR => "HR",
                Role.Manager => "Manager",
                Role.Employee => "Employee",
                _ => "Employee"
            };
            if (!currentRoles.Contains(targetRole))
            {
                await _userManager.AddToRoleAsync(employee, targetRole);
            }
        }
    }
    private async Task EnsureLeaveBalancesAsync()
    {
        var currentYear = DateTime.UtcNow.Year;
        var employees = await _context.Employees.ToListAsync();
        foreach (var employee in employees)
        {
            var existingBalance = await _context.LeaveBalances
                .FirstOrDefaultAsync(lb => lb.EmployeeId == employee.Id && lb.Year == currentYear);
            if (existingBalance == null)
            {
                var balance = new LeaveBalance
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    Year = currentYear,
                    TotalDays = employee.AnnualLeaveQuota,
                    UsedDays = 0,
                    PendingDays = 0,
                    CreatedAt = DateTime.UtcNow
                };
                _context.LeaveBalances.Add(balance);
            }
        }
        await _context.SaveChangesAsync();
    }
    private async Task SeedLeaveBalancesAsync(Dictionary<string, Employee> employees)
    {
        var currentYear = DateTime.UtcNow.Year;
        var balances = new List<LeaveBalance>();
        foreach (var employee in employees.Values)
        {
            balances.Add(new LeaveBalance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                Year = currentYear,
                TotalDays = employee.AnnualLeaveQuota,
                UsedDays = 0,
                PendingDays = 0,
                CreatedAt = DateTime.UtcNow
            });
        }
        _context.LeaveBalances.AddRange(balances);
        await _context.SaveChangesAsync();
    }
    private async Task SeedRolesAsync()
    {
        var roles = new[] { "Employee", "Manager", "HR" };
        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
            }
        }
    }
    private async Task<Dictionary<string, Team>> SeedTeamsAsync()
    {
        var teams = new Dictionary<string, Team>
        {
            ["Engineering"] = new Team
            {
                Id = Guid.NewGuid(),
                Name = "Engineering"
            },
            ["Marketing"] = new Team
            {
                Id = Guid.NewGuid(),
                Name = "Marketing"
            },
            ["Sales"] = new Team
            {
                Id = Guid.NewGuid(),
                Name = "Sales"
            },
            ["HR"] = new Team
            {
                Id = Guid.NewGuid(),
                Name = "Human Resources"
            }
        };
        foreach (var team in teams.Values)
        {
            _context.Teams.Add(team);
        }
        await _context.SaveChangesAsync();
        return teams;
    }
    private async Task<Dictionary<string, Employee>> SeedEmployeesAsync(Dictionary<string, Team> teams)
    {
        var employees = new Dictionary<string, Employee>();
        var now = DateTime.UtcNow;
        var hrUser = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Sarah Chen",
            Email = "sarah.chen@orange.com",
            UserName = "sarah.chen@orange.com",
            Role = Role.HR,
            TeamId = teams["HR"].Id,
            Status = EmploymentStatus.Active,
            CreatedAt = now,
            EmailConfirmed = true
        };
        var hrResult = await _userManager.CreateAsync(hrUser, "Orange123!");
        if (hrResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(hrUser, "HR");
            employees["HR"] = hrUser;
        }
        var engManager = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Michael Rodriguez",
            Email = "michael.rodriguez@orange.com",
            UserName = "michael.rodriguez@orange.com",
            Role = Role.Manager,
            TeamId = teams["Engineering"].Id,
            Status = EmploymentStatus.Active,
            CreatedAt = now,
            EmailConfirmed = true
        };
        var engManagerResult = await _userManager.CreateAsync(engManager, "Orange123!");
        if (engManagerResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(engManager, "Manager");
            employees["EngManager"] = engManager;
            teams["Engineering"].ManagerId = engManager.Id;
        }
        var mktManager = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Jennifer Kim",
            Email = "jennifer.kim@orange.com",
            UserName = "jennifer.kim@orange.com",
            Role = Role.Manager,
            TeamId = teams["Marketing"].Id,
            Status = EmploymentStatus.Active,
            CreatedAt = now,
            EmailConfirmed = true
        };
        var mktManagerResult = await _userManager.CreateAsync(mktManager, "Orange123!");
        if (mktManagerResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(mktManager, "Manager");
            employees["MktManager"] = mktManager;
            teams["Marketing"].ManagerId = mktManager.Id;
        }
        var salesManager = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "David Thompson",
            Email = "david.thompson@orange.com",
            UserName = "david.thompson@orange.com",
            Role = Role.Manager,
            TeamId = teams["Sales"].Id,
            Status = EmploymentStatus.Active,
            CreatedAt = now,
            EmailConfirmed = true
        };
        var salesManagerResult = await _userManager.CreateAsync(salesManager, "Orange123!");
        if (salesManagerResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(salesManager, "Manager");
            employees["SalesManager"] = salesManager;
            teams["Sales"].ManagerId = salesManager.Id;
        }
        await _context.SaveChangesAsync();
        var engEmployee1 = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Gloria Zhu",
            Email = "gloria.zhu@orange.com",
            UserName = "gloria.zhu@orange.com",
            Role = Role.Employee,
            TeamId = teams["Engineering"].Id,
            ManagerId = engManager.Id,
            Status = EmploymentStatus.Active,
            CreatedAt = now,
            EmailConfirmed = true
        };
        var engEmp1Result = await _userManager.CreateAsync(engEmployee1, "Orange123!");
        if (engEmp1Result.Succeeded)
        {
            await _userManager.AddToRoleAsync(engEmployee1, "Employee");
            employees["EngEmp1"] = engEmployee1;
        }
        var engEmployee2 = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "James Wilson",
            Email = "james.wilson@orange.com",
            UserName = "james.wilson@orange.com",
            Role = Role.Employee,
            TeamId = teams["Engineering"].Id,
            ManagerId = engManager.Id,
            Status = EmploymentStatus.Active,
            CreatedAt = now,
            EmailConfirmed = true
        };
        var engEmp2Result = await _userManager.CreateAsync(engEmployee2, "Orange123!");
        if (engEmp2Result.Succeeded)
        {
            await _userManager.AddToRoleAsync(engEmployee2, "Employee");
            employees["EngEmp2"] = engEmployee2;
        }
        var mktEmployee1 = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Emily Martinez",
            Email = "emily.martinez@orange.com",
            UserName = "emily.martinez@orange.com",
            Role = Role.Employee,
            TeamId = teams["Marketing"].Id,
            ManagerId = mktManager.Id,
            Status = EmploymentStatus.Active,
            CreatedAt = now,
            EmailConfirmed = true
        };
        var mktEmp1Result = await _userManager.CreateAsync(mktEmployee1, "Orange123!");
        if (mktEmp1Result.Succeeded)
        {
            await _userManager.AddToRoleAsync(mktEmployee1, "Employee");
            employees["MktEmp1"] = mktEmployee1;
        }
        var salesEmployee1 = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john.doe@orange.com",
            UserName = "john.doe@orange.com",
            Role = Role.Employee,
            TeamId = teams["Sales"].Id,
            ManagerId = salesManager.Id,
            Status = EmploymentStatus.Active,
            CreatedAt = now,
            EmailConfirmed = true
        };
        var salesEmp1Result = await _userManager.CreateAsync(salesEmployee1, "Orange123!");
        if (salesEmp1Result.Succeeded)
        {
            await _userManager.AddToRoleAsync(salesEmployee1, "Employee");
            employees["SalesEmp1"] = salesEmployee1;
        }
        var salesEmployee2 = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Lisa Anderson",
            Email = "lisa.anderson@orange.com",
            UserName = "lisa.anderson@orange.com",
            Role = Role.Employee,
            TeamId = teams["Sales"].Id,
            ManagerId = salesManager.Id,
            Status = EmploymentStatus.Active,
            CreatedAt = now,
            EmailConfirmed = true
        };
        var salesEmp2Result = await _userManager.CreateAsync(salesEmployee2, "Orange123!");
        if (salesEmp2Result.Succeeded)
        {
            await _userManager.AddToRoleAsync(salesEmployee2, "Employee");
            employees["SalesEmp2"] = salesEmployee2;
        }
        var inactiveEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "Robert Lee",
            Email = "robert.lee@orange.com",
            UserName = "robert.lee@orange.com",
            Role = Role.Employee,
            TeamId = teams["Engineering"].Id,
            ManagerId = engManager.Id,
            Status = EmploymentStatus.Inactive,
            CreatedAt = now.AddMonths(-6),
            UpdatedAt = now.AddMonths(-1),
            EmailConfirmed = true
        };
        var inactiveResult = await _userManager.CreateAsync(inactiveEmployee, "Orange123!");
        if (inactiveResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(inactiveEmployee, "Employee");
            employees["Inactive"] = inactiveEmployee;
        }
        await _context.SaveChangesAsync();
        return employees;
    }
    private async Task SeedLeaveRequestsAsync(Dictionary<string, Employee> employees)
    {
        var now = DateTime.UtcNow;
        var leaveRequests = new List<LeaveRequest>();
        if (employees.ContainsKey("EngEmp1"))
        {
            leaveRequests.Add(new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = employees["EngEmp1"].Id,
                StartDate = now.AddDays(10),
                EndDate = now.AddDays(15),
                Reason = "Family vacation",
                Status = RequestStatus.Pending,
                SubmittedAt = now.AddDays(-2)
            });
        }
        if (employees.ContainsKey("EngEmp2") && employees.ContainsKey("EngManager"))
        {
            leaveRequests.Add(new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = employees["EngEmp2"].Id,
                StartDate = now.AddDays(20),
                EndDate = now.AddDays(25),
                Reason = "Medical appointment",
                Status = RequestStatus.Approved,
                SubmittedAt = now.AddDays(-5),
                ReviewedByManagerId = employees["EngManager"].Id,
                ManagerReviewedAt = now.AddDays(-3)
            });
        }
        if (employees.ContainsKey("MktEmp1") && employees.ContainsKey("MktManager") && employees.ContainsKey("HR"))
        {
            leaveRequests.Add(new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = employees["MktEmp1"].Id,
                StartDate = now.AddDays(30),
                EndDate = now.AddDays(35),
                Reason = "Wedding anniversary trip",
                Status = RequestStatus.Approved,
                SubmittedAt = now.AddDays(-10),
                ReviewedByManagerId = employees["MktManager"].Id,
                ManagerReviewedAt = now.AddDays(-8),
                ApprovedByHRId = employees["HR"].Id,
                HRApprovedAt = now.AddDays(-7)
            });
        }
        if (employees.ContainsKey("SalesEmp1") && employees.ContainsKey("SalesManager"))
        {
            leaveRequests.Add(new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = employees["SalesEmp1"].Id,
                StartDate = now.AddDays(5),
                EndDate = now.AddDays(7),
                Reason = "Personal reasons",
                Status = RequestStatus.Rejected,
                RejectionReason = "Insufficient notice period and critical project deadline",
                SubmittedAt = now.AddDays(-1),
                ReviewedByManagerId = employees["SalesManager"].Id,
                ManagerReviewedAt = now
            });
        }
        if (employees.ContainsKey("MktEmp1"))
        {
            leaveRequests.Add(new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = employees["MktEmp1"].Id,
                StartDate = now.AddDays(45),
                EndDate = now.AddDays(50),
                Reason = "Conference attendance",
                Status = RequestStatus.Pending,
                SubmittedAt = now.AddDays(-1)
            });
        }
        if (employees.ContainsKey("EngEmp1") && employees.ContainsKey("EngManager") && employees.ContainsKey("HR"))
        {
            leaveRequests.Add(new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = employees["EngEmp1"].Id,
                StartDate = now.AddDays(-20),
                EndDate = now.AddDays(-15),
                Reason = "Summer vacation",
                Status = RequestStatus.Approved,
                SubmittedAt = now.AddDays(-30),
                ReviewedByManagerId = employees["EngManager"].Id,
                ManagerReviewedAt = now.AddDays(-28),
                ApprovedByHRId = employees["HR"].Id,
                HRApprovedAt = now.AddDays(-27)
            });
        }
        _context.LeaveRequests.AddRange(leaveRequests);
        await _context.SaveChangesAsync();
    }
}
