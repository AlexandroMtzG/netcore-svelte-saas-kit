using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Application.Repositories.App
{
    public interface IWorkspaceRepository : IRepository<Workspace>
    {
        Task<Workspace> Get(Guid id);
        Task<IEnumerable<Workspace>> GetTenantWorkspaces(Guid tenantId);
        Task<IEnumerable<Workspace>> GetUserWorkspaces(TenantUser tenantUser, Guid? tenantId = null);
        Task<IEnumerable<WorkspaceUser>> GetAllUserWorkspaces(TenantUser tenantUser);
        Task<Workspace> GetWorkspaceByName(TenantUser tenantUser, string name);
        Task<WorkspaceUser> GetWorkspaceUserById(Guid id);
        Task<IEnumerable<WorkspaceUser>> GetUsers(Guid id);
        Task<IEnumerable<WorkspaceUser>> GetUsersWithWorkspace(Guid workspaceId);
        Task<IEnumerable<WorkspaceUser>> GetMembersWithTenant(Guid workspaceId);
        void AddWorkspaceUser(WorkspaceUser workspaceUser);
        void RemoveWorkspaceUser(WorkspaceUser workspaceUser);
    }
}
