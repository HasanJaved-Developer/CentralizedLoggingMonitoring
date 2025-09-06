using System.Net.Http.Json;
using UserManagement.Contracts.Auth;
using UserManagement.Sdk.Abstractions;

namespace UserManagement.Sdk
{
    internal sealed class UserManagementClient : IUserManagementClient
    {
        private readonly HttpClient _http;

        public UserManagementClient(HttpClient http) => _http = http;

        public async Task<AuthResult?> LoginAsync(LoginRequest request, CancellationToken ct = default)
            => await _http.PostAsJsonAsync("api/users/authenticate", request, ct)
                          .Result.Content.ReadFromJsonAsync<AuthResult>(cancellationToken: ct);        
    }
}
