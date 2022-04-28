using System.Collections.Generic;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Domain.Enums.Core.Tenants;

namespace NetcoreSaas.Application.Contracts.Core.Tenants
{
    public class TenantUserUpdateRequest
    {
        public TenantUserRole Role { get; set; }
        public string Phone { get; set; }
        public List<WorkspaceDto> Workspaces { get; set; }
    }
}
