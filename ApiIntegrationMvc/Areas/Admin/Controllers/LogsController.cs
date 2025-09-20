using CentralizedLogging.Sdk.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ApiIntegrationMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LogsController : Controller
    {
        private readonly ICentralizedLoggingClient _centralizedlogs;
        private readonly IAccessTokenProvider _cache;
        private readonly IHttpContextAccessor _http;
        public LogsController(ICentralizedLoggingClient centralizedlogs, IAccessTokenProvider cache, IHttpContextAccessor http) => (_centralizedlogs, _cache, _http) = (centralizedlogs, cache, http);

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            string token = await _cache.GetAccessTokenAsync(ct);

            var result = await _centralizedlogs.GetAllErrorAsync(ct);

  
            return View(result.OrderByDescending(v => v.Id));
        }
    }
}
