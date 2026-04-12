using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface ITeamService
    {
        Task<List<TeamModel>> GetTeamsAsync();
        Task<TeamModel?> CreateTeamAsync(TeamModel dto);
        Task DeleteTeamAsync(int id);
    }
}