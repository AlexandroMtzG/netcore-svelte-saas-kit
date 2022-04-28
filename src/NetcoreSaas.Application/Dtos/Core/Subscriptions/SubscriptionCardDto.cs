using System;

namespace NetcoreSaas.Application.Dtos.Core.Subscriptions
{
    public class SubscriptionCardDto
    {
        public string Brand { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string LastFourDigits { get; set; }
        public DateTime ExpiryDate => new DateTime(ExpiryYear, ExpiryMonth, 1);
    }
}
