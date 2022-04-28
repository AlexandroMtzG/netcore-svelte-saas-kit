using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace NetcoreSaas.Infrastructure.Middleware.Tenancy.Strategy
{
    /// <summary>
    /// Resolve the header to a tenant identifier
    /// </summary>
    public class HeaderResolutionStrategy : ITenantResolutionStrategy {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HeaderResolutionStrategy(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Guid> GetTenantIdentifierAsync()
        {
            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Tenant-Key"))
            {
                try
                {
                    return Guid.Parse(
                        await Task.FromResult(_httpContextAccessor.HttpContext.Request.Headers["X-Tenant-Key"]));
                }
                catch
                {
                    // ignore
                }
            }
            return Guid.Empty;
        }
        public Guid GetTenantIdentifier()
        {
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request != null && _httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Tenant-Key"))
            {
                try
                {
                    return Guid.Parse(_httpContextAccessor.HttpContext.Request.Headers["X-Tenant-Key"]);
                }
                catch
                {
                    // ignore
                }
            }

            return Guid.Empty;
        }
    }
}
