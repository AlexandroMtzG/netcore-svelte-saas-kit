using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Application.Contracts.Core.Tenants;
using NetcoreSaas.Application.Contracts.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Tenants;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.IntegrationTests.Core.Subscriptions;
using Xunit;

namespace NetcoreSaas.IntegrationTests.Core.Users
{
    public class UserControllerTests : SubscriptionTestBase
    {
        [Fact]
        public async Task GetAll_AsTenant_ReturnsForbidden()
        {
            // Arrange
            AuthenticateAsTenant();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.User.AdminGetAll);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetAll_AsAdmin_ReturnsAllUsers()
        {
            // Arrange
            AuthenticateAsAdmin();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.User.AdminGetAll);
            var responseContent = await response.Content.ReadAsAsync<List<UserDto>>();

            // Assert
            responseContent.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task GetUser_AsAnotherTenant_ReturnsBadRequest()
        {
            // Arrange
            var userTenantA = CreateTestUser("user@tenantA.com", "password", "Tenant A");
            var userTenantB = CreateTestUser("user@tenantB.com", "password", "Tenant B");

            AuthenticateWithUser(userTenantA.Email, "password");

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.User.GetUser.Replace("{id}", userTenantB.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetUser_AsTenantWithUserAsItsMember_ReturnsUser()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.User.GetUser.Replace("{id}", newUser.Id.ToString()));
            var user = await response.Content.ReadAsAsync<UserDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            user.Email.Should().Be(newUser.Email);
        }

        [Fact]
        public async Task GetUser_AsAdmin_ReturnsAnyUser()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateAsAdmin();

            // Acts
            var response = await TestClient.GetAsync(ApiCoreRoutes.User.GetUser.Replace("{id}", newUser.Id.ToString()));
            var user = await response.Content.ReadAsAsync<UserDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            user.Email.Should().Be(newUser.Email);
        }
        
        [Fact]
        public async Task GetCurrent_NotLogged_ReturnsUnauthorized()
        {
            // Arrange
            CreateTestUser("user@email.com", "password", "Tenant A");

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.User.GetCurrent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        
        [Fact]
        public async Task GetCurrent_Logged_ReturnsUser()
        {
            // Arrange
            var user = CreateTestUser("user@email.com", "password", "Tenant A");
            AuthenticateWithUser(user.Email, "password");

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.User.GetCurrent);
            var current = await response.Content.ReadAsAsync<UserLoggedResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            current.User.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task Update_AsUser_ReturnsUpdatedUser()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var updateRequest = new UserUpdateRequest()
            {
                FirstName = "Name",
                LastName = "Last name",
                Phone = "111 111 111"
            };
            var response = await TestClient.PutAsJsonAsync(ApiCoreRoutes.User.Update.Replace("{id}", newUser.Id.ToString()), updateRequest);
            var updatedUser = await response.Content.ReadAsAsync<UserDto>();

            // Assert
            updatedUser.FirstName.Should().Be(updateRequest.FirstName);
            updatedUser.LastName.Should().Be(updateRequest.LastName);
            updatedUser.Phone.Should().Be(updateRequest.Phone);
        }

        [Fact]
        public async Task UpdateAvatar_WithValidImage_ReturnsUpdatedUser()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");
            var newAvatar = "data:image/jpeg;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeBAMAAADJHrORAAAAG1BMVEXMzMyWlpa3t7eqqqrFxcW+vr6xsbGjo6OcnJwtaz+fAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAOElEQVQYlWNgGHaAyURdAUaC+U4tDTASDEwFBNhUQSSEy+rW0sBSACIhfDYjdQU2VRBJLxfTEAAAv8sIm/VDSJMAAAAASUVORK5CYII=";

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.User.UpdateAvatar, 
                new UserUpdateAvatarRequest()
                {
                    Avatar = newAvatar
                });
            var updatedUser = await response.Content.ReadAsAsync<UserDto>();

            // Assert
            newUser.Avatar.Should().NotBe(newAvatar);
            updatedUser.Avatar.Should().Be(newAvatar);
        }

        [Fact]
        public async Task UpdatePassword_WithInvalidCurrentPassord_ReturnsBadRequest()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");
            var newPassword = "password2";

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.User.UpdatePassword.Replace("{id}", newUser.Id.ToString()),
                new UserUpdatePasswordRequest()
                {
                    PasswordCurrent = "invalidCurrentPassword",
                    PasswordNew = newPassword,
                    PasswordConfirm = newPassword
                });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdatePassword_WithInvalidNewPasswordConfirm_ReturnsBadRequest()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");
            var newPassword = "password2";

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.User.UpdatePassword.Replace("{id}", newUser.Id.ToString()),
                new UserUpdatePasswordRequest()
                {
                    PasswordCurrent = "password",
                    PasswordNew = newPassword,
                    PasswordConfirm = "invalidNewPasswordConfirm"
                });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdatePassword_WithValidOldAndNewPassord_ReturnsOk()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");
            var newPassword = "password2";

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.User.UpdatePassword.Replace("{id}", newUser.Id.ToString()), 
                new UserUpdatePasswordRequest()
                {
                    PasswordCurrent = "password",
                    PasswordNew = newPassword,
                    PasswordConfirm = newPassword
                });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateDefaultTenant_NonExistingTenant_ReturnsNotFound()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.User.UpdateDefaultTenant
                .Replace("{userId}", newUser.Id.ToString())
                .Replace("{tenantId}", Guid.NewGuid().ToString()), new { });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateDefaultTenant_AsUserWith3Tenants_ReturnsTenant()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");
            await TestClient.PostAsJsonAsync(ApiCoreRoutes.Tenant.Create, new TenantCreateRequest()
            {
                Name = "Tenant 2",
                SelectedSubscription = new SelectedSubscriptionRequest(GetExistingPrice(TestPriceType.Free).Id)
            });
            await TestClient.PostAsJsonAsync(ApiCoreRoutes.Tenant.Create, new TenantCreateRequest()
            {
                Name = "Tenant 3",
                SelectedSubscription = new SelectedSubscriptionRequest(GetExistingPrice(TestPriceType.Free).Id)
            });
            var myTenants = await (await TestClient.GetAsync(ApiCoreRoutes.Tenant.GetAll))
                .Content.ReadAsAsync<IEnumerable<TenantDto>>();
            var tenant2 = myTenants.SingleOrDefault(f => f.Name == "Tenant 2");

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.User.UpdateDefaultTenant
                .Replace("{userId}", newUser.Id.ToString())
                .Replace("{tenantId}", tenant2?.Id.ToString()), new { });
            var responseContent = await response.Content.ReadAsAsync<UserLoggedResponse>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.User.CurrentTenant.Name.Should().Be(tenant2?.Name);
        }

        [Fact]
        public async Task Delete_AnotherUser_ReturnsForbidden()
        {
            // Arrange
            var newUserA = CreateTestUser("user@tenantA.com", "password", "Tenant A");
            var newUserB = CreateTestUser("user@tenantB.com", "password", "Tenant B");
            AuthenticateWithUser(newUserA.Email, "password");

            // Act
            var response = await TestClient.DeleteAsync(ApiCoreRoutes.User.AdminDelete.Replace("{id}", newUserB.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Admin_ReturnsOk()
        {
            // Arrange
            var newUserA = CreateTestUser("user@tenantA.com", "password", "Tenant A");
            AuthenticateAsAdmin();

            // Act
            var response = await TestClient.DeleteAsync(ApiCoreRoutes.User.AdminDelete.Replace("{id}", newUserA.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteMe_AsCurrentUser_ReturnsOk()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.DeleteAsync(ApiCoreRoutes.User.DeleteMe);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
