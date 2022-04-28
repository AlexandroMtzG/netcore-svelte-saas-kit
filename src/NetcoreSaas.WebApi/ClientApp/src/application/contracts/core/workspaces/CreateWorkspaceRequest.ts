import type { UserDto } from "@/application/dtos/core/users/UserDto";
import type { WorkspaceType } from "@/application/enums/core/tenants/WorkspaceType";

export interface CreateWorkspaceRequest {
  name: string;
  type: WorkspaceType;
  businessMainActivity: string;
  registrationNumber: string;
  registrationDate: Date | null;
  users: UserDto[];
}
