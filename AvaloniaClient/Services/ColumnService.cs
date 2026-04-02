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
}