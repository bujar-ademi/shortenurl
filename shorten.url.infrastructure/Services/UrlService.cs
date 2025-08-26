using AutoMapper;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using shorten.url.application;
using shorten.url.application.Contracts;
using shorten.url.application.Features.Commands;
using shorten.url.application.Features.Notifications;
using shorten.url.application.Models;
using shorten.url.domain;

namespace shorten.url.infrastructure.Services
{
    public class UrlService : IUrlService
    {
        private readonly IRepository repository;
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly Domain domain;

        public UrlService(IRepository repository, IMapper mapper, IMediator mediator, IOptions<Domain> domains)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.mediator = mediator;
            this.domain = domains.Value;
        }

        public async Task<ShortAddressModel> CreateAsync(CreateShortenAddressCommand request, Guid apiClientId)
        {
            var uniqueId = Randomizer.GenerateUniqueUrl();
            var exists = await repository.ListQueryable<ShortAddress>(x => x.UniqueId.Equals(uniqueId)).AsNoTracking().FirstOrDefaultAsync();
            if (exists != null)
            {
                return await CreateAsync(request, apiClientId);
            }

            var domainName = domain.RandomDomain;

            var entity = new ShortAddress
            {
                Domain = domainName,
                UniqueId = uniqueId,
                ShortUrl = $"{domainName}/{uniqueId}",
                RedirectUrl = request.Address,
                Hits = 0,
                ApiClientId = apiClientId
            };
            await repository.AddAsync(entity);
            await repository.SaveChangesAsync();

            return mapper.Map<ShortAddressModel>(entity);
        }

        public async Task<string> GetRedirectAddressAsync(string uniqueId)
        {
            var entity = await repository.ListQueryable<ShortAddress>(x => x.UniqueId == uniqueId)
                .AsNoTracking()
                .Select(x => new
                {
                    Id = x.Id,
                    RedirectUrl = x.RedirectUrl
                }).FirstOrDefaultAsync();

            if (entity == null)
            {
                return $"{domain.RandomDomain}/not-found/{uniqueId}";
            }

            // publish notification
            await mediator.Publish(new AddressClicked { AddressId = entity.Id }).ConfigureAwait(false);

            return entity.RedirectUrl;
        }
    }
}
