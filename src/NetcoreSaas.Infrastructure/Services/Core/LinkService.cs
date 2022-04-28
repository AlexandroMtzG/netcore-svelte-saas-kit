using System;
using System.Linq;
using System.Threading.Tasks;
using NetcoreSaas.Application.Dtos.Core.Links;
using NetcoreSaas.Application.Services.Core;
using NetcoreSaas.Application.Services.Messages;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.App.Links;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Shared;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Infrastructure.Services.Core
{
    public class LinkService : ILinkService
    {
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly IEmailService _emailService;
        public LinkService(IMasterUnitOfWork masterUnitOfWork, IEmailService emailService)
        {
            _masterUnitOfWork = masterUnitOfWork;
            _emailService = emailService;
        }

        public async Task<Workspace> GetWorkspace(Guid workspaceId, Guid linkId)
        {
            var link = await _masterUnitOfWork.Links.Get(workspaceId, linkId);
            if (link == null)
                throw new Exception("api.errors.youDontBelong");
            
            var otherWorkspaceId = link.ProviderWorkspaceId;
            if (link.ProviderWorkspaceId == workspaceId)
                otherWorkspaceId = link.ClientWorkspaceId;
            
            var workspace = await _masterUnitOfWork.Workspaces.Get(otherWorkspaceId);

            return workspace;
        }

        public async Task<User> SearchMember(User user, string workspaceName)
        {
            var tenantUsers = await _masterUnitOfWork.Users.GetUserTenants(user);
            foreach (var tenantUser in tenantUsers)
            {
                var workspaces = (await _masterUnitOfWork.Workspaces.GetTenantWorkspaces(tenantUser.TenantId)).ToList();
                var workspace = workspaces.Find(f => f.Name == workspaceName);
                if (workspace == null) continue;
                if (tenantUser.Role is TenantUserRole.Admin or TenantUserRole.Member)
                    return user;
                var workspaceUsers = (await _masterUnitOfWork.Workspaces.GetUsers(workspace.Id)).ToList();
                var workspaceUser = workspaceUsers.Find(f => f.User.Email == user.Email);
                if(workspaceUser == null) continue;
                if (workspaceUser.Role != Role.Guest)
                    return user;
                
                throw new Exception($"User {user.Email} does belong to workspace {workspace.Name} pero no is not Owner, Admin or Member");
            }

            return null;
        }

        public async Task<User> GetMember(Workspace workspace, string email)
        {
            var tenantUsers = (await _masterUnitOfWork.Tenants.GetTenantUsers(workspace.TenantId)).ToList();
            var tenantUser = tenantUsers.Find(f => f.User.Email == email && (f.Role == TenantUserRole.Admin || f.Role == TenantUserRole.Owner));
            if (tenantUser != null)
            {
                return tenantUser.User;
            }
            
            var workspaceUsers = (await _masterUnitOfWork.Workspaces.GetUsers(workspace.Id)).ToList();
            var workspaceUser = workspaceUsers.Find(f => f.User.Email == email);
            if (workspaceUser != null)
            {
                return workspaceUser.User;
            }
        
            return null;
        }

        public async Task<LinkInvitation> InviteUser(User fromUser, Workspace fromWorkspace, LinkInvitationDto request)
        {
            var invitation = new LinkInvitation()
            {
                CreatedByWorkspace = fromWorkspace,
                CreatedByWorkspaceId = fromWorkspace.Id,
                CreatedByUser = fromUser,
                CreatedByUserId = fromUser.Id,
                Email = request.Email,
                WorkspaceName = request.WorkspaceName,
                Message = request.Message,
                InviteeIsProvider = request.InviteeIsProvider,
                Status = LinkStatus.Pending
            };
            _masterUnitOfWork.Links.AddInvitation(invitation);
            await _masterUnitOfWork.CommitAsync();
            
            await _emailService.SendInviteCompanyToCreateAccount(invitation);

            return invitation;
        }
    }
}