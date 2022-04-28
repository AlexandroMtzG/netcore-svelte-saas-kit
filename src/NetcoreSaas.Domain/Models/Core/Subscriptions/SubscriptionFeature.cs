using System;

namespace NetcoreSaas.Domain.Models.Core.Subscriptions
{
    public class SubscriptionFeature : MasterEntity
    {
        public int Order { get; set; }
        public Guid SubscriptionProductId { get; set; }
        public SubscriptionProduct SubscriptionProduct { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool TranslateInFrontend { get; set; }
        public bool Included { get; set; }
    }
}
