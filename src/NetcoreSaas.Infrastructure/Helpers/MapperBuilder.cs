using AutoMapper;
using NetcoreSaas.Application.Mapper;

namespace NetcoreSaas.Infrastructure.Helpers
{
    public class MapperBuilder
    {
        public static IMapper Build()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            return new Mapper(configuration);
        }
    }
}
