using System;
using System.Linq;
using NetcoreSaas.Domain.Enums.Core.Subscriptions;
using NetcoreSaas.Domain.Models.Core.Subscriptions;

namespace NetcoreSaas.Domain.Models.Core.Tenants
{
    public class TenantProduct : MasterEntity
    {
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public Guid SubscriptionPriceId { get; set; }
        public SubscriptionPrice SubscriptionPrice { get; set; }
        public bool Active { get; set; }
        public string SubscriptionServiceId { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? TrialEnds;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int MaxUsers { get; set; }
        public int MaxWorkspaces { get; set; }
        public int MaxLinks { get; set; }
        public int MaxStorage { get; set; }
        public int MonthlyContracts { get; set; }

        public DateTime? EndOfTrialDate()
        {
            if (StartDate == DateTime.MinValue)
                StartDate = CreatedAt;
            
            if (Tenant != null && Tenant.IsAdmin())
                return null;
            if (SubscriptionPrice.BillingPeriod == SubscriptionBillingPeriod.Once)
                return null;
            else if (SubscriptionPrice.TrialDays == 0)
                return null;
            else
                return StartDate.AddDays(SubscriptionPrice.TrialDays);
        }

        public int GetFeatureValue(string featureKey)
        {
            var feature = GetFeature(featureKey);
            if (feature == null)
                return -1;
            string intValue = feature.Value.Replace(",", "");
            if (intValue.Contains(" "))
                return Convert.ToInt32(intValue.Split(' ')[0]);
            return Convert.ToInt32(intValue);
        }

        public bool GetFeatureIncluded(string featureKey)
        {
            var feature = GetFeature(featureKey);
            if (feature == null)
                return false;
            return feature.Included;
        }

        private SubscriptionFeature GetFeature(string key)
        {
            return SubscriptionPrice.SubscriptionProduct.Features.SingleOrDefault(f => f.Key == key);
        }
    }
}
