namespace NetcoreSaas.Application.Dtos.Core.Subscriptions
{
    public class SubscriptionInvoiceLineDto
    {
        public string Description { get; set; }
        public string PlanName { get; set; }
        public string PlanInterval { get; set; }
        public string PlanCurrency { get; set; }
        public long? PriceUnitAmount { get; set; }
        public string PriceType { get; set; }
        public SubscriptionInvoiceLineDto(string description, string planName, string planInterval, string planCurrency, long? priceUnitAmount, string priceType)
        {
            Description = description
                .Replace("(at ", "(a ")
                .Replace("/ month", "/ mes")
                .Replace("/ year", "/ año");
            PlanName = planName;
            PlanInterval = planInterval;
            PlanCurrency = planCurrency;
            PriceUnitAmount = priceUnitAmount;
            PriceType = priceType;
        }
    }
}
