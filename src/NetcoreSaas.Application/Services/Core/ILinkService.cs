using System;
using System.Threading.Tasks;
using NetcoreSaas.Application.Dtos.Core.Links;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Application.Services.Core
{
    public interface ILinkService
    {
        Task<Workspace> GetWorkspace(Guid workspaceId, Guid linkId);
        Task<User> SearchMember(User user, string workspaceName);
        Task<User> GetMember(Workspace workspace, string email);
        // Task<Link> Create(User userCreating, Guid workspaceId, User user, Workspace workspace, bool asProvider);
        Task<LinkInvitation> InviteUser(User fromUser, Workspace fromWorkspace, LinkInvitationDto invitation);
    }
}