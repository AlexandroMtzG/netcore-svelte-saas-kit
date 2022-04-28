using System;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;

namespace NetcoreSaas.Infrastructure.Middleware.Tenancy
{
    public interface ITenantAccessService
    {
        // ITenantResolutionStrategy _tenantResolutionStrategy;
        // ITenantStore _tenantStore;
        // IHttpContextAccessor _httpContextAccessor;
        // MasterDbContext _db;

        // Task<Tenant> GetTenantAsync();
        // Tenant GetTenant();
        UserType GetUserType();
        TenantUserRole GetTenantUserRole();
        Guid GetUserId();
        Guid GetTenantUserId();
    }
}
