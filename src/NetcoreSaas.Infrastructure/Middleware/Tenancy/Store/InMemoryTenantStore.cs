// using NetcoreSaas.Domain.Models.Master.Tenants;
// using System;
// using System.Linq;
// using System.Threading.Tasks;
//
// namespace NetcoreSaas.Infrastructure.Middleware.Tenancy.Store
// {
//     /// <summary>
//     /// In memory store for testing
//     /// </summary>
//     public class InMemoryTenantStore : ITenantStore
//     {
//         private static readonly Tenant[] Tenants = new[]{
//                 //new Tenant { Id = Guid.NewGuid().ToString(), Host = "localhost", Nombre = "localhost", Database = "tenant_localhost" },
//                 new Tenant { Id = Guid.Empty, Subdomain = "tenant1", Name = "tenant1", Uuid = Guid.NewGuid() },
//                 new Tenant { Id = Guid.Empty, Subdomain = "tenant1", Name = "tenant2", Uuid = Guid.NewGuid()},
//             };
//         /// <summary>
//         /// Get a tenant for a given identifier
//         /// </summary>
//         /// <param name="identifier"></param>
//         /// <returns></returns>
//         public async Task<Tenant> GetTenantAsync(Guid uuid)
//         {
//             var tenant = Tenants.SingleOrDefault(t => t.Uuid == uuid);
//
//             return await Task.FromResult(tenant);
//         }
//         /// <summary>
//         /// Get a tenant for a given identifier
//         /// </summary>
//         /// <param name="identifier"></param>
//         /// <returns></returns>
//         public Tenant GetTenant(Guid uuid)
//         {
//             var tenant = Tenants.SingleOrDefault(t => t.Uuid == uuid);
//             return tenant;
//         }
//     }
// }
