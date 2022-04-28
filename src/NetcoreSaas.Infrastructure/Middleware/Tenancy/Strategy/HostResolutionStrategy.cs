// using System;
// using Microsoft.AspNetCore.Http;
// using System.Threading.Tasks;
//
// namespace NetcoreSaas.Infrastructure.Middleware.Tenancy.Strategy
// {
//     /// <summary>
//     /// Resolve the host to a tenant identifier
//     /// </summary>
//     public class HostResolutionStrategy : ITenantResolutionStrategy
//     {
//         private readonly IHttpContextAccessor _httpContextAccessor;
//
//         public HostResolutionStrategy(IHttpContextAccessor httpContextAccessor)
//         {
//             _httpContextAccessor = httpContextAccessor;
//         }
//
//         public async Task<Guid> GetTenantIdentifierAsync()
//         {
//             return await Task.FromResult(_httpContextAccessor.HttpContext.Request.Host.Host);
//         }
//         public Guid GetTenantIdentifier()
//         {
//             if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request != null)
//                 return _httpContextAccessor.HttpContext.Request.Host.Host;
//             return "";
//         }
//     }
// }
