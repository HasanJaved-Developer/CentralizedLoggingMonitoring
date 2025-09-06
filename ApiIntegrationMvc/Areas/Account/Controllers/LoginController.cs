using ApiIntegrationMvc.Areas.Account.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UserManagement.Contracts.Auth;
using UserManagement.Sdk.Abstractions;

namespace ApiIntegrationMvc.Areas.Account.Controllers
{
    [Area("Account")]
    public class LoginController : Controller
    {
        private readonly IUserManagementClient _users;
        private readonly IAccessTokenProvider _cache;
        public LoginController(IUserManagementClient users, IAccessTokenProvider cache) => (_users, _cache) = (users, cache);

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
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var req = new LoginRequest(model.Username, model.Password);
                var result = await _users.LoginAsync(req, ct);

                if (result == null || string.IsNullOrWhiteSpace(result.Token))
                {
                    TempData["Error"] = "Invalid username or password.";
                    return RedirectToAction(nameof(Index));   // ← PRG on failure
                }

                
                _cache.SetAccessToken(result.Token, result.UserId, result.ExpiresAtUtc);

                return RedirectToAction("Index", "Home", new { area = "Home" });
            }
            catch (HttpRequestException hx)
            {
                TempData["Error"] = hx.Message;
                return RedirectToAction(nameof(Index));   // ← PRG on failure             
            }
            catch (JsonException jx)
            {                
                TempData["Error"] = jx.Message;
                return RedirectToAction(nameof(Index));   // ← PRG on failure             
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Internal Error. Please contact administrator.";
                return RedirectToAction(nameof(Index));   // ← PRG on failure             
            }
        }

        
    }
}
