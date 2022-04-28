namespace NetcoreSaas.Application.Contracts.Core.Tenants
{
    public class TenantUpdateImageRequest
    {
        public string Type { get; set; }
        public string Icon { get; set; }
        public string Logo { get; set; }
        public string LogoDarkmode { get; set; }
    }
}
