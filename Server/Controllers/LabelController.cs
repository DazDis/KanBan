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
    public class LabelController : ControllerBase
    {
        private readonly LabelRepository _labelRepository;
        private readonly IHubContext<TaskHub> _hubContext;

        public LabelController(LabelRepository labelRepository, IHubContext<TaskHub> hubContext)
        {
            _labelRepository = labelRepository;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetLabels(CancellationToken token)
        {
            var labels = await _labelRepository.GetLabelsAsync(token);
            return Ok(labels);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLabel(LabelDTO label, CancellationToken token)
        {
            await _labelRepository.AddLabelAsync(label, token);
            await _hubContext.Clients.All.SendAsync("LabelCreated", label, token);
            return CreatedAtAction(nameof(GetLabels), new { id = label.Id }, label);
            
        }
        [HttpPut]
        public async Task<IActionResult> UpdateLabel(LabelDTO label, CancellationToken token)
        {
            await _labelRepository.UpdateLabelAsync(label);
            await _hubContext.Clients.All.SendAsync("LabelUpdated", label);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLabel(int id, CancellationToken token)
        {
            await _labelRepository.DeleteLabelAsync(id);
            await _hubContext.Clients.All.SendAsync("LabelDeleted", id);
            return NoContent();
        }
    }
}
