using System;
using NetcoreSaas.Domain.Enums.App.Links;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Domain.Models.Core.Links
{
    public class LinkInvitation : MasterEntity
    {
        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }
        public Guid CreatedByWorkspaceId { get; set; }
        public Workspace CreatedByWorkspace { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public string WorkspaceName { get; set; }
        public bool InviteeIsProvider { get; set; }
        public LinkStatus Status { get; set; }
        public Guid? CreatedLinkId { get; set; }
        public Link CreatedLink { get; set; }
    }
}