using NetcoreSaas.Application.Dtos.Core.Tenants;

namespace NetcoreSaas.Application.Contracts.Core.Tenants
{
    public class TenantInvitationResponse
    {
        public TenantUserDto Invitation { get; set; }
        public TenantDto Tenant { get; set; }
        public bool RequiresVerify { get; set; }
        public TenantInvitationResponse(TenantUserDto invitation, TenantDto tenant, bool requiresVerify)
        {
            Invitation = invitation;
            Tenant = tenant;
            RequiresVerify = requiresVerify;
        }
    }
}
