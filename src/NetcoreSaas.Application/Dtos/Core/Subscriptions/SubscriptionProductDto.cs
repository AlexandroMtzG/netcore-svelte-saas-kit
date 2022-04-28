using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetcoreSaas.Application.Dtos.Core.Subscriptions
{
    public class SubscriptionProductDto : MasterEntityDto
    {
        public string ServiceId { get; set; }  // Stripe/Paddle Id
        public int Tier { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Badge { get; set; }
        public bool Active { get; set; }
        public bool ContactUs { get; set; }
        public int MaxUsers { get; set; }
        public int MaxWorkspaces { get; set; }
        public int MaxLinks { get; set; }
        public int MaxStorage { get; set; }
        public int MonthlyContracts { get; set; }
        public ICollection<SubscriptionPriceDto> Prices { get; set; }
        public ICollection<SubscriptionFeatureDto> Features { get; set; }
        public SubscriptionProductDto()
        {
            Prices = new Collection<SubscriptionPriceDto>();
            Features = new Collection<SubscriptionFeatureDto>();
        }
    }
}
