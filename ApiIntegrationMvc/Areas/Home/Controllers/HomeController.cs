using ApiIntegrationMvc.Areas.Account.Models;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using UserManagement.Contracts.Auth;
using UserManagement.Sdk.Abstractions;
using UserManagementApi.Contracts.Models;
using static System.Net.WebRequestMethods;

namespace ApiIntegrationMvc.Areas.Home.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
        private readonly IAccessTokenProvider _tokens;


        public HomeController(IAccessTokenProvider tokens) => _tokens = tokens;
       

        public async Task<IActionResult> Index(CancellationToken ct)
        {            
            var token = await _tokens.GetAccessTokenAsync(ct);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            IEnumerable<Claim> claims = jwt.Claims;
            var list = claims.Where(c => c.Type == "categories").Select(c => c.Value).ToList();
            if (list.Count == 1)
            {
                var categories = JsonSerializer.Deserialize<List<Category>>(list[0]);
                ViewBag.Categories = categories;
            }
            else
            {
                ViewBag.Categories = new List<Category>();
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }



    }
}
