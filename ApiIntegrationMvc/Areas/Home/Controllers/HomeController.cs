using ApiIntegrationMvc.Areas.Account.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }



       

    }
}
