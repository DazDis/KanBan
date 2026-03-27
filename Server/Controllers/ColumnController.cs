using Microsoft.AspNetCore.Mvc;
using Server.DataBase;
using Server.DTOs;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColumnController : ControllerBase
    {
        private readonly LabelRepository _labelRepository;

        public ColumnController(LabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetColumns()
        {
            var labels = await _labelRepository.GetColumnsAsync();
            return Ok(labels);
        }

        [HttpPost]
        public async Task<IActionResult> CreateColumn(ColumnDTO label)
        {
            await _labelRepository.AddColumnAsync(label);
            return CreatedAtAction(nameof(GetColumns), new { id = label.Id }, label);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLabel(int id)
        {
            await _labelRepository.DeleteLabelAsync(id);
            return NoContent();
        }
    }
}
