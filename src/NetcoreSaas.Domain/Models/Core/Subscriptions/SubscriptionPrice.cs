using System;
using NetcoreSaas.Domain.Enums.Core.Subscriptions;

namespace NetcoreSaas.Domain.Models.Core.Subscriptions
{
    public class SubscriptionPrice : MasterEntity
    {
        public Guid SubscriptionProductId { get; set; }
        public SubscriptionProduct SubscriptionProduct { get; set; }
        public string ServiceId { get; set; }
        public SubscriptionPriceType Type { get; set; }
        public SubscriptionBillingPeriod BillingPeriod { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int TrialDays { get; set; }
        public bool Active { get; set; }
        public decimal? PriceBefore { get; set; }
    }
}
