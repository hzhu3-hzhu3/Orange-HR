using HRManagement.Domain.Entities;
using HRManagement.Domain.Enums;
using HRManagement.Domain.Interfaces;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace HRManagement.Infrastructure.Repositories;
public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly HRManagementDbContext _context;
    public LeaveRequestRepository(HRManagementDbContext context)
    {
        _context = context;
    }
    public async Task<LeaveRequest?> GetByIdAsync(Guid id)
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .FirstOrDefaultAsync(lr => lr.Id == id);
    }
    public async Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId)
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .Where(lr => lr.EmployeeId == employeeId)
            .OrderByDescending(lr => lr.SubmittedAt)
            .ToListAsync();
    }
    public async Task<IEnumerable<LeaveRequest>> GetPendingForManagerAsync(Guid managerId)
    {
        var directReportIds = await _context.Employees
            .Where(e => e.ManagerId == managerId)
            .Select(e => e.Id)
            .ToListAsync();
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .Where(lr => directReportIds.Contains(lr.EmployeeId) && lr.Status == RequestStatus.Pending)
            .OrderBy(lr => lr.SubmittedAt)
            .ToListAsync();
    }
    public async Task<IEnumerable<LeaveRequest>> GetPendingForHRAsync()
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .Where(lr => lr.Status == RequestStatus.Approved && lr.ApprovedByHRId == null)
            .OrderBy(lr => lr.ManagerReviewedAt)
            .ToListAsync();
    }
    public async Task<LeaveRequest> CreateAsync(LeaveRequest request)
    {
        _context.LeaveRequests.Add(request);
        await _context.SaveChangesAsync();
        return request;
    }
    public async Task UpdateAsync(LeaveRequest request)
    {
        _context.LeaveRequests.Update(request);
        await _context.SaveChangesAsync();
    }
}
