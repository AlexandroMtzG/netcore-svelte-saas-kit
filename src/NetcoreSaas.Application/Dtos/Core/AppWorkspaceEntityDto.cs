using System;
using NetcoreSaas.Application.Dtos.Core.Workspaces;

namespace NetcoreSaas.Application.Dtos.Core
{
    public class AppWorkspaceEntityDto : AppEntityDto
    {
        public Guid WorkspaceId { get; set; }
        public WorkspaceSimpleDto Workspace { get; set; }
    }
}