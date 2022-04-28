using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Application.Contracts.Core.Tenants;
using NetcoreSaas.Application.Dtos.App.Usages;
using NetcoreSaas.Application.Services.Core.Subscriptions;
using NetcoreSaas.Application.Services.Core.Tenants;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.App.Links;
using NetcoreSaas.Domain.Enums.App.Usages;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Infrastructure.Services.Core
{
    public class TenantService : ITenantService
    {
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly ISubscriptionService _subscriptionService;
        
        public TenantService(IMasterUnitOfWork masterUnitOfWork, ISubscriptionService subscriptionService)
        {
            _masterUnitOfWork = masterUnitOfWork;
            _subscriptionService = subscriptionService;
        }

        public async Task<IEnumerable<Tenant>> GetUserTenants(User user)
        {
            var tenants = await _masterUnitOfWork.Users.GetUserTenants(user);
            //foreach (var tenant in tenantsDto)
            //{
            //    tenant.Prices = (await subscriptionService.GetTenantSubscriptions(tenant)).ToList();
            //}
            return tenants.Select(f=>f.Tenant);
        }

        public async Task<Tenant> AddNewTenant(string organization, string email, SelectedSubscriptionRequest subscriptionRequest, string subdomain = null)
        {
            var price = await _masterUnitOfWork.Subscriptions.GetPrice(subscriptionRequest.SubscriptionPriceId);

            organization ??= email;
            if (string.IsNullOrEmpty(subdomain))
                subdomain = GenerateSubdomainFromOrganizationName(organization);
            var subscriptionCustomerId = await _subscriptionService.CreateCustomer(organization, subdomain, email);
            var tenant = new Tenant() 
            {
                Name = organization,
                Subdomain = subdomain,
                SubscriptionCustomerId = subscriptionCustomerId
            };

            if (string.IsNullOrEmpty(subscriptionRequest.SubscriptionCardToken) == false)
            {
                await _subscriptionService.AddCardTokenToCustomer(tenant, subscriptionRequest.SubscriptionCardToken);
            }
            var subscriptionServiceId = await _subscriptionService.AddSubscription(tenant, price, subscriptionRequest.SubscriptionCardToken, subscriptionRequest.SubscriptionCoupon);
            _masterUnitOfWork.Tenants.AddProduct(tenant, price, subscriptionServiceId);
            _masterUnitOfWork.Tenants.Add(tenant);
            return tenant;
        }

        //private async Task<string> GenerateSubdomain(string organization)
        //{
        //    int subdomainNumber = 0;
        //    string subdomain = "";
        //    // Looking for non-existing subdomains
        //    while (true)
        //    {
        //        subdomain = GenerateSubdomainFromOrganizationName(organization);
        //        if (subdomainNumber != 0)
        //            subdomain += subdomainNumber.ToString();
        //        if (await masterUnitOfWork.Tenants.CountAsync(f => f.Subdomain.ToLower() == subdomain.ToLower()) == 0)
        //        {
        //            break;
        //        }
        //        subdomainNumber++;
        //    }
        //    return subdomain;
        //}

        private string GenerateSubdomainFromOrganizationName(string organization)
        {
            var withoutSpaces = System.Text.RegularExpressions.Regex.Replace(organization, @"[^A-Za-z0-9]+", "");
            var rgx = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(withoutSpaces, "").ToLower().Trim();
        }

        public async Task CancelAndDelete(Tenant tenant)
        {
            foreach (var product in tenant.Products)
            {
                try
                {
                    await _subscriptionService.CancelSubscription(tenant, product.Id);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex);
                }
            }
            tenant.Users = (await _masterUnitOfWork.Tenants.GetTenantUsers(tenant.Id)).ToList();
            _masterUnitOfWork.Tenants.DeleteWithChildren(tenant);
        }

        public async Task<TenantFeaturesDto> GetFeatures(Tenant tenant)
        {
            var activeProduct = await GetTenantProduct(tenant);
            // var monthlyDocuments = await GetFeature_MonthlyDocuments(tenant);
            // var maxNumberOfWorkspaces = await GetFeature_MaxNumberOfWorkspaces(tenant);
            // var maxNumberOfUsers = await GetFeature_MaxNumberOfUsers(tenant);
            return new TenantFeaturesDto()
            {
                MonthlyContracts = activeProduct?.MonthlyContracts ?? 0,
                MaxWorkspaces = activeProduct?.MaxWorkspaces ?? 0,
                MaxUsers = activeProduct?.MaxUsers ?? 0,
                MaxLinks = activeProduct?.MaxLinks ?? 0,
                MaxStorage = activeProduct?.MaxStorage ?? 0
            };
        }

        
        public async Task<AppUsageSummaryDto> GetTenantUsageSummary(Tenant tenant, AppUsageType type)
        {
            var summary = new AppUsageSummaryDto(type);
            var workspaceIds = tenant.Workspaces.Select(f => f.Id).ToList();

            if (type is AppUsageType.All or AppUsageType.Providers)
            {
                var clients = await _masterUnitOfWork.Links.CountAllProviders(workspaceIds);
                summary.Clients = clients;
            }

            if (type is AppUsageType.All or AppUsageType.Clients)
            {
                var clients = await _masterUnitOfWork.Links.CountAllClients(workspaceIds);
                summary.Clients = clients;
            }
            
            if (type is AppUsageType.All or AppUsageType.PendingInvitations)
            {
                var pending = await _masterUnitOfWork.Links.CountAll(workspaceIds, LinkStatus.Pending);
                summary.PendingInvitations = pending;
            }

            if (type is AppUsageType.All or AppUsageType.Employees)
            {
                var employees = await _masterUnitOfWork.Employees.CountAll(tenant.Id);
                summary.Employees = employees;
            }

            if (type is AppUsageType.All or AppUsageType.Contracts)
            {
                var today = DateTime.Now;
                var fromDate = new DateTime(today.Year, today.Month, 1);
                var toDate = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                var contracts = await _masterUnitOfWork.Contracts.CountAll(workspaceIds, fromDate, toDate);
                summary.Contracts = contracts;
            }

            if (type is AppUsageType.All or AppUsageType.Storage)
            {
                // TODO: Calculate storage usage
                summary.Storage = Convert.ToDecimal(0.1);
            }

            return summary;
        }

        // public async Task<IEnumerable<TenantProduct>> GetTenantProducts(Tenant tenant)
        // {
        //     return await _masterUnitOfWork.Tenants.GetTenantProducts(tenant);
        // }

        public async Task<int> GetFeature_MaxNumberOfUsers(Tenant tenant)
        {
            return await GetFeature_Key(tenant, "maxNumberOfUsers");
        }

        public async Task<bool> GetFeature_WhatsApp(Tenant tenant)
        {
            return await GetFeature_Included(tenant, "whatsApp");
        }
        
        public async Task<TenantProduct> GetTenantProduct(Tenant tenant)
        {
            var activeProduct = (await _masterUnitOfWork.Tenants.GetTenantProducts(tenant, true)).FirstOrDefault();
            return activeProduct;
        }

        private async Task<int> GetFeature_Key(Tenant tenant, string key)
        {
            var activeProduct = await GetTenantProduct(tenant);
            if(activeProduct == null)
                throw new Exception("api.errors.noSubscription");
            
            return activeProduct.GetFeatureValue(key);
        }

        private async Task<bool> GetFeature_Included(Tenant tenant, string key)
        {
            var activeProduct = await GetTenantProduct(tenant);
            if(activeProduct == null)
                throw new Exception("api.errors.noSubscription");
            
            return activeProduct.GetFeatureIncluded(key);
        }
    }
}
