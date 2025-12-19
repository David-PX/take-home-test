using AutoMapper;
using Fundo.Applications.WebApi.Application.Mapping;

namespace Fundo.Services.Tests.Unit.Mapping
{
    public static class AutoMapperFixture
    {
        public static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        }
    }
}
