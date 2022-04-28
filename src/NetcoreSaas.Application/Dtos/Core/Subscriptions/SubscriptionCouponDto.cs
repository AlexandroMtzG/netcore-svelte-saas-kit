namespace NetcoreSaas.Application.Dtos.Core.Subscriptions
{
    public class SubscriptionCouponDto
    {
        public string Name { get; set; }
        public decimal? AmountOff { get; set; }
        public decimal? PercentOff { get; set; }
        public string Currency { get; set; }
        public bool Valid { get; set; }
        public long? TimesRedeemed { get; set; }
        public long? MaxRedemptions { get; set; }
    }
}
