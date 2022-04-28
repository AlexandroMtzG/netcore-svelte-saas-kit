using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;
using NetcoreSaas.Domain.Models.Core.Subscriptions;
using NetcoreSaas.Domain.Models.Core.Tenants;

namespace NetcoreSaas.Application.Services.Core.Subscriptions
{  
    public interface ISubscriptionService
    {
        Task<SubscriptionGetCurrentResponse> GetCurrentSubscription(Tenant tenant);
        Task<SubscriptionCustomerDto> GetCustomer(Tenant tenant);
        Task<IEnumerable<SubscriptionCardDto>> GetCards(Tenant tenant);
        Task<SubscriptionCouponDto> GetCoupon(string couponId, string currency = null);
        Task<IEnumerable<SubscriptionPaymentMethodDto>> GetPaymentMethods(Tenant tenant);
        Task<IEnumerable<SubscriptionInvoiceDto>> GetInvoices(Tenant tenant);
        Task<SubscriptionInvoiceDto> GetUpcomingInvoice(Tenant tenant);
        Task<string> CreateUrlForCustomerPortalSession(Tenant tenant, string returnUrl);
        Task<string> CreateCustomer(string organization, string subdomain, string email);
        Task<string> UpdateCustomer(Tenant tenant);
        Task<string> AddSubscription(Tenant tenant, SubscriptionPrice stripePrice, string cardToken = null, string stripeCoupon = null);
        Task DeleteCustomer(Tenant tenant);
        Task CancelSubscription(Tenant tenant, Guid? subscriptionId = null);
        Task AddCardTokenToCustomer(Tenant tenant, string cardToken);
        Task<string> CreateCardToken(SubscriptionCreateCardTokenRequest card);
        Task<IEnumerable<SubscriptionPriceDto>> GetPrices(string productId = null);
        Task<SubscriptionProductDto> SyncProduct(SubscriptionProductDto product);
        Task<SubscriptionPriceDto> SyncPrice(SubscriptionPriceDto price, SubscriptionProductDto product);
        Task DeleteProduct(SubscriptionProductDto product);
        Task DeletePrice(SubscriptionPriceDto price);
    }
}
