using System.Net.Http.Json;
using System.Text.Json;
using CentralizedLogging.Sdk.Abstractions;

namespace CentralizedLogging.Sdk
{
    internal sealed class CentralizedLoggingClient : ICentralizedLoggingClient
    {
        private readonly HttpClient _http;

        public CentralizedLoggingClient(HttpClient http) => _http = http;

        //public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        //{
        //    var resp = await _http.PostAsJsonAsync("api/users/authenticate", request, ct);
        //    var contentType = resp.Content.Headers.ContentType?.MediaType ?? "";

        //    var body = await resp.Content.ReadAsStringAsync(ct);

        //    if (!resp.IsSuccessStatusCode)
        //    {
        //        // Try to parse ProblemDetails for a better message
        //        ProblemDetails? prob = null;
        //        if (contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
        //        {
        //            try { prob = JsonSerializer.Deserialize<ProblemDetails>(body); } catch { /* ignore */ }
        //        }
        //        var msg = prob?.Detail ?? prob?.Title ?? $"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}";
        //        throw new HttpRequestException(msg);
        //    }

        //    // Success → parse AuthResponse
        //    if (!contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
        //        throw new InvalidOperationException($"Expected JSON but got '{contentType}'. Body: {body[..Math.Min(120, body.Length)]}");

        //    var auth = JsonSerializer.Deserialize<AuthResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //    return auth;
        //}
    }
}
