using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetcoreSaas.Application.Repositories.Core;
using NetcoreSaas.Domain.Enums.Core.Subscriptions;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Subscriptions;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Infrastructure.Data;

namespace NetcoreSaas.Infrastructure.Repositories.Core
{
    public class TenantRepository : MasterRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(MasterDbContext context) : base(context)
        {
        }

        public async Task<Tenant> Get(Guid id)
        {
            var lista = Context.Tenants
                .Include(f => f.Workspaces)
                .Include(f => f.TenantJoinSettings)
                .Include(f => f.Products.Where(x => x.Active)).ThenInclude(f=>f.SubscriptionPrice).ThenInclude(f=>f.SubscriptionProduct)
                .AsQueryable();

            lista = lista.Include(f => f.Users).ThenInclude(f => f.User);
            return await lista.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<Tenant>> GetTenantsWithUsers()
        {
            return await Context.Tenants
                .Include(f => f.Workspaces)
                .Include(f => f.Products).ThenInclude(f => f.SubscriptionPrice).ThenInclude(f => f.SubscriptionProduct)
                .Include(f => f.Users)
                .ThenInclude(f => f.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<TenantUser>> GetTenantUsers(Guid tenantId)
        {
            return await Context.TenantUsers
                .Include(f => f.Tenant)
                .Include(f => f.User)
                .Where(f => f.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<TenantUser> GetTenantUser(Guid id, TenantUserStatus? withStatus = null)
        {
            if (withStatus.HasValue)
            {
                return await Context.TenantUsers
                    .Include(f => f.Tenant)
                    .Include(f => f.User)
                    .ThenInclude(f=>f.Workspaces)
                    .FirstOrDefaultAsync(f => f.Id == id && f.Status == withStatus);
            }

            return await Context.TenantUsers
                .Include(f => f.Tenant)
                .Include(f => f.User)
                .ThenInclude(f=>f.Workspaces)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<TenantUser> GetTenantUserByInvitationLink(Guid invitationLink)
        {
            return await Context.TenantUsers
                .Include(f => f.Tenant)
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.InvitationLink == invitationLink);
        }

        public async Task<TenantUser> GetTenantUser(Guid tenantId, Guid userId)
        {
            return await Context.TenantUsers
                .Include(f => f.Tenant)
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.TenantId == tenantId && f.UserId == userId);
        }

        public async Task<TenantUser> GetTenantUser(Guid tenantId, string email)
        {
            return await Context.TenantUsers
                .Include(f=>f.User)
                .FirstOrDefaultAsync(f => f.TenantId == tenantId && f.User.Email == email.ToLower().Trim());
        }

        public TenantUser AddNewUser(Tenant tenant, User user, bool createUser, TenantUserRole role, TenantUserJoined joined, TenantUserStatus status)
        {
            var tenantUser = new TenantUser()
            {
                Id = Guid.NewGuid(),
                Tenant = tenant,
                User = user,
                Role = role,
                Joined = joined,
                Status = status
            };
            user.DefaultTenant = tenant;

            Context.TenantUsers.Add(tenantUser);
            if (createUser)
                Context.Users.Add(user);
            return tenantUser;
        }

        public void AddProduct(Tenant tenant, SubscriptionPrice price, string subscriptionId = null)
        {
            DateTime? endDate = null;
            switch (price.BillingPeriod)
            {
                case SubscriptionBillingPeriod.Daily:
                    endDate = DateTime.Now.AddDays(1);
                    break;
                case SubscriptionBillingPeriod.Weekly:
                    endDate = DateTime.Now.AddDays(7);
                    break;
                case SubscriptionBillingPeriod.Monthly:
                    endDate = DateTime.Now.AddMonths(1);
                    break;
                case SubscriptionBillingPeriod.Yearly:
                    endDate = DateTime.Now.AddYears(1);
                    break;
                case SubscriptionBillingPeriod.Once:
                    break;
            }
            Context.TenantProducts.Add(new TenantProduct()
            {
                Tenant = tenant,
                TenantId = tenant.Id,
                SubscriptionPrice = price,
                SubscriptionPriceId = price.Id,
                Active = true,
                CancelledAt = null,
                SubscriptionServiceId = subscriptionId,
                MaxUsers = price.SubscriptionProduct?.MaxUsers ?? 0,
                MaxWorkspaces = price.SubscriptionProduct?.MaxWorkspaces ?? 0,
                MaxLinks = price.SubscriptionProduct?.MaxLinks ?? 0,
                MaxStorage = price.SubscriptionProduct?.MaxStorage ?? 0,
                MonthlyContracts = price.SubscriptionProduct?.MonthlyContracts ?? 0,
                StartDate = DateTime.Now,
                EndDate = endDate
            });
        }

        public void DeleteWithChildren(Tenant tenant)
        {
            var workspaces = Context.Workspaces.Where(f => f.TenantId == tenant.Id).ToList();
            foreach (var workspace in workspaces)
            {
                var links = Context.Links
                    .Include(f=>f.LinkInvitation)
                    .Where(f => f.ProviderWorkspaceId == workspace.Id || f.ClientWorkspaceId == workspace.Id).ToList();
                Context.Links.RemoveRange(links);
                Context.Workspaces.Remove(workspace);
            }
            //foreach (var user in tenant.Users)
            //{
            //    context.TenantUsers.Remove(user);
            //}
            //context.TenantJoinSettings.Remove(tenant.TenantJoinSettings);
            Context.Tenants.Remove(tenant);
        }

        public async Task<TenantJoinSettings> GetJoinSettingsByLink(Guid linkUuid)
        {
            return await Context.TenantJoinSettings.FirstOrDefaultAsync(f => f.Link == linkUuid && f.LinkActive);
        }

        public void AddJoinSettings(Tenant tenant, bool linkActive = false, bool publicUrl = false, bool requireAcceptance = false)
        {
            tenant.TenantJoinSettings = new TenantJoinSettings(tenant, linkActive ? Guid.NewGuid() : Guid.Empty, linkActive, publicUrl, requireAcceptance);
            Context.TenantJoinSettings.Add(tenant.TenantJoinSettings);
        }

        public void RemoveUser(TenantUser tenantUser)
        {
            if (tenantUser != null)
                Context.TenantUsers.Remove(tenantUser);
        }

        public async Task RemoveUser(Guid tenantId, Guid userId)
        {
            var tenantUser = await GetTenantUser(tenantId, userId);
            RemoveUser(tenantUser);
        }

        public async Task<IEnumerable<TenantProduct>> GetTenantProducts(Tenant tenant, bool activeOnly)
        {
            var list = Context.TenantProducts
                .Where(f => f.TenantId == tenant.Id)
                .Include(f => f.SubscriptionPrice)
                .ThenInclude(f => f.SubscriptionProduct)
                .ThenInclude(f => f.Features)
                .AsQueryable();
            if (activeOnly)
            {
                list = list.Where(f => f.Active);
            }

            var products = await list.ToListAsync();
            if (tenant.IsAdmin() && products.Count == 0)
            {
                // Add highest product to admin
                var productTier = Context.SubscriptionProducts.Max(f => f.Tier);
                var highestProduct = Context.SubscriptionProducts
                    .Include(f=>f.Features)
                    .Include(f=>f.Prices)
                    .ThenInclude(f=>f.SubscriptionProduct)
                    .Single(f => f.Tier == productTier);
                var highestPrice = highestProduct.Prices.OrderBy(f => f.BillingPeriod).Last();
                var tenantProduct = new TenantProduct()
                {
                    Tenant = tenant,
                    SubscriptionPrice = highestPrice,
                    Active = true
                };

                products = new List<TenantProduct>() { tenantProduct };
            }

            foreach (var product in products)
            {
                product.TrialEnds = product.EndOfTrialDate();
            }
            return products;
        }
    }
}
