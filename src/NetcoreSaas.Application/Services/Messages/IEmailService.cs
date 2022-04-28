using System.Collections.Generic;
using System.Threading.Tasks;
using NetcoreSaas.Application.Dtos.Core.Emails;
using NetcoreSaas.Domain.Models.App.Contracts;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Application.Services.Messages
{
    public interface IEmailService
    {
        // Postmark
        Task<IEnumerable<EmailTemplateDto>> GetAllTemplates();
        Task<IEnumerable<EmailTemplateDto>> CreateTemplates();
        
        // Core
        Task SendWelcome(User user);
        Task SendResetPassword(User user);
        Task SendUserInvitation(Tenant tenant, User user, TenantUser invitation);
        Task SendRequestedAccess(TenantUser tenantUser, Tenant tenant);
        Task SendUserAccepted(Tenant tenant, User admin, TenantUser acceptedUser);
        Task SendInviteCompanyToCreateAccount(LinkInvitation invitation);
        Task SendInviteUserToLinkWorkspace(User fromUser, Workspace fromWorkspace, Link link, User user, Workspace workspace, bool asProvider);
        Task SendLinkInvitationAccepted(User userAccepting, Link link, bool asProvider);
        Task SendLinkInvitationRejected(string email, string name, User toUser);
        
        // Sample App
        Task SendContractNew(User userCreator, Link link, Contract contract);
    }
}