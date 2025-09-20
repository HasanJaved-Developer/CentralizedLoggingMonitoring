using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Sdk.Abstractions;

namespace UserManagement.Sdk.Auth
{
    // IMPORTANT: register this as Transient in DI.
    // It asks the IAccessTokenProvider for the current token each request.
    internal sealed class BearerTokenHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider _tokenProvider;

        public BearerTokenHandler(IAccessTokenProvider tokenProvider)
            => _tokenProvider = tokenProvider;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
