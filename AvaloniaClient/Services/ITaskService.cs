using AvaloniaClient.DataBase;
using AvaloniaClient.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface ITaskService
    {
        Task<List<TaskModel>> GetTasksAsync(CancellationToken token = default);
        Task<TaskModel?> CreateTaskAsync(TaskModel task, CancellationToken token = default);
        Task UpdateTaskAsync(TaskModel task, CancellationToken token = default);
        Task DeleteTaskAsync(int id, CancellationToken token = default);
        Task<List<TaskHistoryEntry>> GetTaskHistoryAsync(int taskId, CancellationToken token = default);
        Task AddHistoryEntryAsync(int taskId, string actionType, string oldValue, string newValue, string comment = null, CancellationToken token = default);
    }
}