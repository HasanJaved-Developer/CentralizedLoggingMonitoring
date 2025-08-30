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
        private readonly ILogger<ApplicationsController> _logger;
        
        private readonly LoggingDbContext _context;

        public ApplicationsController(LoggingDbContext context, ILogger<ApplicationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Application app)
        {
            _context.Applications.Add(app);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = app.Id }, app);
        }

        /// <summary>
        /// Fetches an application by its unique Id.
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves application details from the database.  
        /// 
        /// **Logging:**  
        /// - On every call, Serilog writes an entry to the log file (e.g., `dev-app-.clef`).  
        /// - If an error occurs (e.g., record not found or DB exception), the error is also recorded in the same log file.  
        /// 
        /// **Usage:**  
        /// `GET /api/applications/{id}`  
        /// 
        /// Note:  
        /// The logging setup ensures that each request is auditable and errors can be traced via structured log files.
        /// </remarks>
        /// <param name="id">Unique application Id.</param>
        /// <returns>Returns the application details if found, otherwise NotFound (404).</returns>

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {            

            var app = await _context.Applications.FindAsync(id);
            if (app == null)
            {
                _logger.LogWarning("Application not found. Id={Id}", id);
                return NotFound();
            }

            return Ok(app);
        }

        /// <summary>
        /// Test endpoint that always throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This endpoint exists only for verifying the application's global exception
        /// handling middleware and Serilog logging.
        /// </para>
        /// <para>
        /// When called, it will throw an unhandled exception. The request should be
        /// intercepted by <c>ExceptionHandlingMiddleware</c>, which will log the error
        /// and return a consistent JSON error response.
        /// </para>
        /// </remarks>
        /// <response code="500">Always returned, because the endpoint throws an exception.</response>
        [HttpGet("boom")]
        public Task<IActionResult> Boom()
        {
            throw new InvalidOperationException("Boom!");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var apps = await _context.Applications.ToListAsync();
            return Ok(apps);
        }
    }
}
