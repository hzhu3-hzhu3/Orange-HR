using HRManagement.Application.DTOs;
using HRManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace HRManagement.Web.Controllers;
[ApiController]
[Route("api/employees")]
[Authorize] // Requirement 1.1: All endpoints require authentication
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }
    [HttpGet]
    [Authorize(Policy = "HROnly")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllEmployees()
    {
        var requestingUserId = GetCurrentUserId();
        var employees = await _employeeService.GetAllEmployeesAsync(requestingUserId);
        return Ok(employees);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployeeById(Guid id)
    {
        var requestingUserId = GetCurrentUserId();
        var employee = await _employeeService.GetEmployeeByIdAsync(id, requestingUserId);
        if (employee == null)
        {
            return NotFound(new { Message = "Employee not found" });
        }
        return Ok(employee);
    }
    [HttpGet("team")]
    [Authorize(Policy = "ManagerOrHR")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetTeam()
    {
        var managerId = GetCurrentUserId();
        var directReports = await _employeeService.GetDirectReportsAsync(managerId);
        return Ok(directReports);
    }
    [HttpPost]
    [Authorize(Policy = "HROnly")]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        var createdBy = GetCurrentUserId();
        var employee = await _employeeService.CreateEmployeeAsync(request, createdBy);
        return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
    }
    [HttpPut("{id}")]
    [Authorize(Policy = "HROnly")]
    public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeRequest request)
    {
        var updatedBy = GetCurrentUserId();
        await _employeeService.UpdateEmployeeAsync(id, request, updatedBy);
        return NoContent();
    }
    [HttpDelete("{id}")]
    [Authorize(Policy = "HROnly")]
    public async Task<IActionResult> DeactivateEmployee(Guid id)
    {
        var deactivatedBy = GetCurrentUserId();
        await _employeeService.DeactivateEmployeeAsync(id, deactivatedBy);
        return NoContent();
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
