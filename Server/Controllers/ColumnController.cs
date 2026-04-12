using Microsoft.AspNetCore.Mvc;
using Server.Database.Repositories;
using Server.DataBase;
using Server.DTOs;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColumnController : ControllerBase
    {
        private readonly ColumnRepository _columnRepository;

        public ColumnController(ColumnRepository labelRepository)
        {
            _columnRepository = labelRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetColumns()
        {
            var labels = await _columnRepository.GetColumnsAsync();
            return Ok(labels);
        }

        [HttpPost]
        public async Task<IActionResult> CreateColumn(ColumnDTO label)
        {
            await _columnRepository.AddColumnAsync(label);
            return CreatedAtAction(nameof(GetColumns), new { id = label.Id }, label);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumn(int id)
        {
            await _columnRepository.DeleteColumnAsync(id);
            return NoContent();
        }
    }
}
