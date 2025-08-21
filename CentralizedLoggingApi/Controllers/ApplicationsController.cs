using CentralizedLoggingApi.Data;
using CentralizedLoggingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CentralizedLoggingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly LoggingDbContext _context;

        public ApplicationsController(LoggingDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Application app)
        {
            _context.Applications.Add(app);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = app.Id }, app);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var app = await _context.Applications.FindAsync(id);
            if (app == null) return NotFound();
            return Ok(app);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var apps = await _context.Applications.ToListAsync();
            return Ok(apps);
        }
    }
}
