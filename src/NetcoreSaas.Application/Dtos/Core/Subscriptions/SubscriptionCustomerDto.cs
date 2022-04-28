using System;

namespace NetcoreSaas.Application.Dtos.Core.Subscriptions
{
    public class SubscriptionCustomerDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
    }
}
