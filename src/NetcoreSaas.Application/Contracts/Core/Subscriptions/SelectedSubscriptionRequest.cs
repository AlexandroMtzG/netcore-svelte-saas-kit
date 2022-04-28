using System;

namespace NetcoreSaas.Application.Contracts.Core.Subscriptions
{
    public class SelectedSubscriptionRequest
    {
        public Guid SubscriptionPriceId { get; set; }
        public string SubscriptionCardToken { get; set; }
        public string SubscriptionCoupon { get; set; }
        public SelectedSubscriptionRequest(Guid subscriptionPriceId, string subscriptionCardToken = null, string subscriptionCoupon = null)
        {
            SubscriptionPriceId = subscriptionPriceId;
            SubscriptionCardToken = subscriptionCardToken;
            SubscriptionCoupon = subscriptionCoupon;
        }
    }
}
