using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace AvaloniaClient.Services;

public class TeamService : ITeamService
{
    private readonly IApiClient _apiClient;

    public TeamService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<TeamModel>> GetTeamsAsync()
    {
        return await _apiClient.GetAsync<List<TeamModel>>("api/team") ?? new List<TeamModel>();
    }

    public async Task<TeamModel?> CreateTeamAsync(TeamModel dto)
    {
        return await _apiClient.PostAsync<TeamModel>("api/team", dto);
    }

    public async Task DeleteTeamAsync(int id)
    {
        await _apiClient.DeleteAsync($"api/team/{id}");
    }
}