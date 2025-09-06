using ApiIntegrationMvc.Areas.Account.Models;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Contracts.Auth;
using UserManagement.Sdk.Abstractions;

namespace ApiIntegrationMvc.Areas.Account.Controllers
{
    [Area("Account")]
    public class LoginController : Controller
    {
        private readonly IUserManagementClient _users;
        public LoginController(IUserManagementClient users) => _users = users;

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Index()
        {
            ViewBag.Error = TempData["Error"];     // one-time error
            return View(new LoginViewModel());     // empty fields
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var req = new LoginRequest(model.Username, model.Password);
            var result = await _users.LoginAsync(req, ct);

            if (result == null || string.IsNullOrWhiteSpace(result.AccessToken))
            {
                TempData["Error"] = "Invalid username or password.";
                return RedirectToAction(nameof(Index));   // ← PRG on failure
            }
            
            return RedirectToAction("Index", "Home");     // PRG on success too                                   
            
        }
    }
}
