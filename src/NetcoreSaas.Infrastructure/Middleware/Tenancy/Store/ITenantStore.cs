using System;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;

namespace NetcoreSaas.Infrastructure.Middleware.Tenancy.Store
{
    public interface ITenantStore
    {
        // Task<Tenant> GetTenantAsync(Guid uuid);
        // Tenant GetTenant(Guid uuid);
        UserType GetUserType();
        TenantUserRole GetTenantUserRole();
        Guid GetTenantUserId();
        Guid GetUserId();
    }
}
