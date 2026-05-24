using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Database.Repositories;
using Server.DTOs;
using Server.Hubs;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskRepository _taskRepository;
        private readonly IHubContext<TaskHub> _hubContext;  
        public TaskController(TaskRepository taskRepository, IHubContext<TaskHub> hubContext)
        {
            _taskRepository = taskRepository;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks(CancellationToken token)
        {
            var tasks = await _taskRepository.GetTasksAsync(token);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskDTO task, CancellationToken token)
        {
            await _taskRepository.AddTaskAsync(task, token);
            await _hubContext.Clients.All.SendAsync("TaskCreated", task, token);
            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateTask(TaskDTO task, CancellationToken token)
        {
            await _taskRepository.UpdateTaskAsync(task, token);
            await _hubContext.Clients.All.SendAsync("TaskUpdated", task, token);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id, CancellationToken token)
        {
            await _taskRepository.DeleteTaskAsync(id, token);
            await _hubContext.Clients.All.SendAsync("TaskDeleted", id, token);
            return NoContent();
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetTaskHistory(int id, CancellationToken token)
        {
            var history = await _taskRepository.GetTaskHistoryAsync(id, token);
            return Ok(history);
        }

        [HttpPost("{id}/history")]
        public async Task<IActionResult> AddHistoryEntry(int id, [FromBody] TaskHistoryEntryDto entry, CancellationToken token)
        {
            if (id != entry.TaskId)
                return BadRequest("Task ID mismatch");

            await _taskRepository.AddHistoryEntryAsync(entry, token);
            var history = await _taskRepository.GetTaskHistoryAsync(id, token);

            return Ok(history);
        }
    }
}
