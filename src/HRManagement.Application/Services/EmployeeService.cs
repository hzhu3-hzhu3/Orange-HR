using HRManagement.Application.DTOs;
using HRManagement.Application.Exceptions;
using HRManagement.Application.Interfaces;
using HRManagement.Domain.Entities;
using HRManagement.Domain.Enums;
using HRManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
namespace HRManagement.Application.Services;
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAuditService _auditService;
    private readonly UserManager<Employee> _userManager;
    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IAuditService auditService,
        UserManager<Employee> userManager)
    {
        _employeeRepository = employeeRepository;
        _auditService = auditService;
        _userManager = userManager;
    }
    public async Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id, Guid requestingUserId)
    {
        var requestingUser = await _employeeRepository.GetByIdAsync(requestingUserId);
        if (requestingUser == null)
        {
            throw new UnauthorizedException("Requesting user not found");
        }
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            return null;
        }
        bool canView = requestingUserId == id || 
                       requestingUser.Role == Role.HR ||
                       (requestingUser.Role == Role.Manager && employee.ManagerId == requestingUserId);
        if (!canView)
        {
            throw new UnauthorizedException("You do not have permission to view this employee");
        }
        return MapToDto(employee);
    }
    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(Guid requestingUserId)
    {
        var requestingUser = await _employeeRepository.GetByIdAsync(requestingUserId);
        if (requestingUser == null || requestingUser.Role != Role.HR)
        {
            throw new UnauthorizedException("Only HR can view all employees");
        }
        var employees = await _employeeRepository.GetAllAsync();
        return employees.Select(MapToDto);
    }
    public async Task<IEnumerable<EmployeeDto>> GetDirectReportsAsync(Guid managerId)
    {
        var manager = await _employeeRepository.GetByIdAsync(managerId);
        if (manager == null || manager.Role != Role.Manager)
        {
            throw new UnauthorizedException("Only managers can view direct reports");
        }
        var directReports = await _employeeRepository.GetDirectReportsAsync(managerId);
        return directReports.Select(MapToDto);
    }
    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request, Guid createdBy)
    {
        var creatingUser = await _employeeRepository.GetByIdAsync(createdBy);
        if (creatingUser == null || creatingUser.Role != Role.HR)
        {
            throw new UnauthorizedException("Only HR can create employees");
        }
        if (request.ManagerId.HasValue)
        {
            var manager = await _employeeRepository.GetByIdAsync(request.ManagerId.Value);
            if (manager == null)
            {
                throw new ArgumentException("Manager not found");
            }
        }
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            UserName = request.Email,
            Role = request.Role,
            TeamId = request.TeamId,
            ManagerId = request.ManagerId,
            Status = EmploymentStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        var result = await _userManager.CreateAsync(employee, request.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create employee: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        await _auditService.LogActionAsync(
            createdBy,
            AuditAction.EmployeeCreated,
            nameof(Employee),
            employee.Id,
            $"Created employee: {employee.Name} ({employee.Email})"
        );
        return MapToDto(employee);
    }
    public async Task UpdateEmployeeAsync(Guid id, UpdateEmployeeRequest request, Guid updatedBy)
    {
        var updatingUser = await _employeeRepository.GetByIdAsync(updatedBy);
        if (updatingUser == null || updatingUser.Role != Role.HR)
        {
            throw new UnauthorizedException("Only HR can update employees");
        }
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            throw new ArgumentException("Employee not found");
        }
        var changes = new List<string>();
        if (request.Name != null && request.Name != employee.Name)
        {
            changes.Add($"Name: {employee.Name} → {request.Name}");
            employee.Name = request.Name;
        }
        if (request.Email != null && request.Email != employee.Email)
        {
            changes.Add($"Email: {employee.Email} → {request.Email}");
            employee.Email = request.Email;
            employee.UserName = request.Email;
        }
        if (request.Role.HasValue && request.Role.Value != employee.Role)
        {
            changes.Add($"Role: {employee.Role} → {request.Role.Value}");
            employee.Role = request.Role.Value;
        }
        if (request.TeamId != employee.TeamId)
        {
            changes.Add($"Team: {employee.TeamId} → {request.TeamId}");
            employee.TeamId = request.TeamId;
            await _auditService.LogActionAsync(
                updatedBy,
                AuditAction.TeamAssignmentChanged,
                nameof(Employee),
                employee.Id,
                $"Team changed from {employee.TeamId} to {request.TeamId}"
            );
        }
        if (request.ManagerId != employee.ManagerId)
        {
            if (request.ManagerId.HasValue)
            {
                await ValidateNoCircularManagerReference(id, request.ManagerId.Value);
            }
            changes.Add($"Manager: {employee.ManagerId} → {request.ManagerId}");
            employee.ManagerId = request.ManagerId;
            await _auditService.LogActionAsync(
                updatedBy,
                AuditAction.ManagerAssignmentChanged,
                nameof(Employee),
                employee.Id,
                $"Manager changed from {employee.ManagerId} to {request.ManagerId}"
            );
        }
        if (changes.Any())
        {
            employee.UpdatedAt = DateTime.UtcNow;
            await _employeeRepository.UpdateAsync(employee);
            await _auditService.LogActionAsync(
                updatedBy,
                AuditAction.EmployeeUpdated,
                nameof(Employee),
                employee.Id,
                string.Join("; ", changes)
            );
        }
    }
    public async Task DeactivateEmployeeAsync(Guid id, Guid deactivatedBy)
    {
        var deactivatingUser = await _employeeRepository.GetByIdAsync(deactivatedBy);
        if (deactivatingUser == null || deactivatingUser.Role != Role.HR)
        {
            throw new UnauthorizedException("Only HR can deactivate employees");
        }
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            throw new ArgumentException("Employee not found");
        }
        if (employee.Status == EmploymentStatus.Inactive)
        {
            return; // Already inactive
        }
        employee.Status = EmploymentStatus.Inactive;
        employee.UpdatedAt = DateTime.UtcNow;
        await _employeeRepository.UpdateAsync(employee);
        await _auditService.LogActionAsync(
            deactivatedBy,
            AuditAction.EmployeeDeactivated,
            nameof(Employee),
            employee.Id,
            $"Deactivated employee: {employee.Name} ({employee.Email})"
        );
    }
    public async Task ActivateEmployeeAsync(Guid id, Guid activatedBy)
    {
        var activatingUser = await _employeeRepository.GetByIdAsync(activatedBy);
        if (activatingUser == null || activatingUser.Role != Role.HR)
        {
            throw new UnauthorizedException("Only HR can activate employees");
        }
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            throw new ArgumentException("Employee not found");
        }
        if (employee.Status == EmploymentStatus.Active)
        {
            return; // Already active
        }
        employee.Status = EmploymentStatus.Active;
        employee.UpdatedAt = DateTime.UtcNow;
        await _employeeRepository.UpdateAsync(employee);
        await _auditService.LogActionAsync(
            activatedBy,
            AuditAction.EmployeeActivated,
            nameof(Employee),
            employee.Id,
            $"Activated employee: {employee.Name} ({employee.Email})"
        );
    }
    private async Task ValidateNoCircularManagerReference(Guid employeeId, Guid newManagerId)
    {
        if (employeeId == newManagerId)
        {
            throw new CircularManagerReferenceException("An employee cannot be their own manager");
        }
        var currentManager = await _employeeRepository.GetByIdAsync(newManagerId);
        var visited = new HashSet<Guid> { employeeId };
        while (currentManager != null && currentManager.ManagerId.HasValue)
        {
            if (visited.Contains(currentManager.ManagerId.Value))
            {
                throw new CircularManagerReferenceException("This manager assignment would create a circular reference");
            }
            if (currentManager.ManagerId.Value == employeeId)
            {
                throw new CircularManagerReferenceException("This manager assignment would create a circular reference");
            }
            visited.Add(currentManager.ManagerId.Value);
            currentManager = await _employeeRepository.GetByIdAsync(currentManager.ManagerId.Value);
        }
    }
    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto(
            employee.Id,
            employee.Name,
            employee.Email ?? string.Empty,
            employee.Role,
            employee.TeamId,
            employee.Team?.Name,
            employee.ManagerId,
            employee.Manager?.Name,
            employee.Status
        );
    }
}
