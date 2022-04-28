using System;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Domain.Enums.Core.Tenants;

namespace NetcoreSaas.Application.Dtos.Core.Tenants
{
    public class TenantUserDto : MasterEntityDto
    {
        public Guid TenantId { get; set; }
        public TenantSimpleDto Tenant { get; set; }
        public Guid UserId { get; set; }
        public UserSimpleDto User { get; set; }
        public TenantUserRole Role { get; set; }
        public TenantUserJoined Joined { get; set; }
        public TenantUserStatus Status { get; set; }
        public bool Accepted { get; set; }
        public Guid? InvitationLink { get; set; }
        // public Guid? ChatbotToken { get; set; }
        public string Email => User?.Email ?? "";
        public string FirstName => User?.FirstName ?? "";
        public string LastName => User?.LastName ?? "";
        public string Phone => User?.Phone ?? "";
    }
}
