using System;
using System.Threading.Tasks;

namespace NetcoreSaas.Infrastructure.Middleware.Tenancy.Strategy
{
    public interface ITenantResolutionStrategy
    {
        Task<Guid> GetTenantIdentifierAsync();
        Guid GetTenantIdentifier();
    }
}
