using ApiIntegrationMvc.Areas.Account.Models;
using CentralizedLogging.Contracts.DTO;
using CentralizedLogging.Sdk.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using UserManagement.Contracts.Auth;
using UserManagement.Sdk.Abstractions;

namespace ApiIntegrationMvc.Areas.Account.Controllers
{
    [Area("Account")]
    public class LoginController : Controller
    {
        private readonly IUserManagementClient _users;
        private readonly UserManagement.Sdk.Abstractions.IAccessTokenProvider _cache;
        private readonly IHttpContextAccessor _http;
        private readonly ICentralizedLoggingClient _centralizedlogs;
        public LoginController(IUserManagementClient users, ICentralizedLoggingClient centralizedlogs,
            UserManagement.Sdk.Abstractions.IAccessTokenProvider cache, IHttpContextAccessor http) => (_users, _centralizedlogs, _cache, _http) = (users, centralizedlogs, cache, http);

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


                // Build claims (at least a stable user id + name)
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                    new(ClaimTypes.Name, result.UserName),
                  
                };

                var identity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Sign-in (creates auth cookie)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                        // keep cookie a bit shorter than token
                        ExpiresUtc = result.ExpiresAtUtc.AddMinutes(-5)
                    });


                _cache.SetAccessToken(result.Token, result.UserId, result.ExpiresAtUtc);

                return RedirectToAction("Index", "Home", new { area = "Home" });
            }
            catch (HttpRequestException hx)
            {   
                await _centralizedlogs.LogErrorAsync(BuildDto(hx, requestPath: "GET api/users"), ct);
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

        private CreateErrorLogDto BuildDto(HttpRequestException ex, string requestPath)
        {
            //=>
            CreateErrorLogDto obj = new CreateErrorLogDto
           {
               ApplicationId = 4,
               Severity = "Error",
               Message = ex.Message,
               StackTrace = ex.ToString(),
               Source = ex.Source,
               RequestId = Guid.NewGuid().ToString(), // or pass one down            
           };
            return obj;
        }

        private static string? Claim(ClaimsPrincipal? u, string t) => u?.FindFirst(t)?.Value;

        private string? CurrentUserId() =>
            Claim(_http.HttpContext?.User, "sub")
         ?? Claim(_http.HttpContext?.User, ClaimTypes.NameIdentifier);


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var uid = CurrentUserId();
            if (string.IsNullOrEmpty(uid))
            {
                return RedirectToAction("Index", "Login", new { area = "Account" });
            }
            // IMPORTANT: remove tokens while the user is still authenticated
            await _cache.RemoveAsync(uid, ct);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login", new { area = "Account" });
        }


    }
}
