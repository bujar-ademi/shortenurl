using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using shorten.url.application.Contracts;
using shorten.url.domain;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace shorten.url.infrastructure.ApiAuthentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IRepository repository;

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ICacheProvider cacheProvider, IRepository repository) : base(options, logger, encoder)
        {
            this.cacheProvider = cacheProvider;
            this.repository = repository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var apiKey) || apiKey.Count != 1)
            {
                Logger.LogWarning("An API request was received without the x-api-key header");                
                return AuthenticateResult.Fail("Invalid parameters");
            }

            var client = await GetClientFromApiKey(apiKey);

            if (client == null)
            {
                Logger.LogWarning($"An API request was received with an invalid API key: {apiKey}");
                return AuthenticateResult.Fail("Invalid parameters");
            }

            Logger.BeginScope("{ClientId}", client.ClientName);
            Logger.LogInformation("Client authenticated");

            var claims = new[] { new Claim(ClaimTypes.Name, client.ClientName), new Claim(ClaimTypes.Sid, client.Id.ToString()) };

            var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationOptions.DefaultScheme);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.DefaultScheme);

            return AuthenticateResult.Success(ticket);
        }

        private async Task<ApiClient> GetClientFromApiKey(string apikey)
        {
            if (cacheProvider.TryRetrieve<ApiClient>($"CLIENT_{apikey}", out ApiClient cached))
            {
                return cached;
            }
            var apiClient = await repository.ListGenericQueryable<ApiClient>(x => x.ApiKey.Equals(apikey)).AsNoTracking().FirstOrDefaultAsync();
            if (apiClient == null)
            {
                return null;
            }
            // add to cache
            await cacheProvider.StoreAsync<ApiClient>($"CLIENT_{apikey}", apiClient);

            return apiClient;
        }
    }
}
