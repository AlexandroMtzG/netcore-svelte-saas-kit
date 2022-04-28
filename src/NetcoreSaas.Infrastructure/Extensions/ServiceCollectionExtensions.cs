using NetcoreSaas.Infrastructure.Middleware.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace NetcoreSaas.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static TenantBuilder AddMultiTenancy(this IServiceCollection services)
            => new TenantBuilder(services);
    }
}
