namespace CentralizedLogging.Sdk.Abstractions
{
    public interface IAccessTokenProvider
    {
        Task<string?> GetAccessTokenAsync(CancellationToken ct = default);
        void SetAccessToken(string token, int userId, DateTime expiresAtUtc);
        public Task RemoveAsync(string userId, CancellationToken ct = default);
    }
}
