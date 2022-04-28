// using Microsoft.AspNetCore.Http;
// using System.Threading.Tasks;
//
// namespace NetcoreSaas.Infrastructure.Middleware.Tenancy.Strategy
// {
//     /// <summary>
//     /// Resolve the request URL to a tenant identifier
//     /// </summary>
//     public class UrlResolutionStrategy : ITenantResolutionStrategy
//     {
//         private readonly IHttpContextAccessor _httpContextAccessor;
//
//         public UrlResolutionStrategy(IHttpContextAccessor httpContextAccessor)
//         {
//             _httpContextAccessor = httpContextAccessor;
//         }
//
//         public async Task<string> GetTenantIdentifierAsync()
//         {
//             return await Task.FromResult(_httpContextAccessor.HttpContext.Request.Host.Host);
//         }
//
//         public string GetTenantIdentifier()
//         {
//
//             if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request != null)
//                 return _httpContextAccessor.HttpContext.Request.Host.Host;
//             return "";
//         }
//     }
// }
