using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NetcoreSaas.Application.Contracts.Core.Tenants;
using NetcoreSaas.Application.Contracts.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Tenants;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Domain.Models.Core.Workspaces;
using NetcoreSaas.IntegrationTests.Core.Subscriptions;
using Xunit;

namespace NetcoreSaas.IntegrationTests.Core.Tenants
{
    public class TenantUserInvitationControllerTests : SubscriptionTestBase
    {
        [Fact]
        public async Task GetInvitation_WithExistingPendingInvitation_ReturnsInvititation()
        {
            // Arrange
            var newUser = CreateTestUser("user1@tenantA.com", "password", "Tenant A");
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var inviteUserResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.InviteUser, new UserInviteRequest()
            {
                Email = "user2@tenanta.com",
                Role = TenantUserRole.Guest,
                Workspaces = newUser.Workspaces.Select(f=> new WorkspaceDto()
                {
                    Id = f.Id
                }).ToList()
            });
            var invitedUser = await inviteUserResponse.Content.ReadAsAsync<TenantUserDto>();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.TenantUserInvitation.GetInvitation.Replace("{invitationLink}", invitedUser.InvitationLink.ToString()));
            var responseContent = await response.Content.ReadAsAsync<TenantUserDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Status.Should().Be(TenantUserStatus.PendingInvitation);
        }

        [Fact]
        public async Task GetInviteUrl_WithPublicUrl_ReturnsTenant()
        {
            // Arrange
            var newUser = CreateTestUser("user1@tenantA.com", "password", "Tenant A");
            AuthenticateWithUser(newUser.Email, "password");

            var invitationSettingsResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.UpdateInvitationSettings, new TenantUpdateJoinSettingsRequest()
            {
                EnableLink = true,
                EnablePublicUrl = true,
                RequireAcceptance = true
            });
            var invitationSettings = await invitationSettingsResponse.Content.ReadAsAsync<TenantJoinSettingsDto>();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.TenantUserInvitation.GetInviteUrl
                .Replace("{linkUuid}", invitationSettings.Link.ToString()));
            var tenant = await response.Content.ReadAsAsync<TenantDto>();

            // Assert
            tenant.Should().NotBeNull();
        }

        [Fact]
        public async Task GetInvitationSettings_WithTenantWithoutSettings_ReturnsDefaultSettings()
        {
            // Arrange
            var newUser = CreateTestUser("user1@tenantA.com", "password", "Tenant A");
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.TenantUserInvitation.GetInvitationSettings.Replace("{tenantId}", newUser.CurrentTenant.Id.ToString()));
            var responseContent = await response.Content.ReadAsAsync<TenantJoinSettingsDto>();

            // Assert
            responseContent.Link.Should().BeEmpty();
            responseContent.LinkActive.Should().BeFalse();
            responseContent.PublicUrl.Should().BeFalse();
            responseContent.RequireAcceptance.Should().BeFalse();
        }

        [Fact]
        public async Task InviteUser_UserAsMember_ReturnsUserGuest()
        {
            // Arrange
            var newUser = CreateTestUser("user1@tenantA.com", "password", "Tenant A");
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.InviteUser, new UserInviteRequest()
            {
                 Email = "user2@tenantA.com",
                 FirstName = "User 2 name",
                 LastName = "User 2 last name",
                 Phone = "111 111 111",
                 Role = TenantUserRole.Guest,
                 Workspaces = newUser.Workspaces.Select(f=> new WorkspaceDto()
                 {
                     Id = f.Id
                 }).ToList()
            });
            var responseContent = await response.Content.ReadAsAsync<TenantUserDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Email.Should().Be("user2@tenantA.com".Trim().ToLower());
            responseContent.FirstName.Should().Be("User 2 name");
            responseContent.Status.Should().Be(TenantUserStatus.PendingInvitation);
            responseContent.Role.Should().Be(TenantUserRole.Guest);
        }

        [Fact]
        public async Task RequestAccess_WithoutRequiringAcceptance_ReturnsActiveUser()
        {
            // Arrange
            var newUser = CreateTestUser("user1@tenantA.com", "password", "Tenant A");
            AuthenticateWithUser(newUser.Email, "password");

            var invitationSettingsResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.UpdateInvitationSettings, new TenantUpdateJoinSettingsRequest()
            {
                EnableLink = true,
                EnablePublicUrl = true,
                RequireAcceptance = false
            });
            var invitationSettings = await invitationSettingsResponse.Content.ReadAsAsync<TenantJoinSettingsDto>();
            Logout();

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.RequestAccess
                .Replace("{linkUuid}", invitationSettings.Link.ToString()), new UserVerifyRequest()
            {
                Email = "user2@tenantA.com",
                Password = "password2",
                FirstName = "User 2 name",
                LastName = "User 2 last name",
            });
            var responseContent = await response.Content.ReadAsAsync<TenantUserDto>();

            // Assert
            responseContent.Status.Should().Be(TenantUserStatus.Active);
        }

        [Fact]
        public async Task RequestAccess_WithRequiringAcceptance_ReturnsTenantUser()
        {
            // Arrange
            var newUser = CreateTestUser("user1@tenantA.com", "password", "Tenant A");
            AuthenticateWithUser(newUser.Email, "password");

            var invitationSettingsResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.UpdateInvitationSettings, new TenantUpdateJoinSettingsRequest()
            {
                EnableLink = true,
                EnablePublicUrl = true,
                RequireAcceptance = true
            });
            var invitationSettings = await invitationSettingsResponse.Content.ReadAsAsync<TenantJoinSettingsDto>();
            Logout();

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.RequestAccess
                .Replace("{linkUuid}", invitationSettings.Link.ToString()), new UserVerifyRequest()
                {
                    Email = "user2@tenantA.com",
                    Password = "password2",
                });
            var responseContent = await response.Content.ReadAsAsync<TenantUserDto>();

            // Assert
            responseContent.Status.Should().Be(TenantUserStatus.PendingAcceptance);
        }


        [Fact]
        public async Task AcceptInvitation_WithValidInvitation_ReturnsAuthenticatedUser()
        {
            // Arrange
            // Arrange: Creating tenant
            var newUser = CreateTestUser("user1@tenantA.com", "password", "Tenant A");
            AuthenticateWithUser(newUser.Email, "password");
            // Arrange: Adding user
            var inviteResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.InviteUser, 
                new UserInviteRequest()
                {
                    Email = "user2@tenantA.com",
                    FirstName = "User 2 name",
                    LastName = "User 2 last name",
                    Phone = "111 111 111",
                    Role = TenantUserRole.Member,
                    Workspaces = newUser.Workspaces.Select(f=> new WorkspaceDto()
                    {
                        Id = f.Id
                    }).ToList()
                });
            var invitedUser = await inviteResponse.Content.ReadAsAsync<TenantUserDto>();
            Logout();

            // Act
            var acceptInvitationResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.AcceptInvitation
                .Replace("{invitationLink}", invitedUser.InvitationLink.ToString()), new UserVerifyRequest()
                {
                    Email = "user2@tenantA.com",
                    Password = "password2",
                });
            var userJoined = await acceptInvitationResponse.Content.ReadAsAsync<UserLoggedResponse>();
            var userMembershipInTenant = userJoined.User.MembershipFromTenantId(newUser.CurrentTenant.Id);

            // Assert
            userJoined.User.Should().NotBeNull();
            userJoined.Token.Should().NotBeEmpty();
            userJoined.User.Email.Should().Be("user2@tenantA.com".ToLower().Trim());
            userMembershipInTenant.Status.Should().Be(TenantUserStatus.Active);
        }

        [Fact]
        public async Task AcceptUser_WithRequiringAcceptance_ReturnsOk()
        {
            // Arrange
            var newUser = CreateTestUser("user1@tenantA.com", "password", "Tenant A");
            AuthenticateWithUser(newUser.Email, "password");

            // Arrange: Setting invitation settings
            var invitationSettingsResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.UpdateInvitationSettings, new TenantUpdateJoinSettingsRequest()
            {
                EnableLink = true,
                EnablePublicUrl = true,
                RequireAcceptance = true
            });
            var invitationSettings = await invitationSettingsResponse.Content.ReadAsAsync<TenantJoinSettingsDto>();
            Logout();

            // Arrange: Requesting access
            var requestAccessResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.RequestAccess
               .Replace("{linkUuid}", invitationSettings.Link.ToString()), new UserVerifyRequest()
               {
                   Email = "user2@tenantA.com",
                   Password = "password2",
               });
            var tenantUserRequestingAccess = await requestAccessResponse.Content.ReadAsAsync<TenantUserDto>();

            // Act
            AuthenticateWithUser(newUser.Email, "password");
            tenantUserRequestingAccess.Accepted = true;
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.AcceptUser
                .Replace("{tenantUserId}", tenantUserRequestingAccess.Id.ToString()), tenantUserRequestingAccess);
            var responseContent = await response.Content.ReadAsAsync<TenantUserDto>();

            // Assert
            responseContent.Status.Should().Be(TenantUserStatus.Active);
            responseContent.Joined.Should().Be(TenantUserJoined.JoinedByLink);
        }

        [Fact]
        public async Task AcceptUser_RejectUser_ReturnsNoContent()
        {
            // Arrange
            var newUser = CreateTestUser("user1@tenantA.com", "password", "Tenant A");
            AuthenticateWithUser(newUser.Email, "password");

            // Arrange: Setting invitation settings
            var invitationSettingsResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.UpdateInvitationSettings, new TenantUpdateJoinSettingsRequest()
            {
                EnableLink = true,
                EnablePublicUrl = true,
                RequireAcceptance = true
            });
            var invitationSettings = await invitationSettingsResponse.Content.ReadAsAsync<TenantJoinSettingsDto>();
            Logout();

            // Arrange: Requesting access
            var requestAccessResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.RequestAccess
               .Replace("{linkUuid}", invitationSettings.Link.ToString()), new UserVerifyRequest()
               {
                   Email = "user2@tenantA.com",
                   Password = "password2",
               });
            var tenantUserRequestingAccess = await requestAccessResponse.Content.ReadAsAsync<TenantUserDto>();

            // Act
            AuthenticateWithUser(newUser.Email, "password");
            tenantUserRequestingAccess.Accepted = false;
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.AcceptUser
                .Replace("{tenantUserId}", tenantUserRequestingAccess.Id.ToString()), tenantUserRequestingAccess);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateInvitationSettings_WithPublicUrl_ReturnsSettings()
        {
            // Arrange
            var newUser = CreateTestUser("user1@tenantA.com", "password", "Tenant A");
            AuthenticateWithUser(newUser.Email, "password");

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.TenantUserInvitation.UpdateInvitationSettings, new TenantUpdateJoinSettingsRequest()
            {
                EnableLink = true,
                EnablePublicUrl = true,
                RequireAcceptance = true
            }); 
            var responseContent = await response.Content.ReadAsAsync<TenantJoinSettingsDto>();

            // Assert
            responseContent.Link.Should().NotBeEmpty();
            responseContent.LinkActive.Should().BeTrue();
            responseContent.PublicUrl.Should().BeTrue();
            responseContent.RequireAcceptance.Should().BeTrue();
        }
    }
}
