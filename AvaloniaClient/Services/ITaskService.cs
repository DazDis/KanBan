using AvaloniaClient.DataBase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaClient.Services
{
    public interface ITaskService
    {
        Task<List<TaskModel>> GetTasksAsync();
        Task<TaskModel?> CreateTaskAsync(TaskModel task);
        Task UpdateTaskAsync(TaskModel task);
        Task DeleteTaskAsync(int id);
    }
}