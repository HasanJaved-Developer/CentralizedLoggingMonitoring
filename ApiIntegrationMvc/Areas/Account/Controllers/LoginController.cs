using ApiIntegrationMvc.Areas.Account.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiIntegrationMvc.Areas.Account.Controllers
{
    [Area("Account")]
    public class LoginController : Controller
    {
        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Index()
        {
            ViewBag.Error = TempData["Error"];     // one-time error
            return View(new LoginViewModel());     // empty fields
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
                        
            if(model.Username == "admin" && model.Password == "123")
                return RedirectToAction("Index", "Home");     // PRG on success too            
            
            TempData["Error"] = "Invalid username or password.";
            return RedirectToAction(nameof(Index));   // ← PRG on failure
            
        }
    }
}
