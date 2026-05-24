using AvaloniaClient.DataBase;
using AvaloniaClient.Models;
using System;
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
        var created =  await _apiClient.PostAsync<TaskModel>("api/task", task);
        if (created != null)
        {
            await AddHistoryEntryAsync(
                created.Id,
                "Создание",
                "",
                $"Задача \"{created.Title}\" создана",
                $"Название: {created.Title}, Описание: {created.Description}"
            );
        }
        return created;
    }

    public async Task DeleteTaskAsync(int id, CancellationToken token = default)
    {
        await _apiClient.DeleteAsync($"api/task/{id}");
    }

    public async Task UpdateTaskAsync(TaskModel task, CancellationToken token = default)
    {
        await _apiClient.PutAsync("api/task", task);
    }
    public async Task<List<TaskHistoryEntry>> GetTaskHistoryAsync(int taskId, CancellationToken token = default)
    {
        return await _apiClient.GetAsync<List<TaskHistoryEntry>>($"api/task/{taskId}/history", token)
               ?? new List<TaskHistoryEntry>();
    }

    public async Task AddHistoryEntryAsync(int taskId, string actionType, string oldValue, string newValue, string comment = null, CancellationToken token = default)
    {
        var entry = new
        {
            TaskId = taskId,
            ActionType = actionType,
            OldValue = oldValue,
            NewValue = newValue,
            ChangedAt = DateTime.UtcNow,
            Comment = comment
        };

        await _apiClient.PostAsync<object>($"api/task/{taskId}/history", entry, token);
    }
}