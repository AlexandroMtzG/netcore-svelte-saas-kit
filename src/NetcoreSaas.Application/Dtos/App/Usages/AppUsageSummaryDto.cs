using NetcoreSaas.Domain.Enums.App.Usages;

namespace NetcoreSaas.Application.Dtos.App.Usages
{
    public class AppUsageSummaryDto
    {
        public AppUsageType Type { get; set; }
        public int Providers { get; set; }
        public int ProvidersInCompliance { get; set; }
        public int Clients { get; set; }
        public int Employees { get; set; }
        public int Cfdis { get; set; }
        public int Contracts { get; set; }
        public decimal Storage { get; set; }
        public int PendingInvitations { get; set; }

        public AppUsageSummaryDto(AppUsageType type)
        {
            Type = type;
        }
    }
}