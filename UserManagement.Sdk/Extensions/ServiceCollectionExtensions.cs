using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using UserManagement.Sdk.Abstractions;
using UserManagement.Sdk.Auth;
using UserManagement.Sdk.Configuration;


namespace UserManagement.Sdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUserManagementSdk(
            this IServiceCollection services,
            Action<UserManagementOptions>? configure = null)
        {
            services.AddOptions<UserManagementOptions>()
                    .Configure<IConfiguration>((opt, config) =>
                    {
                        // optional: bind from config "UserManagement"
                        config.GetSection("UserManagement").Bind(opt);
                    });

            if (configure is not null)
                services.PostConfigure(configure);

            // Delegating handler MUST be transient
            services.AddTransient<BearerTokenHandler>();

            // Register the token provider (memory-based)
            services.AddScoped<IAccessTokenProvider, MemoryCacheAccessTokenProvider>();


            // The Typed client
            services.AddHttpClient<IUserManagementClient, UserManagementClient>((sp, http) =>
            {
                var opts = sp.GetRequiredService<IOptions<UserManagementOptions>>().Value;
                if (opts.BaseAddress is null)
                    throw new InvalidOperationException("UserManagementOptions.BaseAddress must be set.");

                http.BaseAddress = opts.BaseAddress;
                http.Timeout = opts.Timeout;
            })
            .AddHttpMessageHandler<BearerTokenHandler>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                // If you need custom certs, proxies, cookies, etc.
                UseCookies = false
            })
            .AddPolicyHandler((sp, req) =>
            {
                var opts = sp.GetRequiredService<IOptions<UserManagementOptions>>().Value;
                return opts.EnableResiliencePolicies
                    ? HttpPolicies.GetRetryPolicy()
                    : Polly.Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();
            })
            .AddPolicyHandler((sp, req) =>
            {
                var opts = sp.GetRequiredService<IOptions<UserManagementOptions>>().Value;
                return opts.EnableResiliencePolicies
                    ? HttpPolicies.GetCircuitBreakerPolicy()
                    : Polly.Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();
            });

            return services;
        }
    }
}
