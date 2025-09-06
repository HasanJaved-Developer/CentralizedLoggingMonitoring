using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using CentralizedLogging.Sdk.Abstractions;
using CentralizedLogging.Sdk.Configuration;


namespace CentralizedLogging.Sdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUserManagementSdk(
            this IServiceCollection services,
            Action<CentralizedLoggingOptions>? configure = null)
        {
            services.AddOptions<CentralizedLoggingOptions>()
                    .Configure<IConfiguration>((opt, config) =>
                    {
                        // optional: bind from config "CentralizedLogging"
                        config.GetSection("CentralizedLogging").Bind(opt);
                    });

            if (configure is not null)
                services.PostConfigure(configure);

           
            // The Typed client
            services.AddHttpClient<ICentralizedLoggingClient, CentralizedLoggingClient>((sp, http) =>
            {
                var opts = sp.GetRequiredService<IOptions<CentralizedLoggingOptions>>().Value;
                if (opts.BaseAddress is null)
                    throw new InvalidOperationException("CentralizedLoggingOptions.BaseAddress must be set.");

                http.BaseAddress = opts.BaseAddress;
                http.Timeout = opts.Timeout;
            })            
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                // If you need custom certs, proxies, cookies, etc.
                UseCookies = false
            })
            .AddPolicyHandler((sp, req) =>
            {
                var opts = sp.GetRequiredService<IOptions<CentralizedLoggingOptions>>().Value;                
                return opts.EnableResiliencePolicies
                    ? HttpPolicies.GetRetryPolicy()
                    : Polly.Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();
            })
            .AddPolicyHandler((sp, req) =>
            {
                var opts = sp.GetRequiredService<IOptions<CentralizedLoggingOptions>>().Value;
                return opts.EnableResiliencePolicies
                    ? HttpPolicies.GetCircuitBreakerPolicy()
                    : Polly.Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();
            });

            return services;
        }
    }
}
