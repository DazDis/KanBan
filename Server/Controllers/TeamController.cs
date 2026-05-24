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
        public async Task<IActionResult> GetTeams(CancellationToken token)
        {
            var team = await _teamRepository.GetTeamsAsync(token);
            return Ok(team);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam(TeamDTO team, CancellationToken token)
        {
            await _teamRepository.AddTeamAsync(team, token);
            await _hubContext.Clients.All.SendAsync("TeamCreated", team, token);
            return CreatedAtAction(nameof(GetTeams), new { id = team.Id }, team);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateTeam(TeamDTO team, CancellationToken token)
        {
            await _teamRepository.UpdateTeamAsync(team, token);
            await _hubContext.Clients.All.SendAsync("TeamUpdated", team, token);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id, CancellationToken token)
        {
            await _teamRepository.DeleteTeamAsync(id, token);
            await _hubContext.Clients.All.SendAsync("TeamDeleted", id, token);
            return NoContent();
        }
    }
}
