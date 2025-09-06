using Microsoft.Extensions.Caching.Memory;
using UserManagement.Sdk.Abstractions;

namespace UserManagement.Sdk
{
    public sealed class MemoryCacheAccessTokenProvider : IAccessTokenProvider
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheAccessTokenProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<string?> GetAccessTokenAsync(CancellationToken ct = default)
        {
            // Try to fetch from cache
            if (_cache.TryGetValue("AccessToken", out string? token))
            {
                return Task.FromResult(token);
            }

            return Task.FromResult<string?>(null);
        }

        // Optional helper method to set the token into cache
        public void SetAccessToken(string token, TimeSpan expiresIn)
        {
            _cache.Set("AccessToken", token, expiresIn);
        }
    }
}
