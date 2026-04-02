using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaClient.Services;

public class TaskService : ITaskService
{
    private readonly IApiClient _apiClient;

    public TaskService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<TaskModel>> GetTasksAsync()
    {
        return await _apiClient.GetAsync<List<TaskModel>>("api/task") ?? new List<TaskModel>();
    }

    public async Task<TaskModel?> CreateTaskAsync(TaskModel task)
    {
        return await _apiClient.PostAsync<TaskModel>("api/task", task);
    }

    public async Task DeleteTaskAsync(int id)
    {
        await _apiClient.DeleteAsync($"api/task/{id}");
    }

    public async Task UpdateTaskAsync(TaskModel task)
    {
        await _apiClient.PutAsync<TaskModel>("api/task", task);
    }
}