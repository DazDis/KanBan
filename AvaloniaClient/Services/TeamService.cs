using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace AvaloniaClient.Services;

public class TeamService : ITeamService
{
    private readonly IApiClient _apiClient;

    public TeamService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<TeamModel>> GetTeamsAsync(CancellationToken token = default)
    {
        return await _apiClient.GetAsync<List<TeamModel>>("api/team") ?? new List<TeamModel>();
    }

    public async Task<TeamModel?> CreateTeamAsync(TeamModel dto, CancellationToken token = default)
    {
        return await _apiClient.PostAsync<TeamModel>("api/team", dto);
    }
    public async Task UpdateTeamAsync(TeamModel dto, CancellationToken token = default)
    {
        await _apiClient.PutAsync("api/team", dto);
    }

    public async Task DeleteTeamAsync(int id, CancellationToken token = default)
    {
        await _apiClient.DeleteAsync($"api/team/{id}");
    }
}