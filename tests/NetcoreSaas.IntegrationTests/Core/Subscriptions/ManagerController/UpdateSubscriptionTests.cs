using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Domain.Helpers;
using Xunit;

namespace NetcoreSaas.IntegrationTests.Core.Subscriptions.ManagerController
{
    public class UpdateSubscriptionTests : SubscriptionTestBase
    {
        [Fact]
        public async Task UpdateSubscription_AlreadySubscribed_ReturnsBadRequest()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1", TestPriceType.Monthly);
            AuthenticateWithUser(newUser.Email, "password");

            var selectedSubscription = GetExistingPrice(TestPriceType.Monthly);

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionManager.UpdateSubscription,
                new SelectedSubscriptionRequest(selectedSubscription.Id));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateSubscription_FromFreeToMonthly_ReturnsNewSubscription()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");

            var selectedSubscription = GetExistingPrice(TestPriceType.Monthly);
            TestClient.PostAsync(ApiCoreRoutes.SubscriptionManager.UpdateCardToken.Replace("{cardToken}", CreateTestCard()), null).Wait();

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionManager.UpdateSubscription,
                new SelectedSubscriptionRequest(selectedSubscription.Id));
            var responseContent = await response.Content.ReadAsAsync<SubscriptionGetCurrentResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.ActiveProduct.SubscriptionPriceId.Should().Be(selectedSubscription.Id);
        }

        [Fact]
        public async Task UpdateSubscription_FromFreeToOneTimeWithoutPaymentMethod_ReturnsBadRequest()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");

            var selectedSubscription = GetExistingPrice(TestPriceType.OneTime);

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionManager.UpdateSubscription,
                new SelectedSubscriptionRequest(selectedSubscription.Id));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateSubscription_FromFreeToOneTimeWithPaymentMethod_ReturnsNewSubscription()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");

            var selectedSubscription = GetExistingPrice(TestPriceType.OneTime);
            TestClient.PostAsync(ApiCoreRoutes.SubscriptionManager.UpdateCardToken.Replace("{cardToken}", CreateTestCard()), null!).Wait();

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionManager.UpdateSubscription,
                new SelectedSubscriptionRequest(selectedSubscription.Id));
            var responseContent = await response.Content.ReadAsAsync<SubscriptionGetCurrentResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.ActiveProduct.SubscriptionPriceId.Should().Be(selectedSubscription.Id);
        }

        [Fact]
        public async Task UpdateSubscription_FromMonthlyToFree_ReturnsNewSubscription()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1", TestPriceType.Monthly);
            AuthenticateWithUser(newUser.Email, "password");

            var selectedSubscription = GetExistingPrice(TestPriceType.Free);
            TestClient.PostAsync(ApiCoreRoutes.SubscriptionManager.UpdateCardToken.Replace("{cardToken}", CreateTestCard()), null).Wait();

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionManager.UpdateSubscription,
                new SelectedSubscriptionRequest(selectedSubscription.Id));
            var responseContent = await response.Content.ReadAsAsync<SubscriptionGetCurrentResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.ActiveProduct.SubscriptionPriceId.Should().Be(selectedSubscription.Id);
        }

        [Fact]
        public async Task UpdateSubscription_FromOneTimeToMonthly_Returns2Products()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var newUser = CreateTestUser("user@tenant1.com", "password", "Tenant 1", TestPriceType.OneTime);
            AuthenticateWithUser(newUser.Email, "password");

            var selectedSubscription = GetExistingPrice(TestPriceType.Monthly);

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionManager.UpdateSubscription,
                new SelectedSubscriptionRequest(selectedSubscription.Id));
            var responseContent = await response.Content.ReadAsAsync<SubscriptionGetCurrentResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.MyProducts.Should().HaveCount(2);
        }
    }
}
