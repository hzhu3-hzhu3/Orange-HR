using HRManagement.Domain.Entities;
namespace HRManagement.Domain.Interfaces;
public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id);
    Task<IEnumerable<Team>> GetAllAsync();
    Task<Team> CreateAsync(Team team);
    Task UpdateAsync(Team team);
}
