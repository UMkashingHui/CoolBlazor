using Blazored.LocalStorage;
using CoolBlazor.Infrastructure.Constants.Storage;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace CoolBlazor.Infrastructure.Managers.Identity.Authentication
{
    public class AuthenticationHeaderHandler : DelegatingHandler
    {
        protected readonly ILocalStorageService _localStorage;

        public AuthenticationHeaderHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // request.Version = HttpVersion.Version10;

            if (request.Headers.Authorization?.Scheme != "Bearer")
            {
                try
                {
                    // var tokenStorage = await this.localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
                    var savedToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);

                    if (!string.IsNullOrWhiteSpace(savedToken))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
                    }
                }
                // catch (JSDisconnectedException ex)
                // {
                //     // Ignore
                // }
                catch (InvalidOperationException)
                {
                }
            }
            return await base.SendAsync(request, cancellationToken);
        }



    }
}