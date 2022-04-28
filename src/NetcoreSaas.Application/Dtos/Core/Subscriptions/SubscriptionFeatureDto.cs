using System;

namespace NetcoreSaas.Application.Dtos.Core.Subscriptions
{
    public class SubscriptionFeatureDto : MasterEntityDto
    {
        public int Order { get; set; }
        public Guid SubscriptionProductId { get; set; }
        public SubscriptionProductDto SubscriptionProduct { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool TranslateInFrontend { get; set; }
        public bool Included { get; set; }

        public string ToChatbotString()
        {
            if (Key == "monthlyDocuments")
            {
                return Value;
            } else if (Key == "maxNumberOfUsers" || Key == "oneUser")
            {
                if (Key == "oneUser")
                {
                    return "1 usuario";
                } else
                {
                    return $"Hasta {Value} usuarios";
                }
            }else if (Key == "maxNumberOfWorkspaces" || Key == "oneWorkspace")
            {
                if (Key == "oneWorkspace")
                {
                    return "1 RFC";
                } else
                {
                    return $"Hasta {Value} RFCs";
                }
            }else if (Key == "whatsApp")
            {
                return Included ? "Envía tickets por WhatsApp" : "~Envía tickets por WhatsApp~";
            } else if (Key == "onboarding30min" || Key == "onboarding2hrs")
            {
                if (Key == "onboarding30min")
                {
                    return Included ? "30 min de introducción" : "~30 min de introducción~";
                }
                else
                {
                    return Included ? "2 hrs de introducción" : "~2 hrs de introducción~";
                }
            } else if (Key == "prioritySupport")
            {
                return Included ? "Soporte prioritario" : "~Soporte prioritario~";
            }
            return "";
        }
    }
}
