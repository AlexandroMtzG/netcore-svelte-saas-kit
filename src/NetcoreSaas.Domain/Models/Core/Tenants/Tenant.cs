using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NetcoreSaas.Domain.Enums.Core.Subscriptions;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Domain.Models.Core.Tenants
{
    public class Tenant : MasterEntity
    {
        public Guid Uuid { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Domain { get; set; }
        public string Subdomain { get; set; }
        public string Icon { get; set; }
        public string Logo { get; set; }
        public string LogoDarkmode { get; set; }
        public string SubscriptionCustomerId { get; set; }
        public TenantJoinSettings TenantJoinSettings { get; set; }
        public ICollection<TenantUser> Users { get; set; }
        public ICollection<TenantProduct> Products { get; set; }
        public ICollection<Workspace> Workspaces { get; set; }
        public TenantUser CurrentUser;

        public Tenant()
        {
            Users = new Collection<TenantUser>();
            Products = new Collection<TenantProduct>();
        }

        public bool IsAdmin()
        {
            return Subdomain == "admin";
        }

        public bool HasMaxNumberOfUsers()
        {
            return GetMaxNumberOfUsers() > 0 && Users.Count >= GetMaxNumberOfUsers();
        }

        public int GetMaxNumberOfUsers()
        {
            if (Products.Count > 0)
                return Products.First().SubscriptionPrice.SubscriptionProduct.MaxUsers;
            else
                return 0;
        }

        public TenantProduct GetCurrentSubscription(Guid? subscriptionId = null)
        {
            TenantProduct subscription;
            if (subscriptionId.HasValue)
                subscription = Products.Single(f => f.Id == subscriptionId.Value && f.SubscriptionPrice.BillingPeriod != SubscriptionBillingPeriod.Once);
            else
                subscription = Products.SingleOrDefault(f => f.Active && f.SubscriptionPrice.BillingPeriod != SubscriptionBillingPeriod.Once);
            return subscription;
        }

        public List<TenantUser> GetOwners()
        {
            return Users?.Where(f => f.Role == TenantUserRole.Owner).ToList() ?? new List<TenantUser>();
        }
    }
}
