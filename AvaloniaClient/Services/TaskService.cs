using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaClient.Services;

public class TaskService : ITaskService
{
    private readonly IApiClient _apiClient;

    public TaskService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<TaskModel>> GetTasksAsync(CancellationToken token = default)
    {
        return await _apiClient.GetAsync<List<TaskModel>>("api/task") ?? new List<TaskModel>();
    }

    public async Task<TaskModel?> CreateTaskAsync(TaskModel task, CancellationToken token = default)
    {
        return await _apiClient.PostAsync<TaskModel>("api/task", task);
    }

    public async Task DeleteTaskAsync(int id, CancellationToken token = default)
    {
        await _apiClient.DeleteAsync($"api/task/{id}");
    }

    public async Task UpdateTaskAsync(TaskModel task, CancellationToken token = default)
    {
        await _apiClient.PutAsync("api/task", task);
    }
}