using System;
using System.Linq;
using NetcoreSaas.Domain.Enums.Core.Subscriptions;
using NetcoreSaas.Domain.Extensions;

namespace NetcoreSaas.Application.Dtos.Core.Subscriptions
{
    public class SubscriptionPriceDto : MasterEntityDto
    {
        public string ServiceId { get; set; } // Stripe/Paddle Id
        public SubscriptionPriceType Type { get; set; }
        public SubscriptionBillingPeriod BillingPeriod { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int TrialDays { get; set; }
        public bool Active { get; set; }
        public decimal? PriceBefore { get; set; }
        public Guid SubscriptionProductId { get; set; }
        public SubscriptionProductDto SubscriptionProduct { get; set; }
        public SubscriptionPlanDto SubscriptionPlan;

        public string ToChatbotString(bool conPrecio)
        {
            if (SubscriptionProduct != null)
            {
                var strPrecio = conPrecio ? $" {Price.GetFormattedInt()}/{BillingPeriodShort()}" : "";
                return $"{SubscriptionProduct.Title}{strPrecio}";
            }
            return "!! Inválida";
        }

        public string ToChatbotString_Features()
        {
            return string.Join(Environment.NewLine, SubscriptionProduct.Features
                .OrderBy(f=>f.Order)
                .ThenBy(f=>f.Included)
                .Select(f => " -" + f.ToChatbotString()));
        }

        public string BillingPeriodShort()
        {
            switch (BillingPeriod)
            {
                case SubscriptionBillingPeriod.Daily: return "día";
                case SubscriptionBillingPeriod.Monthly: return "mes";
                case SubscriptionBillingPeriod.Yearly: return "año";
                case SubscriptionBillingPeriod.Once: return "único";
                case SubscriptionBillingPeriod.Weekly: return "semana";
            }

            return "";
        }
    }
}
