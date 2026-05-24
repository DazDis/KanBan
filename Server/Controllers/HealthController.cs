using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Database.Repositories;
using Server.DataBase;
using Server.DTOs;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;  
        public HealthController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetHealth(CancellationToken token)
        {
            return Ok(new
            {
                status = "ok",
                message = "Сервер работает"
            });
        }

        [HttpPost]
        public async Task<IActionResult> DropDB(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                if (await _context.Database.CanConnectAsync(token))
                {
                    await _context.Database.EnsureDeletedAsync(token);
                }

                await _context.Database.EnsureCreatedAsync(token);

                return Ok(new
                {
                    status = "ok",
                    message = "База данных пересоздана"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}
