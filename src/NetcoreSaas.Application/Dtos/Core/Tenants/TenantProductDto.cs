using System;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;

namespace NetcoreSaas.Application.Dtos.Core.Tenants
{
    public class TenantProductSimpleDto : MasterEntityDto
    {
        public Guid SubscriptionPriceId { get; set; }
        public SubscriptionPriceDto SubscriptionPrice { get; set; }
        public bool Active { get; set; }
        // public string SubscriptionServiceId { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? TrialEnds { get; set; }
        public SubscriptionProductDto SubscriptionProduct => SubscriptionPrice.SubscriptionProduct;
    }
}