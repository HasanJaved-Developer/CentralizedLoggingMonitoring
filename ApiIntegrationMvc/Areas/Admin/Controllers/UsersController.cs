using Microsoft.AspNetCore.Mvc;

namespace ApiIntegrationMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "Home" });
            //return View();
        }
    }
}
