using System.ComponentModel.DataAnnotations;

namespace NetcoreSaas.Application.Contracts.Core.Subscriptions
{
    public class SubscriptionCreateCardTokenRequest
    {
        [Required]
        public string Number { get; set; }
        [Required]
        public int ExpiryMonth { get; set; }
        [Required]
        public int ExpiryYear { get; set; }
        [Required]
        public string Cvc { get; set; }
    }
}
