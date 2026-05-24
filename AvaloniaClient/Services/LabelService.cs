using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace AvaloniaClient.Services;

public class LabelService : ILabelService
{
    private readonly IApiClient _apiClient;

    public LabelService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<LabelModel>> GetLabelsAsync(CancellationToken token = default)
    {
        return await _apiClient.GetAsync<List<LabelModel>>("api/label") ?? new List<LabelModel>();
    }

    public async Task<LabelModel?> CreateLabelAsync(LabelModel dto, CancellationToken token = default)
    {
        return await _apiClient.PostAsync<LabelModel>("api/label", dto);
    }
    public async Task UpdateLabelAsync(LabelModel dto, CancellationToken token = default)
    {
        await _apiClient.PutAsync("api/label", dto);
    }

    public async Task DeleteLabelAsync(int id, CancellationToken token = default)
    {
        await _apiClient.DeleteAsync($"api/label/{id}");
    }
}