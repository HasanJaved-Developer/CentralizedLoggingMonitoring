using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using UserManagement.Sdk.Abstractions;

namespace UserManagement.Sdk
{
    public sealed class MemoryCacheAccessTokenProvider : IAccessTokenProvider
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _http;        

        public MemoryCacheAccessTokenProvider(IMemoryCache cache, IHttpContextAccessor http)
        {
            _cache = cache;
            _http = http;
        }

        public Task<string?> GetAccessTokenAsync(CancellationToken ct = default)
        {
            var user = _http.HttpContext?.User;
            var uid =
                user?.FindFirst("sub")?.Value
             ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(uid)) return Task.FromResult<string?>(null);

            var key = $"token:{uid}";
            _cache.TryGetValue(key, out string? token);
            return Task.FromResult(token);
        }

        // Optional helper method to set the token into cache
        public void SetAccessToken(string token, int userId, DateTime expiresAtUtc)
        {            
            var ttl = ToTtl(expiresAtUtc);            
            _cache.Set($"token:{userId}", token, ttl);
        }

        public static TimeSpan ToTtl(DateTime expiresAtUtc, TimeSpan? safety = null)
        {
            // ensure it's treated as UTC
            if (expiresAtUtc.Kind != DateTimeKind.Utc)
                expiresAtUtc = DateTime.SpecifyKind(expiresAtUtc, DateTimeKind.Utc);

            var ttl = expiresAtUtc - DateTime.UtcNow;

            // subtract a small safety margin to avoid edge expiries
            ttl -= safety ?? TimeSpan.FromSeconds(15);

            return ttl > TimeSpan.Zero ? ttl : TimeSpan.Zero;
        }
    }
}
