using Microsoft.AspNetCore.Mvc;

namespace ApiIntegrationMvc.Areas.Ops.Controllers
{
    [Area("Ops")]
    public class LibraryController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "Home" });            
        }
    }
}
