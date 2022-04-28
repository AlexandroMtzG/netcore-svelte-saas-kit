using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Subscriptions;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Application.Repositories.Core
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        //Task<IEnumerable<SubscriptionPrice>> GetMyProducts(Tenant tenant, ICollection<SubscriptionPrice> prices);
        Task<Tenant> Get(Guid id);
        Task<IEnumerable<Tenant>> GetTenantsWithUsers();
        Task<TenantUser> GetTenantUser(Guid id, TenantUserStatus? withStatus = null);
        Task<TenantUser> GetTenantUser(Guid tenantId, Guid userId);
        Task<TenantUser> GetTenantUser(Guid tenantId, string email);
        Task<TenantUser> GetTenantUserByInvitationLink(Guid invitationLink);
        Task<IEnumerable<TenantUser>> GetTenantUsers(Guid tenantId);
        TenantUser AddNewUser(Tenant tenant, User user, bool createUser, TenantUserRole role, TenantUserJoined joined, TenantUserStatus status);
        void AddProduct(Tenant tenant, SubscriptionPrice price, string subscriptionId = null);
        void RemoveUser(TenantUser tenantUser);
        Task RemoveUser(Guid tenantId, Guid userId);
        void DeleteWithChildren(Tenant tenant);
        void AddJoinSettings(Tenant tenant, bool linkActive = false, bool publicUrl = false, bool requireAcceptance = false);
        Task<TenantJoinSettings> GetJoinSettingsByLink(Guid linkUuid);
        Task<IEnumerable<TenantProduct>> GetTenantProducts(Tenant tenant, bool activeOnly);
    }
}
