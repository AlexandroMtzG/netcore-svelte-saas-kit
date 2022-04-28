using System;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Domain.Enums.Shared;

namespace NetcoreSaas.Application.Dtos.Core.Workspaces
{
    public class WorkspaceUserDto : AppEntityDto
    {
        public Guid WorkspaceId { get; set; }
        public WorkspaceDto Workspace { get; set; }
        public Guid UserId { get; set; }
        public UserSimpleDto User { get; set; }
        public Role Role { get; set; }
        public bool Default { get; set; }
    }
}