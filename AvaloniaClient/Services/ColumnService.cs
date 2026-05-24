using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
namespace AvaloniaClient.Services;

public class ColumnService : IColumnService
{
    private readonly IApiClient _apiClient;

    public ColumnService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<ColumnModel>> GetColumnsAsync(CancellationToken token = default)
    {
        var DTOs = await _apiClient.GetAsync<List<ColumnDTO>>("api/column") ?? new List<ColumnDTO>();
        List<ColumnModel> columns = new();
        foreach (var dto in DTOs) {
            columns.Add(new ColumnModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Tasks = new ObservableCollection<TaskModel>(),
                Position = dto.Position,
            });
        }
        return columns;
    }

    public async Task<ColumnDTO?> CreateColumnAsync(ColumnDTO dto, CancellationToken token = default)
    {
        return await _apiClient.PostAsync<ColumnDTO>("api/column", dto);
    }

    public async Task DeleteColumnAsync(int id, CancellationToken token = default)
    {
        await _apiClient.DeleteAsync($"api/task/{id}");
    }

    public async Task UpdateColumnAsync(ColumnModel dto, CancellationToken token = default)
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