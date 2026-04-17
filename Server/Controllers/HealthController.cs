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
        public async Task<IActionResult> GetHealth()
        {
            return Ok(new
            {
                status = "ok",
                message = "Сервер работает"
            });
        }

        [HttpPost]
        public async Task<IActionResult> DropDB()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            return Ok(new
            {
                status = "ok",
                message = "Сервер работает"
            });
        }


    }
}
