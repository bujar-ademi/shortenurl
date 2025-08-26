using AutoMapper;
using shorten.url.application.Models;
using shorten.url.domain;

namespace shorten.url.application
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ShortAddress, ShortAddressModel>();
        }
    }
}
