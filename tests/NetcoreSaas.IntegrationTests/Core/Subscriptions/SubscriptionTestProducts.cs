using System.Collections.Generic;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;
using NetcoreSaas.Domain.Enums.Core.Subscriptions;

namespace NetcoreSaas.IntegrationTests.Core.Subscriptions
{
    public static class SubscriptionTestProducts
    {
        public static List<SubscriptionProductDto> GetMyTestProducts(int tiers, string currency)
        {
            var products = new List<SubscriptionProductDto>();
            for (var tier = 1; tier <= tiers; tier++)
            {
                decimal monthlyPrice = tier * 10;
                // it will create 4 prices, free, monthly, yearly with 2 months discount and one time with 4 years price at 50%
                products.Add(CreateTestProduct(tier, currency, monthlyPrice));
            }
            return products;
        }

        private static SubscriptionProductDto CreateTestProduct(int tier, string currency, decimal basePrice)
        {
            var testProduct = new SubscriptionProductDto()
            {
                ServiceId = "",
                Title = "Test product " + tier,
                Active = true,
                Tier = tier
            };
            // Free Subscription
            testProduct.Prices.Add(new SubscriptionPriceDto()
            {
                ServiceId = "",
                Currency = currency,
                Price = 0,
                Type = SubscriptionPriceType.Recurring,
                BillingPeriod = SubscriptionBillingPeriod.Monthly,
                Active = true,
            });
            // Monthly Subscription
            testProduct.Prices.Add(new SubscriptionPriceDto()
            {
                ServiceId = "",
                Currency = currency,
                Price = basePrice,
                Type = SubscriptionPriceType.Recurring,
                BillingPeriod = SubscriptionBillingPeriod.Monthly,
                Active = true,
                TrialDays = 14,
            });
            // Yearly Subscription
            testProduct.Prices.Add(new SubscriptionPriceDto()
            {
                ServiceId = "",
                Currency = currency,
                Price = basePrice * 10,
                Type = SubscriptionPriceType.Recurring,
                BillingPeriod = SubscriptionBillingPeriod.Yearly,
                Active = true,
                TrialDays = 14,
            });
            // One time payment Subscription
            testProduct.Prices.Add(new SubscriptionPriceDto()
            {
                ServiceId = "",
                Currency = currency,
                Price = basePrice * 12 * 4 / 2,
                Type = SubscriptionPriceType.OneTime,
                BillingPeriod = SubscriptionBillingPeriod.Once,
                Active = true,
            });

            // Features
            testProduct.Features.Add(new SubscriptionFeatureDto()
            {
                Order = 1,
                Key = "maxNumberOfUsers",
                Value = (tier * 3).ToString(),
                TranslateInFrontend = true
            });
            testProduct.Features.Add(new SubscriptionFeatureDto()
            {
                Order = 1,
                Key = "Storage",
                Value = "Up to 1 GB",
                TranslateInFrontend = false
            });

            return testProduct;
        }
    }
}
