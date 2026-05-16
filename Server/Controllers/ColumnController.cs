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
        public async Task<IActionResult> GetColumns()
        {
            var labels = await _columnRepository.GetColumnsAsync();
            return Ok(labels);
        }

        [HttpPost]
        public async Task<IActionResult> CreateColumn(ColumnDTO column)
        {
            await _columnRepository.AddColumnAsync(column);
            await _hubContext.Clients.All.SendAsync("ColumnCreated", column);
            return CreatedAtAction(nameof(GetColumns), new { id = column.Id }, column);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateColumn(ColumnDTO column)
        {
            await _columnRepository.UpdateColumnAsync(column);
            await _hubContext.Clients.All.SendAsync("ColumnUpdated", column);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumn(int id)
        {
            await _columnRepository.DeleteColumnAsync(id);
            return NoContent();
        }
    }
}
