using System;
using NetcoreSaas.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;

namespace NetcoreSaas.Infrastructure.Middleware.Tenancy.Store
{
    /// <summary>
    /// In memory store for testing
    /// </summary>
    public class ClaimsTenantStore : ITenantStore
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public ClaimsTenantStore(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        // public async Task<Tenant> GetTenantAsync(Guid uuid)
        // {
        //     var type = _contextAccessor.HttpContext.GetTenantUserRole();
        // }
        
        // public Tenant GetTenant(Guid uuid)
        // {
        //     return _contextAccessor.HttpContext.GetTenant();
        // }

        public UserType GetUserType()
        {
            return _contextAccessor.HttpContext.GetUserType();
        }
        
        public TenantUserRole GetTenantUserRole()
        {
            return _contextAccessor.HttpContext.GetTenantUserRole();
        }

        public Guid GetTenantUserId()
        {
            return _contextAccessor.HttpContext.GetTenantUserId();
        }

        public Guid GetUserId()
        {
            return _contextAccessor.HttpContext.GetUserId();
        }
    }
}
