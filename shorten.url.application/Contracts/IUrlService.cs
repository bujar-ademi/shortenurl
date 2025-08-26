using shorten.url.application.Features.Commands;
using shorten.url.application.Models;

namespace shorten.url.application.Contracts
{
    public interface IUrlService
    {
        Task<ShortAddressModel> CreateAsync(CreateShortenAddressCommand request, Guid apiClientId);
        Task<string> GetRedirectAddressAsync(string uniqueId);
    }
}
