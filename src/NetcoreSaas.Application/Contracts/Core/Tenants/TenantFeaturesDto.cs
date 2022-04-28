namespace NetcoreSaas.Application.Contracts.Core.Tenants
{
    public class TenantFeaturesDto
    {
        public int MaxWorkspaces { get; set; }
        public int MaxUsers { get; set; }
        public int MaxLinks { get; set; }
        public int MaxStorage { get; set; }
        public int MonthlyContracts { get; set; }
    }
}
