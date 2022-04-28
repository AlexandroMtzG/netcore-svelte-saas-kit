using System;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Domain.Models.Interfaces
{
    public interface IAppWorkspaceEntity : IAppEntity
    {
         Guid WorkspaceId { get; set; }
         Workspace Workspace { get; set; }
    }
}