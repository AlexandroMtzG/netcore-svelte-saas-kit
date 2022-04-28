using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NetcoreSaas.Application.Contracts.Core.Tenants;
using NetcoreSaas.Application.Dtos.Core.Tenants;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.IntegrationTests.Core.Subscriptions;
using Xunit;

namespace NetcoreSaas.IntegrationTests.Core.Tenants
{
    public class TenantControllerTests : SubscriptionTestBase
    {
        [Fact]
        public async Task AdminGetAll_AsAdmin_ReturnAllTenants()
        {
            // Arrange
            AuthenticateAsAdmin();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.Tenant.GetAll);
            var tenants = await response.Content.ReadAsAsync<List<TenantDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            tenants.Should().NotBeEmpty();
        }

        [Fact]
        public async Task AdminGetAll_AsTenants_ReturnForbidden()
        {
            // Arrange
            AuthenticateAsTenant();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.Tenant.AdminGetAll);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetAll_AsTenantWith1Organization_Return1Tenant()
        {
            // Arrange
            AuthenticateAsAdmin();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.Tenant.GetAll);
            var tenants = await response.Content.ReadAsAsync<IEnumerable<TenantDto>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (tenants).Should().HaveCount(1);
        }
        
        [Fact]
        public async Task GetCurrent_NotLogged_ReturnsUnauthorized()
        {
            // Arrange

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.Tenant.GetCurrent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        
        [Fact]
        public async Task GetCurrent_Logged_ReturnsTenant()
        {
            // Arrange
            var user = CreateTestUser("user@email.com", "password", "Tenant A");
            AuthenticateWithUser(user.Email, "password");

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.Tenant.GetCurrent);
            var current = await response.Content.ReadAsAsync<TenantDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            current.Name.Should().Be(user.CurrentTenant.Name);
        }

        [Fact]
        public async Task Update_NewName_ReturnsOk()
        {
            // Arrange
            AuthenticateAsTenant();

            // Act
            CurrentTenant.Name = "New Organization/Tenant Name";
            var response = await TestClient.PutAsJsonAsync(ApiCoreRoutes.Tenant.Update.Replace("{id}", CurrentTenant.Id.ToString()), CurrentTenant);
            var tenant = await response.Content.ReadAsAsync<TenantDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            tenant.Name.Should().Be(CurrentTenant.Name);
        }

        [Fact]
        public async Task UpdateImage_UpdatingLogo_ReturnsOk()
        {
            // Arrange
            AuthenticateAsTenant();

            // Act
            var newLogo = "data:image/jpeg;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeBAMAAADJHrORAAAAG1BMVEXMzMyWlpa3t7eqqqrFxcW+vr6xsbGjo6OcnJwtaz+fAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAOElEQVQYlWNgGHaAyURdAUaC+U4tDTASDEwFBNhUQSSEy+rW0sBSACIhfDYjdQU2VRBJLxfTEAAAv8sIm/VDSJMAAAAASUVORK5CYII=";
            var response = await TestClient.PutAsJsonAsync(ApiCoreRoutes.Tenant.UpdateImage.Replace("{id}", CurrentTenant.Id.ToString()), 
                new TenantUpdateImageRequest()
                {
                    Type = "logo",
                    Logo = newLogo
                });
            var tenant = await response.Content.ReadAsAsync<TenantDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            tenant.Logo.Should().Be(newLogo);
        }

        //[Fact]
        //public async Task Delete_AsTenantAdmin_ReturnsUnauthorized()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
