using System;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Domain.Enums.App.Links;

namespace NetcoreSaas.Application.Dtos.Core.Links
{
    public class LinkInvitationDto : MasterEntityDto
    {
        public Guid CreatedByWorkspaceId { get; set; }
        public WorkspaceDto CreatedByWorkspace { get; set; }
        public string Email { get; set; }
        public string WorkspaceName { get; set; }
        public string Message { get; set; }
        public bool InviteeIsProvider { get; set; }
        public LinkStatus Status { get; set; }
        public Guid? CreatedLinkId { get; set; }
        public LinkDto CreatedLink { get; set; }
    }
}