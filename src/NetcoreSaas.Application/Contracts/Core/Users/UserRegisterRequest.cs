using System;
using System.ComponentModel.DataAnnotations;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;

namespace NetcoreSaas.Application.Contracts.Core.Users
{
    public class UserRegisterRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public SelectedSubscriptionRequest SelectedSubscription { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public UserLoginType LoginType { get; set; }
        public string WorkspaceName { get; set; }
        public Guid? JoinedByLinkInvitation { get; set; }
        public WorkspaceType WorkspaceType { get; set; }
    }
}
