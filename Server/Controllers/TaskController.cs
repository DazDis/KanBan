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
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _taskRepository.GetTasksAsync();
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskDTO task)
        {
            await _taskRepository.AddTaskAsync(task);
            await _hubContext.Clients.All.SendAsync("TaskCreated", task);
            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateTask(TaskDTO task)
        {
            await _taskRepository.UpdateTaskAsync(task);
            await _hubContext.Clients.All.SendAsync("TaskUpdated", task);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _taskRepository.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}
