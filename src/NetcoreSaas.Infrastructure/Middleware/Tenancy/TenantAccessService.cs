using Microsoft.AspNetCore.Http;
using System;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;
using NetcoreSaas.Infrastructure.Extensions;

namespace NetcoreSaas.Infrastructure.Middleware.Tenancy
{
    public class TenantAccessService : ITenantAccessService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantAccessService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserType GetUserType()
        {
            return _httpContextAccessor.HttpContext.GetUserType();
        }

        public TenantUserRole GetTenantUserRole()
        {
            return _httpContextAccessor.HttpContext.GetTenantUserRole();
        }

        public Guid GetUserId()
        {
            return _httpContextAccessor.HttpContext.GetUserId();
        }

        public Guid GetTenantId()
        {
            return _httpContextAccessor.HttpContext.GetTenantId();
        }

        public Guid GetTenantUuid()
        {
            return _httpContextAccessor.HttpContext.GetTenantUuid();
        }

        public Guid GetTenantUserId()
        {
            return _httpContextAccessor.HttpContext.GetTenantUserId();
        }
    }
}
