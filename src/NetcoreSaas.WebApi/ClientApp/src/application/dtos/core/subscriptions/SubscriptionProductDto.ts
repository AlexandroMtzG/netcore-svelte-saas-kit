import type { SubscriptionPriceDto } from "./SubscriptionPriceDto";
import type { MasterEntityDto } from "../MasterEntityDto";
import type { SubscriptionFeatureDto } from "./SubscriptionFeatureDto";

export interface SubscriptionProductDto extends MasterEntityDto {
  serviceId: string;
  tier: number;
  title: string;
  description: string;
  badge: string;
  active: boolean;
  contactUs: boolean;
  maxWorkspaces: number;
  maxUsers: number;
  maxLinks: number;
  maxStorage: number;
  monthlyContracts: number;
  prices: SubscriptionPriceDto[];
  features: SubscriptionFeatureDto[];
}
