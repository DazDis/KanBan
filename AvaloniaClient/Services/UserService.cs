using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace AvaloniaClient.Services;

public class UserService : IUserService
{
    private readonly IApiClient _apiClient;

    public UserService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<UserModel>> GetUsersAsync(CancellationToken token = default)
    {
        return await _apiClient.GetAsync<List<UserModel>>("api/user") ?? new List<UserModel>();
    }

    public async Task<UserModel?> CreateUserAsync(UserModel dto, CancellationToken token = default)
    {
        return await _apiClient.PostAsync<UserModel>("api/user", dto);
    }
    public async Task UpdateUserAsync(UserModel dto, CancellationToken token = default)
    {
        await _apiClient.PutAsync("api/user", dto);
    }
    public async Task DeleteUserAsync(int id, CancellationToken token = default)
    {
        await _apiClient.DeleteAsync($"api/user/{id}");
    }
}