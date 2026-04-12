using Microsoft.AspNetCore.Mvc;
using Server.Database.Repositories;
using Server.DataBase;
using Server.DTOs;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        public HealthController()
        {
        }
        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
            return Ok(new
            {
                status = "ok",
                message = "Сервер работает"
            });
        }

    }
}
