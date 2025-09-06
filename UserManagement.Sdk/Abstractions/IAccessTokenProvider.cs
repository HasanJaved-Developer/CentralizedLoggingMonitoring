using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Sdk.Abstractions
{
    public interface IAccessTokenProvider
    {
        Task<string?> GetAccessTokenAsync(CancellationToken ct = default);
        void SetAccessToken(string token, int userId, DateTime expiresAtUtc);
    }
}
