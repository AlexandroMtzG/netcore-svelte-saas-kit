// using Microsoft.AspNetCore.Http;
//
// namespace NetcoreSaas.Infrastructure.Middleware.Tenancy
// {
//     public interface ITenantAccessor
//     {
//         // Tenant Tenant { get; }
//     }
//     public class TenantAccessor : ITenantAccessor
//     {
//         private readonly IHttpContextAccessor _httpContextAccessor;
//
//         public TenantAccessor(IHttpContextAccessor httpContextAccessor)
//         {
//             _httpContextAccessor = httpContextAccessor;
//         }
//
//         // public Tenant Tenant => _httpContextAccessor.HttpContext.GetTenant();
//     }
// }