using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Application.Contracts.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Domain.Enums.Core.Subscriptions;
using NetcoreSaas.Domain.Helpers;

namespace NetcoreSaas.IntegrationTests.Core.Subscriptions
{
    public class SubscriptionTestBase : TestBase
    {
        public enum TestPriceType { Free, Monthly, Yearly, OneTime }
        protected const string DefaultCurrency = "usd";
        protected SubscriptionTestBase()
        {
            ConfigureTestProductsWithPrices();
        }

        private void ConfigureTestProductsWithPrices()
        {
            TestProducts = SubscriptionTestProducts.GetMyTestProducts(3, DefaultCurrency);
        }

        // Creates test products
        protected void SetupTestProductsWithPrices()
        {
            var productResponse = TestClient.GetAsync(ApiCoreRoutes.SubscriptionProduct.GetAll).Result;
            if (productResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                AuthenticateAsAdmin();
                foreach (var product in TestProducts)
                {
                    TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.CreateProduct, product).Wait();
                }
                Logout();
                productResponse = TestClient.GetAsync(ApiCoreRoutes.SubscriptionProduct.GetAll).Result;
            }
            TestProducts = productResponse.Content.ReadAsAsync<List<SubscriptionProductDto>>().Result;
        }

        protected SubscriptionPriceDto GetExistingPrice(TestPriceType testPriceType, int productTier = 1)
        {
            SubscriptionPriceDto price = null;
            switch (testPriceType)
            {
                case TestPriceType.Free:
                    price = TestProducts.Where(f => f.Tier == productTier).SelectMany(f => f.Prices).OrderBy(f => f.Price).LastOrDefault(f => f.Price == 0);
                    break;
                case TestPriceType.Monthly:
                    price = TestProducts.Where(f => f.Tier == productTier).SelectMany(f => f.Prices).OrderBy(f => f.Price).LastOrDefault(f => f.BillingPeriod == SubscriptionBillingPeriod.Monthly);
                    break;
                case TestPriceType.Yearly:
                    price = TestProducts.Where(f => f.Tier == productTier).SelectMany(f => f.Prices).OrderBy(f => f.Price).LastOrDefault(f => f.BillingPeriod == SubscriptionBillingPeriod.Yearly);
                    break;
                case TestPriceType.OneTime:
                    price = TestProducts.Where(f => f.Tier == productTier).SelectMany(f => f.Prices).OrderBy(f => f.Price).LastOrDefault(f => f.BillingPeriod == SubscriptionBillingPeriod.Once);
                    break;
            }
            if (price == null)
                throw new Exception($"You did not configure a {testPriceType} price for tier {productTier} on ConfigureTestProductsWithPrices()");
            if (string.IsNullOrEmpty(price.ServiceId))
                throw new Exception("Call SetupTestProductsWithPrices() to create test prices on Database and Stripe");
            return price;
        }

        protected UserDto CreateTestUser(string email, string password, string organization, TestPriceType testPriceType = TestPriceType.Free, int productTier = 1)
        {
            SetupTestProductsWithPrices();

            string subscriptionCardToken = null;
            if (testPriceType == TestPriceType.OneTime)
            {
                subscriptionCardToken = CreateTestCard();
            }
            var user = new UserRegisterRequest()
            {
                Email = email,
                Password = password,
                WorkspaceName = organization,
                SelectedSubscription = new SelectedSubscriptionRequest(
                    GetExistingPrice(testPriceType, productTier).Id, 
                    subscriptionCardToken)
            };
            var registerResponse = TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user).Result;
            return registerResponse.Content.ReadAsAsync<UserLoggedResponse>().Result.User;
        }
        protected string CreateTestCard()
        {
            var tokenResponse = TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionManager.CreateCardToken, new SubscriptionCreateCardTokenRequest()
            {
                Number = "4242424242424242",
                ExpiryMonth = 1,
                ExpiryYear = 2024,
                Cvc = "424"
            }).Result;
            return tokenResponse.Content.ReadAsStringAsync().Result;
        }
    }
}
