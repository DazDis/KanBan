using Microsoft.AspNetCore.Mvc;
using Server.Database.Repositories;
using Server.DataBase;
using Server.DTOs;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly TeamRepository _teamRepository;

        public TeamController(TeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            var team = await _teamRepository.GetTeamsAsync();
            return Ok(team);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam(TeamDTO team)
        {
            await _teamRepository.AddTeamAsync(team);
            return CreatedAtAction(nameof(GetTeams), new { id = team.Id }, team);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            await _teamRepository.DeleteTeamAsync(id);
            return NoContent();
        }
    }
}
