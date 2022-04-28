using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetcoreSaas.Infrastructure.Middleware.Tenancy.Strategy;
using NetcoreSaas.Infrastructure.Middleware.Tenancy.Store;

namespace NetcoreSaas.Infrastructure.Middleware.Tenancy
{
    /// <summary>
    /// Configure tenant services
    /// </summary>
    public class TenantBuilder
    {
        private readonly IServiceCollection _services;

        public TenantBuilder(IServiceCollection services)
        {
            services.AddTransient<ITenantAccessService, TenantAccessService>();
            _services = services;
        }

        /// <summary>
        /// Register the tenant resolver implementation
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder WithResolutionStrategy<TV>(ServiceLifetime lifetime = ServiceLifetime.Transient) where TV : class, ITenantResolutionStrategy
        {
            _services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            _services.Add(ServiceDescriptor.Describe(typeof(ITenantResolutionStrategy), typeof(TV), lifetime));
            return this;
        }

        /// <summary>
        /// Register the tenant store implementation
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder WithStore<TV>(ServiceLifetime lifetime = ServiceLifetime.Transient) where TV : class, ITenantStore
        {
            _services.Add(ServiceDescriptor.Describe(typeof(ITenantStore), typeof(TV), lifetime));
            return this;
        }
    }
}
