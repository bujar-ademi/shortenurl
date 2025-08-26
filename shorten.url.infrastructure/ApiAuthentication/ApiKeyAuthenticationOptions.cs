using Microsoft.AspNetCore.Authentication;

namespace shorten.url.infrastructure.ApiAuthentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "ClientKey";
        public const string HeaderName = "x-api-key";
    }
}
