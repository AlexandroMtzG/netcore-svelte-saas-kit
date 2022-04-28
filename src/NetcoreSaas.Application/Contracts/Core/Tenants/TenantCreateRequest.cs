using System.ComponentModel.DataAnnotations;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;

namespace NetcoreSaas.Application.Contracts.Core.Tenants
{
    public class TenantCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public SelectedSubscriptionRequest SelectedSubscription { get; set; }
        public string Subdomain { get; set; }
    }
}
