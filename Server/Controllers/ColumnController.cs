using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Database.Repositories;
using Server.DataBase;
using Server.DTOs;
using Server.Hubs;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColumnController : ControllerBase
    {
        private readonly ColumnRepository _columnRepository;
        private readonly IHubContext<TaskHub> _hubContext;

        public ColumnController(ColumnRepository labelRepository, IHubContext<TaskHub> hubContext)
        {
            _columnRepository = labelRepository;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetColumns(CancellationToken token)
        {
            var labels = await _columnRepository.GetColumnsAsync(token);
            return Ok(labels);
        }

        [HttpPost]
        public async Task<IActionResult> CreateColumn(ColumnDTO column, CancellationToken token)
        {
            await _columnRepository.AddColumnAsync(column, token);
            await _hubContext.Clients.All.SendAsync("ColumnCreated", column, token);
            return CreatedAtAction(nameof(GetColumns), new { id = column.Id }, column);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateColumn(ColumnDTO column, CancellationToken token)
        {
            await _columnRepository.UpdateColumnAsync(column, token);
            await _hubContext.Clients.All.SendAsync("ColumnUpdated", column, token);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumn(int id, CancellationToken token)
        {
            await _columnRepository.DeleteColumnAsync(id, token);
            await _hubContext.Clients.All.SendAsync("ColumnDeleted", id, token);
            return NoContent();
        }
    }
}
