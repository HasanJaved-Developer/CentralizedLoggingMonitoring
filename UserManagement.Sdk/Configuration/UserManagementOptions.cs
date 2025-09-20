using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Sdk.Configuration
{
    public sealed class UserManagementOptions
    {
        /// <summary>Base URL of the User Management API, e.g., https://auth.myorg.local/</summary>
        public Uri? BaseAddress { get; set; }

        /// <summary>HttpClient timeout (default 30s).</summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>Enable Polly resilience policies.</summary>
        public bool EnableResiliencePolicies { get; set; } = true;
    }
}
