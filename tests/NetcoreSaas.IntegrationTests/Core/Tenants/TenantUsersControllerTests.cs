using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NetcoreSaas.Application.Contracts.Core.Tenants;
using NetcoreSaas.Application.Contracts.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Tenants;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.IntegrationTests.Core.Subscriptions;
using Xunit;

namespace NetcoreSaas.IntegrationTests.Core.Tenants
{
    public class TenantUsersControllerTests : SubscriptionTestBase
    {
        [Fact]
        public async Task AdminGetAll_With2Users_ReturnsUsersWithTenants()
        {
            // Arrange
            AuthenticateAsAdmin();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.TenantUsers.AdminGetAll);
            var responseContent = await response.Content.ReadAsAsync<List<UserDto>>();

            // Assert
            responseContent.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAll_AsTenantWith4Users_Returns4Users()
        {
            // Arrange
            var newUser = CreateTestUser("user1@tenantA.com", "password", "Tenant A");
            AuthenticateWithUser(newUser.Email, "password");
            
            await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.InviteUser
                , new UserInviteRequest()
            {
                Email = "user2@tenantA.com",
                Role = TenantUserRole.Admin,
                Workspaces = newUser.Workspaces.Select(f=> new WorkspaceDto()
                {
                    Id = f.Id
                }).ToList()
            });
            await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.InviteUser
                , new UserInviteRequest()
            {
                Email = "user3@tenantA.com",
                Role = TenantUserRole.Member,
                Workspaces = newUser.Workspaces.Select(f=> new WorkspaceDto()
                {
                    Id = f.Id
                }).ToList()
            });
            await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.InviteUser
                , new UserInviteRequest()
            {
                Email = "user4@tenantA.com",
                Role = TenantUserRole.Guest,
                Workspaces = newUser.Workspaces.Select(f=> new WorkspaceDto()
                {
                    Id = f.Id
                }).ToList()
            });
            
            // Act
            var tenantUsersResponse = await TestClient.GetAsync(ApiCoreRoutes.TenantUsers.GetAll);
            var tenantUsers = await tenantUsersResponse.Content.ReadAsAsync<List<TenantUserDto>>();

            // Assert
            tenantUsers.Should().HaveCount(4);
            tenantUsers.Count(f => f.Role == TenantUserRole.Admin).Should().Be(1);
            tenantUsers.Count(f => f.Role == TenantUserRole.Owner).Should().Be(1);
            tenantUsers.Count(f => f.Role == TenantUserRole.Member).Should().Be(1);
            tenantUsers.Count(f => f.Role == TenantUserRole.Guest).Should().Be(1);
        }

        [Fact]
        public async Task GetUser()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.TenantUsers.Get
                .Replace("{id}", newUser.MembershipFromCurrentTenant().Id.ToString()));
            var responseContent = await response.Content.ReadAsAsync<TenantUserDto>();

            // Assert
            responseContent.User.Id.Should().Be(newUser.Id);
            responseContent.Tenant.Id.Should().Be(newUser.CurrentTenant.Id);
            responseContent.Role.Should().Be(TenantUserRole.Owner);
        }

        [Fact]
        public async Task UpdateUser_FromAnotherTenant_ReturnsUnauthorized()
        {
            // Arrange
            var newUser1 = CreateTestUser("user@tenantA.com", "password", "Tenant A");
            var newUser2 = CreateTestUser("user@tenantB.com", "password", "Tenant B");

            AuthenticateWithUser(newUser1.Email, "password");

            // Act
            var response = await TestClient.PutAsJsonAsync(ApiCoreRoutes.TenantUsers.Update
                .Replace("{id}", newUser2.MembershipFromCurrentTenant().Id.ToString()),
                new TenantUserUpdateRequest()
                {
                    Role = TenantUserRole.Admin,
                    Phone = "111 111 111"
                });

            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateUser_AsCurrentUser_ReturnsOk()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var tenantUser = newUser.MembershipFromTenantId(newUser.CurrentTenant.Id);
            var response = await TestClient.PutAsJsonAsync(ApiCoreRoutes.TenantUsers.Update.Replace("{id}", tenantUser.Id.ToString()),
                new TenantUserUpdateRequest()
                {
                    Role = TenantUserRole.Owner,
                    Phone = "111 111 111",
                    Workspaces = tenantUser.User.Workspaces.Select(f=> new WorkspaceDto()
                    {
                        Id = f.Id
                    }).ToList()
                });
            var responseContent = await response.Content.ReadAsAsync<TenantUserDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.User.Phone.Should().Be("111 111 111");
        }

        [Fact]
        public async Task Delete_TenantWith1User_ReturnsBadRequest()
        {
            // Arrange
            var newUser = CreateTestUser("user@tenant.com", "password", "Tenant 1");
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.DeleteAsync(ApiCoreRoutes.TenantUsers.Delete
                .Replace("{id}", newUser.MembershipFromCurrentTenant().Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
