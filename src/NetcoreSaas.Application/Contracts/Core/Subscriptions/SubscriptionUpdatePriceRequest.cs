using System;

namespace NetcoreSaas.Application.Contracts.Core.Subscriptions
{
    public class SubscriptionUpdatePriceRequest
    {
        public Guid Id { get; set; }
        public bool Active { get; set; }
    }
}
