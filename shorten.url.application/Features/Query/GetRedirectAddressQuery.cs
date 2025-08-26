using Mediator;
using Microsoft.Extensions.Options;
using shorten.url.application.Contracts;
using shorten.url.application.Models;

namespace shorten.url.application.Features.Query
{
    public class GetRedirectAddressQuery : IRequest<string>
    {
        public string UniqueId { get; set; }
    }

    public class GetRedirectAddressQueryHandler : IRequestHandler<GetRedirectAddressQuery, string>
    {
        private readonly IUrlService urlService;
        private readonly Domain domain;
        public GetRedirectAddressQueryHandler(IUrlService urlService, IOptions<Domain> domains)
        {
            this.urlService = urlService;
            this.domain = domains.Value;
        }
        public async ValueTask<string> Handle(GetRedirectAddressQuery request, CancellationToken cancellationToken)
        {
            var redirectUrl = await urlService.GetRedirectAddressAsync(request.UniqueId).ConfigureAwait(false);

            if (string.IsNullOrEmpty(redirectUrl))
            {
                // need to redirect to some of our page informing that url does not exists?
                redirectUrl = $"{domain.RandomDomain}/not-found/{request.UniqueId}";
            }
            return redirectUrl;
        }
    }
}
