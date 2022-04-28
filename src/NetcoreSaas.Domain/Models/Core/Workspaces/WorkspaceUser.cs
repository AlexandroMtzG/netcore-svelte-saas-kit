using System;
using NetcoreSaas.Domain.Enums.Shared;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Domain.Models.Core.Workspaces
{
    public class WorkspaceUser : MasterEntity
    {
        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
        public bool Default { get; set; }
    }
}