using System.Collections.Generic;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Domain.Enums.Core.Tenants;

namespace NetcoreSaas.Application.Contracts.Core.Users
{
    public class UserInviteRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public TenantUserRole Role { get; set; }
        public List<WorkspaceDto> Workspaces { get; set; }
    }
}
