using System;

namespace NetcoreSaas.Domain.Models.Core.Tenants
{
    public class TenantJoinSettings : MasterEntity
    {
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public Guid Link { get; set; }
        public bool LinkActive { get; set; }
        public bool PublicUrl { get; set; }
        public bool RequireAcceptance { get; set; }
        public TenantJoinSettings()
        {

        }
        public TenantJoinSettings(Tenant tenant, Guid link, bool linkActive, bool publicUrl, bool requireAcceptance)
        {
            Tenant = tenant;
            Link = link;
            LinkActive = linkActive;
            PublicUrl = publicUrl;
            RequireAcceptance = requireAcceptance;
        }
    }
}
