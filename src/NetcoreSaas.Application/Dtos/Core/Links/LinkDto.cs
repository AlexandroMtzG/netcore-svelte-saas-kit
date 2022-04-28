using System;
using System.Collections.ObjectModel;
using NetcoreSaas.Application.Dtos.App.Contracts;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Domain.Enums.App.Links;

namespace NetcoreSaas.Application.Dtos.Core.Links
{
    public class LinkDto : MasterEntityDto
    {
        public Guid CreatedByWorkspaceId { get; set; }
        public WorkspaceDto CreatedByWorkspace { get; set; }
        public Guid ProviderWorkspaceId { get; set; }
        public WorkspaceDto ProviderWorkspace { get; set; }
        public Guid ClientWorkspaceId { get; set; }
        public WorkspaceDto ClientWorkspace { get; set; }
        public LinkStatus Status { get; set; }
        public Collection<ContractDto> Contracts { get; set; }
    }
}