using System;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Domain.Models.Core.Workspaces;
using NetcoreSaas.Domain.Models.Interfaces;

namespace NetcoreSaas.Domain.Models.Core
{
    public abstract class AppWorkspaceEntity : Entity, IAppWorkspaceEntity
    {
        public Guid? CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }
        public Guid? ModifiedByUserId { get; set; }
        public User ModifiedByUser { get; set; }
        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}