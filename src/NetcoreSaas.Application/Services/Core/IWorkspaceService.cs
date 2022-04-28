using System.Collections.Generic;
using System.Threading.Tasks;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Application.Services.Core
{
    public interface IWorkspaceService
    {
        Task<Workspace> AddWorkspace(TenantUser tenantUser, Tenant tenant, string name);
        Task UpdateUserWorkspaces(TenantUser tenantUser, List<WorkspaceDto> workspaces);
        Task UpdateWorkspaceUsers(Workspace workspace, List<UserDto> workspaces);
    }
}
