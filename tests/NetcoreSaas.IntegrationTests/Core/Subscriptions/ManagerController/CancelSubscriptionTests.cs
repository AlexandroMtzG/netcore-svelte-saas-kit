using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Domain.Helpers;
using Xunit;

namespace NetcoreSaas.IntegrationTests.Core.Subscriptions.ManagerController
{
    public class CancelSubscriptionTests : SubscriptionTestBase
    {
        [Fact]
        public async Task CancelSubscription_ActiveSubscription_ReturnsOk()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.PostAsync(ApiCoreRoutes.SubscriptionManager.CancelSubscription, null);
            var responseContent = response.Content.ReadAsAsync<SubscriptionGetCurrentResponse>().Result;

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.MyProducts.Should().HaveCount(0);
        }

        [Fact]
        public async Task CancelSubscription_InactiveSubscription_ReturnsBadRequest()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");
            await TestClient.PostAsync(ApiCoreRoutes.SubscriptionManager.CancelSubscription, null);

            // Act
            var response = await TestClient.PostAsync(ApiCoreRoutes.SubscriptionManager.CancelSubscription, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CancelSubscription_WithActiveMonthlySubscription_ReturnsOk()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1", TestPriceType.Monthly);
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.PostAsync(ApiCoreRoutes.SubscriptionManager.CancelSubscription, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CancelSubscription_WithOneTimePaymentSubscription_ReturnsBadRequest()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1", TestPriceType.OneTime);
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.PostAsync(ApiCoreRoutes.SubscriptionManager.CancelSubscription, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}