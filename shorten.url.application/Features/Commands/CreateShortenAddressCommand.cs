using Mediator;
using shorten.url.application.Contracts;
using shorten.url.application.Models;

namespace shorten.url.application.Features.Commands
{
    public class CreateShortenAddressCommand : IRequest<ShortAddressModel>
    {
        public string Address { get; set; }
    }

    public class CreateShortenAddressCommandHandler : IRequestHandler<CreateShortenAddressCommand, ShortAddressModel>
    {
        private readonly IUrlService urlService;
        private readonly ICurrentUserService currentUserService;

        public CreateShortenAddressCommandHandler(IUrlService urlService, ICurrentUserService currentUserService)
        {
            this.urlService = urlService;
            this.currentUserService = currentUserService;
        }
        public async ValueTask<ShortAddressModel> Handle(CreateShortenAddressCommand request, CancellationToken cancellationToken)
        {
            var apiClientId = currentUserService.ApiClientId;
            if (!apiClientId.HasValue)
            {
                throw new Exception("API_CLIENT_ID");
            }
            var item = await urlService.CreateAsync(request, apiClientId.Value).ConfigureAwait(false);

            return item;
        }
    }
}
