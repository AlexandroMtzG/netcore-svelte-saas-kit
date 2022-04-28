using Stripe;
using Stripe.BillingPortal;
using Microsoft.Extensions.Options;
using NetcoreSaas.Domain.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using AutoMapper;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;
using NetcoreSaas.Application.Dtos.Core.Tenants;
using NetcoreSaas.Application.Services.Core.Subscriptions;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.Core.Subscriptions;
using NetcoreSaas.Domain.Extensions;
using NetcoreSaas.Domain.Models.Core.Subscriptions;
using NetcoreSaas.Domain.Models.Core.Tenants;

namespace NetcoreSaas.Infrastructure.Services.Subscription
{
    public class SubscriptionStripeService : ISubscriptionService
    {
        private readonly IMapper _mapper;
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        public SubscriptionStripeService(IOptions<SubscriptionSettings> subscriptionSettings, IMapper mapper, IMasterUnitOfWork masterUnitOfWork)
        {
            _mapper = mapper;
            _masterUnitOfWork = masterUnitOfWork;
            
            StripeConfiguration.ApiKey = subscriptionSettings.Value.SecretKey;
        }

        private readonly SubscriptionService _subscriptionService = new SubscriptionService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly InvoiceService _invoiceService = new InvoiceService();
        private readonly InvoiceItemService _invoiceItemService = new InvoiceItemService();
        private readonly CardService _cardService = new CardService();
        private readonly TokenService _tokenService = new TokenService();
        private readonly PaymentMethodService _paymentMethodService = new PaymentMethodService();
        private readonly SessionService _sessionService = new SessionService();
        private readonly ProductService _productService = new ProductService();
        private readonly PriceService _priceService = new PriceService();
        private readonly CouponService _couponService = new CouponService();
        //private readonly PaymentIntentService paymentIntentService = new PaymentIntentService();
        //private readonly OrderService orderService = new OrderService();

        public async Task<SubscriptionGetCurrentResponse> GetCurrentSubscription(Tenant tenant)
        {
            var myProducts = (await _masterUnitOfWork.Tenants.GetTenantProducts(tenant, true)).ToList();
            var myProductsDto = _mapper.Map<IEnumerable<TenantProductDto>>(myProducts).ToList();
            var customer = await GetCustomer(tenant);
            var invoices = await GetInvoices(tenant);
            var cards = await GetCards(tenant);
            var paymentMethods = await GetPaymentMethods(tenant);

            TenantProductDto activeProduct = null;
            if (myProductsDto.Count(f=>f.Active) > 0)
                activeProduct = myProductsDto.OrderByDescending(f => f.SubscriptionPrice.SubscriptionProduct.Tier).FirstOrDefault(f => f.Active);

            return new SubscriptionGetCurrentResponse(activeProduct, myProductsDto, customer, invoices, cards, paymentMethods);
        }

        public async Task<SubscriptionCustomerDto> GetCustomer(Tenant tenant)
        {
            if (string.IsNullOrEmpty(tenant.SubscriptionCustomerId))
                return null;
            var customer = await _customerService.GetAsync(tenant.SubscriptionCustomerId);
            return new SubscriptionCustomerDto()
            {
                Id = customer.Id,
                Email = customer.Email,
                Name = customer.Name,
                Description = customer.Description,
                Phone = customer.Phone,
                Currency = customer.Currency,
                Created = customer.Created
            };
        }

        //public async Task<IEnumerable<SubscriptionPriceDto>> GetTenantSubscriptions(TenantDto tenant)
        //{
        //    var prices = await masterUnitOfWork.Subscriptions.GetPricesWithProducts();
        //    var activeProducts = new List<SubscriptionPriceDto>();
        //    if(tenant.SubscriptionCustomerId == null)
        //        return new List<SubscriptionPriceDto>();

        //    var customer = await customerService.GetAsync(tenant.SubscriptionCustomerId);

        //    if (customer.Subscriptions?.Count() > 0)
        //    {
        //        foreach (var subscription in customer.Subscriptions)
        //        {
        //            var price = prices.FirstOrDefault(f => f.ServiceId == subscription.Id);
        //            price.SubscriptionPlan = new SubscriptionPlanDto()
        //            {
        //                //TrialPeriodDays = subscription.Plan.TrialPeriodDays.GetInt() : 0
        //            };
        //            activeProducts.Add(price);
        //        }
        //    }

        //    var invoices = await invoiceService.ListAsync(new InvoiceListOptions()
        //    {
        //        Customer = tenant.SubscriptionCustomerId
        //    });
        //    foreach (var invoice in invoices)
        //    {
        //        foreach (var line in invoice.Lines)
        //        {
        //            var price = prices.FirstOrDefault(f => f.ServiceId == line.Price.Id && f.BillingPeriod == SubscriptionBillingPeriod.Once);
        //            if (price != null && activeProducts.Exists(f => f.ServiceId == line.Price.Id) == false)
        //            {
        //                activeProducts.Add(price);
        //            }
        //        }
        //    }

        //    return activeProducts.OrderByDescending(f => f.Price).ToList();
        //}

        public async Task<IEnumerable<SubscriptionCardDto>> GetCards(Tenant tenant)
        {
            if (string.IsNullOrEmpty(tenant.SubscriptionCustomerId))
                return new List<SubscriptionCardDto>();

            var cards = await _cardService.ListAsync(tenant.SubscriptionCustomerId);
            var cardsDto = new List<SubscriptionCardDto>();
            foreach (var card in cards)
            {
                cardsDto.Add(new SubscriptionCardDto()
                {
                    Brand = card.Brand,
                    ExpiryMonth = card.ExpMonth.GetInt(),
                    ExpiryYear = card.ExpYear.GetInt(),
                    LastFourDigits = card.Last4,
                });
            }
            return cardsDto;
        }

        public async Task<SubscriptionCouponDto> GetCoupon(string couponId, string currency = null)
        {
            Coupon coupon;
            try
            {
                coupon = await _couponService.GetAsync(couponId);
            }
            catch
            {
                return null;
            }

            if (coupon.AmountOff != null && coupon.AmountOff > 0 && string.IsNullOrEmpty(currency) == false && coupon.Currency != currency)
            {
                return null;
            }
            else
            {
                return new SubscriptionCouponDto()
                {
                    Name = coupon.Name,
                    AmountOff = coupon.AmountOff,
                    PercentOff = coupon.PercentOff,
                    Currency = coupon.Currency,
                    Valid = coupon.Valid,
                    TimesRedeemed = coupon.TimesRedeemed,
                    MaxRedemptions = coupon.MaxRedemptions
                };
            }
        }

        public async Task<IEnumerable<SubscriptionPaymentMethodDto>> GetPaymentMethods(Tenant tenant)
        {
            if (string.IsNullOrEmpty(tenant.SubscriptionCustomerId))
                return new List<SubscriptionPaymentMethodDto>();

            var list = await _paymentMethodService.ListAsync(new PaymentMethodListOptions()
            {
                Customer = tenant.SubscriptionCustomerId,
                Type = "card"
            });
            var paymentMethodsDto = new List<SubscriptionPaymentMethodDto>();
            foreach (var paymentMethod in list)
            {
                var paymentMethodDto = new SubscriptionPaymentMethodDto
                {
                    Card = new SubscriptionCardDto()
                    {
                        Brand = paymentMethod.Card.Brand,
                        ExpiryMonth = paymentMethod.Card.ExpMonth.GetInt(),
                        ExpiryYear = paymentMethod.Card.ExpYear.GetInt(),
                        LastFourDigits = paymentMethod.Card.Last4,
                    }
                };
                paymentMethodsDto.Add(paymentMethodDto);
            }
            return paymentMethodsDto;
        }

        public async Task<IEnumerable<SubscriptionInvoiceDto>> GetInvoices(Tenant tenant)
        {
            if (string.IsNullOrEmpty(tenant.SubscriptionCustomerId))
                return new List<SubscriptionInvoiceDto>();
            var list = await _invoiceService.ListAsync(new InvoiceListOptions()
            {
                Customer = tenant.SubscriptionCustomerId
            });
            var invoicesDto = new List<SubscriptionInvoiceDto>();
            foreach (var invoice in list)
            {
                var lines = new List<SubscriptionInvoiceLineDto>();
                foreach (var line in invoice.Lines)
                {
                    lines.Add(new SubscriptionInvoiceLineDto(line.Description, line.Plan?.Nickname ?? line.Description, line.Plan?.Interval ?? "", line.Currency, line.Price.UnitAmount, line.Price.Type));
                }
                var invoiceDto = new SubscriptionInvoiceDto(invoice.Created, invoice.InvoicePdf, lines);
                invoicesDto.Add(invoiceDto);
            }
            return invoicesDto;
        }

        public async Task<SubscriptionInvoiceDto> GetUpcomingInvoice(Tenant tenant)
        {
            try
            {
                var invoice = await _invoiceService.UpcomingAsync(new UpcomingInvoiceOptions()
                {
                    Customer = tenant.SubscriptionCustomerId
                });
                var lines = new List<SubscriptionInvoiceLineDto>();
                foreach (var line in invoice.Lines)
                {
                    lines.Add(new SubscriptionInvoiceLineDto(line.Description, line.Plan.Nickname, line.Plan.Interval, line.Plan.Currency, line.Price.UnitAmount, line.Price.Type));
                }
                var invoiceDto = new SubscriptionInvoiceDto(invoice.Created, invoice.InvoicePdf, lines);
                return invoiceDto;
            }catch(Exception)
            {
                return null;
            }
        }

        public async Task<string> CreateUrlForCustomerPortalSession(Tenant tenant, string returnUrl)
        {
            var session = await _sessionService.CreateAsync(new SessionCreateOptions()
            {
                ReturnUrl = returnUrl,
                Customer = tenant.SubscriptionCustomerId
            });
            return session.Url;
        }

        public async Task<string> CreateCustomer(string organization, string subdomain, string email)
        {
            try
            { 
                var createdCustomer = await _customerService.CreateAsync(new CustomerCreateOptions()
                {
                    Name = organization,
                    Email = email,
                    Metadata = new Dictionary<string, string>(){
                    {"Organization", organization},
                    {"Subdomain", subdomain }
                }
                });
                return createdCustomer.Id;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> UpdateCustomer(Tenant tenant)
        {
            var createdCustomer = await _customerService.UpdateAsync(tenant.SubscriptionCustomerId, new CustomerUpdateOptions()
            {
                Metadata = new Dictionary<string, string>(){
                    {"Organization", tenant.Name },
                    {"Domain", tenant.Domain },
                    {"Subdomain", tenant.Subdomain }
                }
            });
            return createdCustomer.Id;
        }

        public async Task<string> AddSubscription(Tenant tenant, SubscriptionPrice price, string cardToken = null, string couponId = null)
        {
            if (string.IsNullOrEmpty(tenant.SubscriptionCustomerId))
                throw new Exception("api.errors.invalidStripeCustomerId");

            if (string.IsNullOrEmpty(couponId) == false)
            {
                var coupon = await _couponService.GetAsync(couponId);
                if (coupon == null)
                {
                    throw new Exception("api.errors.invalidCoupon");
                }

                if (coupon.TimesRedeemed >= coupon.MaxRedemptions)
                {
                    throw new Exception("api.errors.couponMaxRedeems");
                }

                await _customerService.UpdateAsync(tenant.SubscriptionCustomerId, new CustomerUpdateOptions()
                {
                    Coupon = couponId
                });
            }

            if (price.Type == SubscriptionPriceType.OneTime)
            {
                await _invoiceItemService.CreateAsync(new InvoiceItemCreateOptions()
                {
                    Customer = tenant.SubscriptionCustomerId,
                    Price = price.ServiceId,
                });
                var invoice = await _invoiceService.CreateAsync(new InvoiceCreateOptions()
                {
                    Customer = tenant.SubscriptionCustomerId,
                });
                //tenant.AddProduct(price.Id);
                if (price.Price > 0)
                {
                    await _invoiceService.PayAsync(invoice.Id, new InvoicePayOptions());
                }
                return "";
            }
            else
            {
                var subscription = new SubscriptionCreateOptions()
                {
                    Customer = tenant.SubscriptionCustomerId,
                    Items = new List<SubscriptionItemOptions>()
                    {
                        new SubscriptionItemOptions()
                        {
                            Price = price.ServiceId,
                            Quantity = 1,
                        }
                    },
                    TrialFromPlan = price.TrialDays > 0
                };

                var createdSuscription = await _subscriptionService.CreateAsync(subscription);
                return createdSuscription.Id;
            }
        }

        public async Task DeleteCustomer(Tenant tenant)
        {
            await _customerService.DeleteAsync(tenant.SubscriptionCustomerId);
        }

        public async Task CancelSubscription(Tenant tenant, Guid? subscriptionId = null)
        {
            var subscription = tenant.GetCurrentSubscription();
             if (subscription == null)
                throw new Exception("api.errors.noActiveSubscriptions");

            try
            {
                if (subscription.SubscriptionPrice.Price > 0)
                {
                    var existing = await _subscriptionService.GetAsync(subscription.SubscriptionServiceId);
                    if (existing != null)
                        await _subscriptionService.CancelAsync(existing.Id, new SubscriptionCancelOptions());
                }
                subscription.CancelledAt = DateTime.Now;
                subscription.Active = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task AddCardTokenToCustomer(Tenant tenant, string cardToken)
        {
            await _customerService.UpdateAsync(tenant.SubscriptionCustomerId, new CustomerUpdateOptions
            {
                Source = cardToken
            });
        }

        public async Task<string> CreateCardToken(SubscriptionCreateCardTokenRequest card)
        {
            Token token = await _tokenService.CreateAsync(new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = card.Number,
                    ExpMonth = card.ExpiryMonth,
                    ExpYear = card.ExpiryYear,
                    Cvc = card.Cvc,
                },
            });
            return token.Id;
        }

        public async Task<IEnumerable<SubscriptionProduct>> GetProducts()
        {
            var list = await _productService.ListAsync();
            var productsDto = new List<SubscriptionProduct>();
            foreach (var product in list)
            {
                productsDto.Add(new SubscriptionProduct()
                {
                    ServiceId = product.Id,
                    Title = product.Name,
                    Description = product.Description,
                    Active = product.Active
                });
            }
            return productsDto;
        }

        public async Task<IEnumerable<SubscriptionPriceDto>> GetPrices(string productId = null)
        {
            var list = await _priceService.ListAsync(new PriceListOptions()
            {
                Product = productId
            });
            var pricesDto = new List<SubscriptionPriceDto>();
            foreach (var price in list)
            {
                if (price.UnitAmount != null)
                    pricesDto.Add(new SubscriptionPriceDto()
                    {
                        ServiceId = price.Id,
                        Type = price.Type == "one_time"
                            ? SubscriptionPriceType.OneTime
                            : SubscriptionPriceType.Recurring,
                        BillingPeriod = GetBillingPeriod(price),
                        Price = price.UnitAmount.Value.GetDecimal(),
                        Currency = price.Currency
                    });
            }
            return pricesDto;
        }

        private static SubscriptionBillingPeriod GetBillingPeriod(Price price)
        {
            if (price.Recurring != null)
            {
                return price.Recurring.Interval switch
                {
                    "day" => SubscriptionBillingPeriod.Daily,
                    "week" => SubscriptionBillingPeriod.Weekly,
                    "month" => SubscriptionBillingPeriod.Monthly,
                    "year" => SubscriptionBillingPeriod.Yearly,
                    _ => SubscriptionBillingPeriod.Once,
                };
            }
            else
            {
                return SubscriptionBillingPeriod.Once;
            }
        }

        public async Task<SubscriptionProductDto> SyncProduct(SubscriptionProductDto product)
        {
            if (string.IsNullOrEmpty(product.ServiceId))
            {
                var created = await _productService.CreateAsync(new ProductCreateOptions()
                {
                    Name = product.Title,
                    Description = product.Description,
                    Active = product.Active,
                    Metadata = new Dictionary<string, string>()
                    {
                        {"Tier", product.Tier.ToString()}
                    }
                });
                product.ServiceId = created.Id;
            }
            else
            {
                await _productService.UpdateAsync(product.ServiceId, new ProductUpdateOptions()
                {
                    Name = product.Title,
                    Description = product.Description,
                    Active = product.Active,
                    Metadata = new Dictionary<string, string>()
                    {
                        {"Tier", product.Tier.ToString()}
                    }
                });
            }
            var existingProduct = await _productService.GetAsync(product.ServiceId);
            if (existingProduct == null)
                throw new Exception("Invalid product: " + product.ServiceId);
            
            if (product.Prices != null)
            {
                foreach (var price in product.Prices)
                {
                    await SyncPrice(price, product);
                }
            }

            if (product.Features != null && product.Features.Count > 0)
            {
                try
                {
                    var metadata = new Dictionary<string, string>();
                    foreach (var feature in product.Features)
                    {
                        if (metadata.ContainsKey(feature.Key) == false)
                        {
                            metadata.Add(feature.Key, feature.Value);
                        }
                    }
                    _productService.Update(product.ServiceId, new ProductUpdateOptions()
                    {
                        Metadata = metadata
                    });
                }
                catch
                {
                    // ignored
                }
            }
            return product;
        }

        public async Task<SubscriptionPriceDto> SyncPrice(SubscriptionPriceDto price, SubscriptionProductDto product)
        {
            if (string.IsNullOrEmpty(price.ServiceId))
            {
                var priceCreateOptions = new PriceCreateOptions()
                {
                    UnitAmount = Convert.ToInt64(price.Price * 100),
                    Currency = price.Currency,
                    Product = product.ServiceId,
                    Active = price.Active
                };
                if (price.Type == SubscriptionPriceType.Recurring)
                {
                    var interval = GetInterval(price.BillingPeriod);
                    priceCreateOptions.Recurring = new PriceRecurringOptions()
                    {
                        Interval = interval,
                        TrialPeriodDays = price.TrialDays
                    };
                }
                var created = await _priceService.CreateAsync(priceCreateOptions);
                price.ServiceId = created.Id;
            }
            else
            {
                var priceUpdateOptions = new PriceUpdateOptions()
                {
                    Active = price.Active,
                };
                await _priceService.UpdateAsync(price.ServiceId, priceUpdateOptions);
            }
            var existingPrice = await _priceService.GetAsync(price.ServiceId);
            if (existingPrice == null)
                throw new Exception("Invalid price: " + price.ServiceId);
            
            return price;
        }

        public async Task DeleteProduct(SubscriptionProductDto product)
        {
            if (string.IsNullOrEmpty(product.ServiceId) == false)
            {
                await _productService.UpdateAsync(product.ServiceId, new ProductUpdateOptions() { Active = false });
            }
        }

        public async Task DeletePrice(SubscriptionPriceDto price)
        {
            if (string.IsNullOrEmpty(price.ServiceId) == false)
            {
                await _priceService.UpdateAsync(price.ServiceId, new PriceUpdateOptions() { Active = false });
            }
        }

        private string GetInterval(SubscriptionBillingPeriod billingPeriod)
        {
            return billingPeriod switch
            {
                SubscriptionBillingPeriod.Once => "once",
                SubscriptionBillingPeriod.Daily => "day",
                SubscriptionBillingPeriod.Weekly => "week",
                SubscriptionBillingPeriod.Monthly => "month",
                SubscriptionBillingPeriod.Yearly => "year",
                _ => "",
            };
        }
    }
}
