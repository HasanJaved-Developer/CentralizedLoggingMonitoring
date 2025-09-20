using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using UserManagement.Sdk.Abstractions;
using UserManagementApi.Contracts.Models;

namespace ApiIntegrationMvc.Views.Shared.Components
{

    public sealed class CategoryTreeViewComponent: ViewComponent
    {
        private readonly IAccessTokenProvider _tokens;
        public CategoryTreeViewComponent(IAccessTokenProvider tokens)
        => _tokens = tokens;
    
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var ct = HttpContext?.RequestAborted ?? default;
            var token = await _tokens.GetAccessTokenAsync(ct);
            
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            IEnumerable<Claim> claims = jwt.Claims;
            var list = claims.Where(c => c.Type == "categories").Select(c => c.Value).ToList();
            IReadOnlyList<Category> categories = new List<Category>();
            if (list.Count == 1)
            {
                categories = JsonSerializer.Deserialize<List<Category>>(list[0]);
            }

            return View(categories); // Views/Shared/Components/CategoryTree/Default.cshtml
        }
    }
}
