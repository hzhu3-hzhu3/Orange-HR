using HRManagement.Domain.Entities;
using HRManagement.Domain.Interfaces;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace HRManagement.Infrastructure.Repositories;
public class EmployeeRepository : IEmployeeRepository
{
    private readonly HRManagementDbContext _context;
    public EmployeeRepository(HRManagementDbContext context)
    {
        _context = context;
    }
    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _context.Employees
            .Include(e => e.Team)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _context.Employees
            .Include(e => e.Team)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.Email == email);
    }
    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .Include(e => e.Team)
            .Include(e => e.Manager)
            .ToListAsync();
    }
    public async Task<IEnumerable<Employee>> GetDirectReportsAsync(Guid managerId)
    {
        return await _context.Employees
            .Include(e => e.Team)
            .Include(e => e.Manager)
            .Where(e => e.ManagerId == managerId)
            .ToListAsync();
    }
    public async Task<Employee> CreateAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }
    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id);
    }
}
