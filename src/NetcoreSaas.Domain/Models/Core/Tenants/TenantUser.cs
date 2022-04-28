using System;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Domain.Models.Core.Tenants
{
    public class TenantUser : MasterEntity
    {
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public TenantUserRole Role { get; set; }
        public TenantUserJoined Joined { get; set; }
        public TenantUserStatus Status { get; set; }
        public Guid? InvitationLink { get; set; }
        // public Guid? ChatbotToken { get; set; } = Guid.NewGuid();
        // public bool ChatbotBotNotifications { get; set; } = true;
        public bool EmailNotifications { get; set; } = true;
        public string ChatbotCustomerId { get; set; }
        public bool RequiresVerify()
        {
            return !(string.IsNullOrEmpty(User.Password) == false && User.LoginType == UserLoginType.Password);
        }
    }
}
