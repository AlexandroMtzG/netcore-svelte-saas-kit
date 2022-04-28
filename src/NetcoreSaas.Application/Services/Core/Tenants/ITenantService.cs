using System.Collections.Generic;
using System.Threading.Tasks;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Application.Contracts.Core.Tenants;
using NetcoreSaas.Application.Dtos.App.Usages;
using NetcoreSaas.Domain.Enums.App.Usages;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Application.Services.Core.Tenants
{
    public interface ITenantService
    {
        Task<IEnumerable<Tenant>> GetUserTenants(User user);
        Task<Tenant> AddNewTenant(string organization, string email, SelectedSubscriptionRequest subscriptionRequest, string subdomain = null);
        Task CancelAndDelete(Tenant tenant);
        Task<TenantFeaturesDto> GetFeatures(Tenant tenant);
        Task<AppUsageSummaryDto> GetTenantUsageSummary(Tenant tenant, AppUsageType type);
    }
}
