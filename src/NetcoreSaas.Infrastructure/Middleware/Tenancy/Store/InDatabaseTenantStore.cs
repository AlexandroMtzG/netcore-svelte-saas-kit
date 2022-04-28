// using System;
// using NetcoreSaas.Domain.Models.Master.Tenants;
// using NetcoreSaas.Infrastructure.Data;
// using System.Linq;
// using System.Threading.Tasks;
// using NetcoreSaas.Domain.Enums.Master;
// using Microsoft.EntityFrameworkCore;
//
// namespace NetcoreSaas.Infrastructure.Middleware.Tenancy.Store
// {
//     /// <summary>
//     /// In memory store for testing
//     /// </summary>
//     public class InDatabaseTenantStore : ITenantStore
//     {
//         private readonly MasterDbContext _db;
//         public InDatabaseTenantStore(MasterDbContext db)
//         {
//             _db = db;
//         }
//
//         public async Task<Tenant> GetTenantAsync(Guid uuid)
//         {
//             var tenant = await Task.FromResult(_db.Tenants.FirstOrDefault(f => f.Uuid == uuid));
//             return tenant;
//         }
//         
//         public Tenant GetTenant(Guid uuid)
//         {
//             var tenant = _db.Tenants.FirstOrDefault(f => f.Uuid == uuid);
//             return tenant;
//         }
//
//         public TenantUserRole? GetTenantUserRole()
//         {
//             var tenant = _db.TenantUsers.Include(f=>f.User).FirstOrDefault(f => f.TenantId == tenantId && f.UserId == userId);
//             return tenant?.Role;
//         }
//     }
// }
