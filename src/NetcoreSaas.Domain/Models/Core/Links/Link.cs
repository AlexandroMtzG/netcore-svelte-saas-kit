using System;
using System.Collections.ObjectModel;
using NetcoreSaas.Domain.Enums.App.Links;
using NetcoreSaas.Domain.Models.App.Contracts;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Domain.Models.Core.Links
{
    public class Link : MasterEntity
    {
        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }
        public Guid CreatedByWorkspaceId { get; set; }
        public Workspace CreatedByWorkspace { get; set; }
        public Guid ProviderWorkspaceId { get; set; }
        public Workspace ProviderWorkspace { get; set; }
        public Guid ClientWorkspaceId { get; set; }
        public Workspace ClientWorkspace { get; set; }
        public LinkStatus Status { get; set; }
        public Collection<Contract> Contracts { get; set; }
        public LinkInvitation LinkInvitation { get; set; }

        public Link()
        {
            Contracts = new Collection<Contract>();
        }

        public bool HasWorkspaceId(Guid workspaceId)
        {
            return ClientWorkspaceId == workspaceId || ProviderWorkspaceId == workspaceId;
        }
    }
}