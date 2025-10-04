using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using UserManagement.Contracts.Auth;

namespace UserManagementApi.Tests.Integration
{
    public class LoginFlowTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public LoginFlowTests(WebApplicationFactory<Program> factory)
            => _client = factory.CreateClient();

        [Fact]
        public async Task Login_ValidCredentials_Returns200AndToken()
        {
            var payload = new LoginRequest("bob","bob");
            var resp = await _client.PostAsJsonAsync("api/users/authenticate", payload);

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await resp.Content.ReadFromJsonAsync<AuthResponse>();
            body.Should().NotBeNull();
            body.Token.Should().NotBeNullOrEmpty();
        }
    }
}
