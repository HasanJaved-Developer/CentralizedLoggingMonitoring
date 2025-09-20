using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Contracts.Auth;

namespace UserManagement.Sdk.Abstractions
{
    public interface IUserManagementClient
    {
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    }
}
