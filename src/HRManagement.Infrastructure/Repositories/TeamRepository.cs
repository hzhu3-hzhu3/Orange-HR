using HRManagement.Domain.Entities;
using HRManagement.Domain.Interfaces;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace HRManagement.Infrastructure.Repositories;
public class TeamRepository : ITeamRepository
{
    private readonly HRManagementDbContext _context;
    public TeamRepository(HRManagementDbContext context)
    {
        _context = context;
    }
    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await _context.Teams
            .Include(t => t.Manager)
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
    public async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _context.Teams
            .Include(t => t.Manager)
            .Include(t => t.Members)
            .ToListAsync();
    }
    public async Task<Team> CreateAsync(Team team)
    {
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();
        return team;
    }
    public async Task UpdateAsync(Team team)
    {
        _context.Teams.Update(team);
        await _context.SaveChangesAsync();
    }
}
