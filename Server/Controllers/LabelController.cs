using Microsoft.AspNetCore.Mvc;
using Server.Database.Repositories;
using Server.DTOs;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LabelController : ControllerBase
    {
        private readonly LabelRepository _labelRepository;

        public LabelController(LabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetLabels()
        {
            var labels = await _labelRepository.GetLabelsAsync();
            return Ok(labels);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLabel(LabelDTO label)
        {
            await _labelRepository.AddLabelAsync(label);
            return CreatedAtAction(nameof(GetLabels), new { id = label.Id }, label);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLabel(int id)
        {
            await _labelRepository.DeleteLabelAsync(id);
            return NoContent();
        }
    }
}
