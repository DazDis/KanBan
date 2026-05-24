using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface ITeamService
    {
        Task<List<TeamModel>> GetTeamsAsync(CancellationToken token = default);
        Task<TeamModel?> CreateTeamAsync(TeamModel dto, CancellationToken token = default);
        Task UpdateTeamAsync(TeamModel dto, CancellationToken token = default);
        Task DeleteTeamAsync(int id, CancellationToken token = default);
    }
}