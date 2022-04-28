using Microsoft.AspNetCore.Builder;

namespace NetcoreSaas.Infrastructure.Middleware.Tenancy
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMultiTenancy(this IApplicationBuilder builder)
            => builder.UseMiddleware<TenantMiddleware>();
    }
}
