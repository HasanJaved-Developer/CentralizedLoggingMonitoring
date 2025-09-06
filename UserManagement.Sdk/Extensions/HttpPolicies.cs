using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Sdk.Extensions
{
    internal static class HttpPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
            => HttpPolicyExtensions
                .HandleTransientHttpError() // 5xx + 408 + network failures
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromMilliseconds(500),
                    TimeSpan.FromSeconds(1)
                });

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
            => HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 5, durationOfBreak: TimeSpan.FromSeconds(30));
    }
}
