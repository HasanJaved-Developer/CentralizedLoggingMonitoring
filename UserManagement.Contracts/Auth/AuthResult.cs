using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Contracts.Auth
{
    public sealed record AuthResult(
    string AccessToken,
    int ExpiresInSeconds,
    string? TokenType = "Bearer");
}
