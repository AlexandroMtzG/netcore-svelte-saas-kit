using System;

namespace NetcoreSaas.Application.Contracts.Core.Subscriptions
{
    public class SubscriptionUpdateProductRequest
    {
        public Guid Id { get; set; }
        public int Tier { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Badge { get; set; }
        public bool Active { get; set; }
        public string Image { get; set; }
        public int MaxUsers { get; set; }
    }
}
