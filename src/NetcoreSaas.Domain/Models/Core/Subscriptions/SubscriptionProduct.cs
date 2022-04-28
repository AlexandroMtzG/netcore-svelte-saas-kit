using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetcoreSaas.Domain.Models.Core.Subscriptions
{
    public class SubscriptionProduct : MasterEntity
    {
        public string ServiceId { get; set; }
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
        public ICollection<SubscriptionPrice> Prices { get; set; }
        public ICollection<SubscriptionFeature> Features { get; set; }
        public SubscriptionProduct()
        {
            Prices = new Collection<SubscriptionPrice>();
            Features = new Collection<SubscriptionFeature>();
        }
    }
}
