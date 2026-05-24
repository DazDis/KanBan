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
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly IHubContext<TaskHub> _hubContext;

        public UserController(UserRepository userRepository, IHubContext<TaskHub> hubContext)
        {
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken token)
        {
            var users = await _userRepository.GetUsersAsync(token);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserDTO user, CancellationToken token)
        {
            await _userRepository.AddUserAsync(user, token);
            await _hubContext.Clients.All.SendAsync("UserCreated", user, token);
            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserDTO user, CancellationToken token)
        {
            await _userRepository.UpdateUserAsync(user, token);
            await _hubContext.Clients.All.SendAsync("UserUpdated", user, token);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken token)
        {
            await _userRepository.DeleteUserAsync(id, token);
            await _hubContext.Clients.All.SendAsync("UserDeleted", id, token);
            return NoContent();
        }
    }
}
