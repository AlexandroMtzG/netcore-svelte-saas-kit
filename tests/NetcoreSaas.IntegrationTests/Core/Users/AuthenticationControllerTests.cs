using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Application.Contracts.Core.Users;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Infrastructure;
using NetcoreSaas.IntegrationTests.Core.Subscriptions;
using Xunit;

namespace NetcoreSaas.IntegrationTests.Core.Users
{
    public class AuthenticationControllerTests : SubscriptionTestBase
    {
        [Fact]
        public async Task Impersonate_ExistingUser_ReturnsAuthenticatedUser()
        {
            // Arrange
            SetupTestProductsWithPrices();

            var user = new UserRegisterRequest()
            {
                Email = "valid@user.com",
                Password = "password",
                WorkspaceName = "Tenant 1",
                SelectedSubscription = new SelectedSubscriptionRequest(GetExistingPrice(TestPriceType.Free).Id)
            };
            var registeredUserResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user);
            var registeredUser = await registeredUserResponse.Content.ReadAsAsync<UserLoggedResponse>();

            AuthenticateAsAdmin();

            // Act
            var response = await TestClient.PostAsync(ApiCoreRoutes.Authentication.AdminImpersonate
                .Replace("{userId}", registeredUser.User.Id.ToString()), null!);
            var responseContent = await response.Content.ReadAsAsync<UserLoggedResponse>();
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.User.Email.Should().Be(user.Email.ToLower().Trim());
            responseContent.User.Token.Should().NotBeEmpty();
        }
        [Fact]
        public async Task Login_NonExistingUser_ReturnsBadRequest()
        {
            // Arrange

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Login, new UserLoginRequest()
            {
                Email = "nonexisting@user.com",
                Password = "abc",
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_ExistingUser_ReturnsAuthenticatedUser()
        {
            // Arrange
            ProjectConfiguration.GlobalConfiguration.RequiresVerification = false;

            // Act
            var user = new UserLoginRequest()
            {
                Email = " admin@testing.com ",
                Password = "password",
            };
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Login, user);

            // Assert
            var responseContent = await response.Content.ReadAsAsync<UserLoggedResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.User.Email.Should().Be(user.Email.ToLower().Trim());
            responseContent.Token.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Register_WithInvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            SetupTestProductsWithPrices();

            // Act
            var user = new UserRegisterRequest()
            {
                Email = "invalid@email",
                Password = "password",
                WorkspaceName = "Tenant 1",
                SelectedSubscription = new SelectedSubscriptionRequest(GetExistingPrice(TestPriceType.Free).Id)
            };
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_WithValidEmailAndInvalidPrice_ReturnsBadRequest()
        {
            // Arrange
            SetupTestProductsWithPrices();

            // Act
            var user = new UserRegisterRequest()
            {
                Email = "valid@user.com",
                Password = "password",
                WorkspaceName = "Tenant 1",
                SelectedSubscription = new SelectedSubscriptionRequest(Guid.Empty)
            };
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_WithValidEmailAndValidPrice_ReturnsAuthenticatedUser()
        {
            // Arrange
            SetupTestProductsWithPrices();
            ProjectConfiguration.GlobalConfiguration.RequiresVerification = false;

            // Act
            var user = new UserRegisterRequest()
            {
                Email = "valid@user.com",
                Password = "password",
                WorkspaceName = "Tenant 1",
                SelectedSubscription = new SelectedSubscriptionRequest(GetExistingPrice(TestPriceType.Free).Id)
            };
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user);
            var responseContent = await response.Content.ReadAsAsync<UserLoggedResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.User.Email.Should().Be(user.Email.ToLower().Trim());
            responseContent.User.Token.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Register_WithValidEmailAndValidPriceAndRequieresVerification_ReturnsOk()
        {
            // Arrange
            SetupTestProductsWithPrices();
            ProjectConfiguration.GlobalConfiguration.RequiresVerification = true;

            // Act
            var user = new UserRegisterRequest()
            {
                Email = "valid@user.com",
                Password = "password",
                WorkspaceName = "Tenant 1",
                SelectedSubscription = new SelectedSubscriptionRequest(GetExistingPrice(TestPriceType.Free).Id)
            };
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Register_WithInvalidSubscription_ReturnsBadRequest()
        {
            // Arrange
            SetupTestProductsWithPrices();

            // Act
            var user = new UserRegisterRequest()
            {
                Email = "valid@user.com",
                Password = "password",
                WorkspaceName = "Tenant 1",
                SelectedSubscription = new SelectedSubscriptionRequest(Guid.Empty)
            };
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_WithMonthlySubscriptionPrice_ReturnsOk()
        {
            // Arrange
            SetupTestProductsWithPrices();
            

            // Act
            var user = new UserRegisterRequest()
            {
                Email = "valid@user.com",
                Password = "password",
                WorkspaceName = "Tenant 1",
                SelectedSubscription = new SelectedSubscriptionRequest(GetExistingPrice(TestPriceType.Monthly).Id)
            };
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user);
            var responseContent = await response.Content.ReadAsAsync<UserLoggedResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Register_WithYearlySubscriptionPrice_ReturnsOk()
        {
            // Arrange
            SetupTestProductsWithPrices();
            
            // Act
            var user = new UserRegisterRequest()
            {
                Email = "valid@user.com",
                Password = "password",
                WorkspaceName = "Tenant 1",
                SelectedSubscription = new SelectedSubscriptionRequest(GetExistingPrice(TestPriceType.Yearly).Id)
            };
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user);
            var responseContent = await response.Content.ReadAsAsync<UserLoggedResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Token.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Register_WithOneTimePriceWithoutPaymentMethod_ReturnsBadRequest()
        {
            // Arrange
            SetupTestProductsWithPrices();
            
            // Act
            var user = new UserRegisterRequest()
            {
                Email = "valid@user.com",
                Password = "password",
                WorkspaceName = "Tenant 1",
                SelectedSubscription = new SelectedSubscriptionRequest(GetExistingPrice(TestPriceType.OneTime).Id)
            };
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_WithOneTimePriceWithValidPaymentMethod_ReturnsOk()
        {
            // Arrange
            SetupTestProductsWithPrices();
            
            var tokenResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionManager.CreateCardToken, new SubscriptionCreateCardTokenRequest()
            {
                Number = "4242424242424242",
                ExpiryMonth = 1,
                ExpiryYear = 2024,
                Cvc = "424"
            });
            var cardToken = await tokenResponse.Content.ReadAsStringAsync();

            // Act
            var user = new UserRegisterRequest()
            {
                Email = "valid@user.com",
                Password = "password",
                WorkspaceName = "Tenant 1",
                SelectedSubscription = new SelectedSubscriptionRequest(GetExistingPrice(TestPriceType.OneTime).Id, cardToken)
            };
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user);
            var userAuthenticatedOnCreatedTenant = await response.Content.ReadAsAsync<UserLoggedResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            userAuthenticatedOnCreatedTenant.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Verify_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            SetupTestProductsWithPrices();
            ProjectConfiguration.GlobalConfiguration.RequiresVerification = true;
            var user = new UserRegisterRequest()
            {
                Email = "valid@user.com",
                Password = "password",
                WorkspaceName = "Tenant 1",
                SelectedSubscription = new SelectedSubscriptionRequest(GetExistingPrice(TestPriceType.Free).Id)
            };
            await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user);

            // Act
            var verificationRequest = new UserVerifyRequest()
            {
                Email = user.Email,
                Password = user.Password,
                Token = Guid.NewGuid()
            };
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Verify, verificationRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Reset_ExistingUser_ReturnsOk()
        {
            // Arrange
            SetupTestProductsWithPrices();
            var user = new UserRegisterRequest()
            {
                Email = "valid@user.com",
                Password = "password",
                WorkspaceName = "Tenant 1",
                SelectedSubscription = new SelectedSubscriptionRequest(GetExistingPrice(TestPriceType.Free).Id)
            };
            await TestClient.PostAsJsonAsync(ApiCoreRoutes.Authentication.Register, user);

            // Act
            var response = await TestClient.PostAsync(ApiCoreRoutes.Authentication.Reset.Replace("{email}", user.Email), null!);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
