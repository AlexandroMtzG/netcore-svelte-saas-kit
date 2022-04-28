namespace NetcoreSaas.Application.Contracts.Core.Tenants
{
    public class TenantUpdateJoinSettingsRequest
    {
        public bool EnableLink { get; set; }
        public bool ResetLink { get; set; }
        public bool EnablePublicUrl { get; set; }
        public bool RequireAcceptance { get; set; }
    }
}
