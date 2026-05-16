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
    public class TeamController : ControllerBase
    {
        private readonly TeamRepository _teamRepository;
        private readonly IHubContext<TaskHub> _hubContext;

        public TeamController(TeamRepository teamRepository, IHubContext<TaskHub> hubContext)
        {
            _teamRepository = teamRepository;
            _hubContext = hubContext;
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
            await _hubContext.Clients.All.SendAsync("TeamCreated", team);
            return CreatedAtAction(nameof(GetTeams), new { id = team.Id }, team);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateTeam(TeamDTO team)
        {
            await _teamRepository.UpdateTeamAsync(team);
            await _hubContext.Clients.All.SendAsync("TeamUpdated", team);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            await _teamRepository.DeleteTeamAsync(id);
            await _hubContext.Clients.All.SendAsync("TeamDeleted", id);
            return NoContent();
        }
    }
}
