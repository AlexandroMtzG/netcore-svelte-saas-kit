using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;
using NetcoreSaas.Domain.Helpers;
using Xunit;

namespace NetcoreSaas.IntegrationTests.Core.Subscriptions.ManagerController
{
    public class ManagerTests : SubscriptionTestBase
    {
        [Fact]
        public async Task GetCurrentSubscription_AsAdmin_ReturnsFakeSubscriptionWithHighestTier()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var highestProductTier = TestProducts.Max(f => f.Tier);

            AuthenticateAsAdmin();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.SubscriptionManager.GetCurrentSubscription);
            var subscription = await response.Content.ReadAsAsync<SubscriptionGetCurrentResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            subscription.MyProducts.First().SubscriptionPrice.Should().NotBeNull();
            subscription.MyProducts.First().SubscriptionPrice.SubscriptionProduct.Tier.Should().Be(highestProductTier);
        }

        [Fact]
        public async Task GetCurrentSubscription_AsTenantAfterRegistering_ReturnsValidSubscription()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1", TestPriceType.Monthly);
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.SubscriptionManager.GetCurrentSubscription);
            var subscription = await response.Content.ReadAsAsync<SubscriptionGetCurrentResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            subscription.MyProducts.Should().HaveCountGreaterThan(0);
            subscription.MyProducts.First().Active.Should().BeTrue();
        }

        [Fact]
        public async Task GetUpcomingInvoice_WithActiveRecurringSubscription_ReturnsInvoice()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1", TestPriceType.Monthly);
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.SubscriptionManager.GetUpcomingInvoice);
            var invoice = await response.Content.ReadAsAsync<SubscriptionInvoiceDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            invoice.Should().NotBeNull();
            invoice.Lines.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task GetUpcomingInvoice_WithCancelledRecurringSubscription_ReturnsNoContent()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1", TestPriceType.Monthly);
            AuthenticateWithUser(newUser.Email, "password");

            await TestClient.PostAsync(ApiCoreRoutes.SubscriptionManager.CancelSubscription, null);

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.SubscriptionManager.GetUpcomingInvoice);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetCoupon_WithNonExistingCoupon_ReturnsBadRequest()
        {
            // Arrange
            SetupTestProductsWithPrices();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.SubscriptionManager.GetCoupon
                .Replace("{couponId}", Guid.NewGuid().ToString())
                .Replace("{currency}", DefaultCurrency));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
