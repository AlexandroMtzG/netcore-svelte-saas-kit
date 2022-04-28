using System;

namespace NetcoreSaas.Application.Dtos.Core.Subscriptions
{
    public class SubscriptionPlanDto
    {
        public int TrialPeriodDays { get; set; }
        public DateTime? TrialStart { get; set; }
        public DateTime? TrialEnd { get; set; }
    }
}
