using System;

namespace NetcoreSaas.Application.Dtos.Core.Tenants
{
    public class TenantJoinSettingsDto : MasterEntityDto
    {
        public Guid TenantId { get; set; }
        public TenantSimpleDto Tenant { get; set; }
        public Guid Link { get; set; }
        public bool LinkActive { get; set; }
        public bool PublicUrl { get; set; }
        public bool RequireAcceptance { get; set; }
    }
}
