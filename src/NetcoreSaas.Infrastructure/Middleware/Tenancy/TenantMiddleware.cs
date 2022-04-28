using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace NetcoreSaas.Infrastructure.Middleware.Tenancy
{
    internal class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Items.ContainsKey("X-Tenant-Key"))
            {
                if (context.RequestServices.GetService(typeof(ITenantAccessService)) is TenantAccessService tenantService)
                {
                    var tenant = tenantService.GetTenantUuid();
                    if (tenant != Guid.Empty)
                        context.Items.Add("X-Tenant-Key", tenant);
                }
            }

            //Continue processing
            if (_next != null)
                await _next(context);
        }
    }
}
