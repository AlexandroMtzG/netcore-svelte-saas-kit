using System;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;

namespace NetcoreSaas.Application.Dtos.Core.Tenants
{
    public class TenantProductDto : MasterEntityDto
    {
        public Guid TenantId { get; set; }
        public TenantSimpleDto Tenant { get; set; }
        public Guid SubscriptionPriceId { get; set; }
        public SubscriptionPriceDto SubscriptionPrice { get; set; }
        public bool Active { get; set; }
        public string SubscriptionServiceId { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? TrialEnds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int MaxUsers { get; set; }
        public int MaxWorkspaces { get; set; }
        public int MaxLinks { get; set; }
        public int MaxStorage { get; set; }
        public int MonthlyContracts { get; set; }
        public SubscriptionProductDto SubscriptionProduct => SubscriptionPrice.SubscriptionProduct;
    }
}
