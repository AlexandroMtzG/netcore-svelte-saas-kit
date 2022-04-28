using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;
using NetcoreSaas.Application.Services.Core.Subscriptions;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.Core.Subscriptions;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Infrastructure.Extensions;

namespace NetcoreSaas.WebApi.Controllers.Core.Subscriptions
{
    [ApiController]
    [Authorize]
    public class SubscriptionManagerController : ControllerBase
    {
        private readonly IConfiguration _conf;
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionManagerController(IConfiguration conf, IMasterUnitOfWork masterUnitOfWork, ISubscriptionService subscriptionService)
        {
            _conf = conf;
            _masterUnitOfWork = masterUnitOfWork;
            _subscriptionService = subscriptionService;
        }
        
        [HttpGet(ApiCoreRoutes.SubscriptionManager.GetCurrentSubscription)]
        public async Task<IActionResult> GetCurrentSubscription()
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(HttpContext.GetTenantId());
            if (tenant == null)
                return NotFound();

            try
            {
                var subscription = await _subscriptionService.GetCurrentSubscription(tenant);
                return Ok(subscription);
            }
            catch
            {
                return Ok(new SubscriptionGetCurrentResponse(null, null, null, null, null, null));
            }
        }

        [HttpGet(ApiCoreRoutes.SubscriptionManager.GetUpcomingInvoice)]
        public async Task<IActionResult> GetUpcomingInvoice()
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(HttpContext.GetTenantId());

            var upcomingInvoice = await _subscriptionService.GetUpcomingInvoice(tenant);
            if (upcomingInvoice == null)
                return NoContent();

            return Ok(upcomingInvoice);
        }

        [HttpGet(ApiCoreRoutes.SubscriptionManager.GetCoupon)]
        [AllowAnonymous]
        public async Task<IActionResult> GetCoupon(string couponId, string currency)
        {
            var coupon = await _subscriptionService.GetCoupon(couponId, currency);
            if (coupon == null)
                return BadRequest("api.errors.invalidCoupon");
            else if (!coupon.Valid)
                return BadRequest("api.errors.couponInactive");
            else if (coupon.TimesRedeemed >= coupon.MaxRedemptions)
                return BadRequest("api.errors.couponMaxRedeems");
            return Ok(coupon);
        }

        [HttpPost(ApiCoreRoutes.SubscriptionManager.UpdateSubscription)]
        public async Task<IActionResult> UpdateSubscription([FromBody] SelectedSubscriptionRequest request)
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(HttpContext.GetTenantId());
            if (tenant == null)
                return NotFound();
            var price = await _masterUnitOfWork.Subscriptions.GetPrice(request.SubscriptionPriceId);


            if (tenant.Products.Any(f => f.SubscriptionPrice.ServiceId == price.ServiceId))
            {
                return BadRequest(price.BillingPeriod == SubscriptionBillingPeriod.Once ?
                    "settings.tenant.subscription.alreadyBought" :
                    "settings.tenant.subscription.alreadySubscribed");
            }
            var currentSubscription = tenant.GetCurrentSubscription();
            try
            {
                if (currentSubscription != null)
                {
                    await _subscriptionService.CancelSubscription(tenant, currentSubscription.Id);
                    tenant.Products.Remove(currentSubscription);
                }
                var subscriptionServiceId = await _subscriptionService.AddSubscription(tenant, price, request.SubscriptionCardToken, request.SubscriptionCoupon);
                _masterUnitOfWork.Tenants.AddProduct(tenant, price, subscriptionServiceId);
                await _masterUnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return await GetCurrentSubscription();
        }

        [HttpPost(ApiCoreRoutes.SubscriptionManager.CancelSubscription)]
        public async Task<IActionResult> CancelSubscription()
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(HttpContext.GetTenantId());
            if (tenant == null)
                return NotFound();

            try
            {
                await _subscriptionService.CancelSubscription(tenant);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            await _masterUnitOfWork.CommitAsync();

            return await GetCurrentSubscription();
        }

        [HttpPost(ApiCoreRoutes.SubscriptionManager.UpdateCardToken)]
        public async Task<IActionResult> UpdateCardToken(string cardToken)
        {
            var tenant = _masterUnitOfWork.Tenants.GetById(HttpContext.GetTenantId());
            if (tenant == null)
                return NotFound();
            await _subscriptionService.AddCardTokenToCustomer(tenant, cardToken);
            return await GetCurrentSubscription();
        }

        [AllowAnonymous]
        [HttpPost(ApiCoreRoutes.SubscriptionManager.CreateCardToken)]
        public async Task<IActionResult> CreateCardToken([FromBody] SubscriptionCreateCardTokenRequest request)
        {
            var token = await _subscriptionService.CreateCardToken(request);
            return Ok(token);
        }

        [HttpPost(ApiCoreRoutes.SubscriptionManager.UpdateCard)]
        public async Task<IActionResult> UpdateCard([FromBody] SubscriptionCreateCardTokenRequest request)
        {
            var tenant = _masterUnitOfWork.Tenants.GetById(HttpContext.GetTenantId());
            if (tenant == null)
                return NotFound();
            var token = await _subscriptionService.CreateCardToken(request);
            await _subscriptionService.AddCardTokenToCustomer(tenant, token);
            return Ok(token);
        }

        [HttpPost(ApiCoreRoutes.SubscriptionManager.CreateCustomerPortalSession)]
        public async Task<IActionResult> CreateCustomerPortalSession()
        {
            var tenant = _masterUnitOfWork.Tenants.GetById(HttpContext.GetTenantId());
            if (tenant == null)
                return NotFound();
            if (string.IsNullOrEmpty(tenant.SubscriptionCustomerId))
                return BadRequest("api.errors.invalidStripeCustomerId");
            var session = await _subscriptionService.CreateUrlForCustomerPortalSession(tenant, _conf["App:URL"] + "/app/settings/organization/subscription");
            return Ok(session);
        }
    }
}
