using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace AvaloniaClient.Services;

public class ColumnService : IColumnService
{
    private readonly IApiClient _apiClient;

    public ColumnService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<ColumnDTO>> GetColumnsAsync()
    {
        return await _apiClient.GetAsync<List<ColumnDTO>>("api/column") ?? new List<ColumnDTO>();
    }

    public async Task<ColumnDTO?> CreateColumnAsync(ColumnDTO dto)
    {
        return await _apiClient.PostAsync<ColumnDTO>("api/column", dto);
    }

    public async Task DeleteColumnAsync(int id)
    {
        await _apiClient.DeleteAsync($"api/task/{id}");
    }

    public async Task UpdateColumnAsync(ColumnModel dto)
    {
        ColumnDTO column = new ColumnDTO()
        {
            Id = dto.Id,
            Title = dto.Title,
            Position = dto.Position,
        };
        await _apiClient.PutAsync("api/column", column);
    }
}