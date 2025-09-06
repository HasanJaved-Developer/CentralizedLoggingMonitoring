using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Contracts.Auth
{    
    public record AuthResponse(
      int UserId,
      string UserName,
      string Token,
      DateTime ExpiresAtUtc 
  );
}
