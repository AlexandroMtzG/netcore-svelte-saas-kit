using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetcoreSaas.Domain.Enums.App.Links;
using NetcoreSaas.Domain.Models.Core.Links;

namespace NetcoreSaas.Application.Repositories.Core
{
    public interface ILinkRepository : IRepository<Link>
    {
        Task<IEnumerable<Link>> GetAll(Guid workspaceId, LinkStatus status);
        Task<IEnumerable<Link>> GetAll(List<Guid> tenantWorkspaces, LinkStatus status);
        Task<int> CountAll(List<Guid> tenantWorkspaces, LinkStatus status);
        Task<IEnumerable<Link>> GetAllProviders(List<Guid> tenantWorkspaces);
        // Task<IEnumerable<Link>> GetAllProviders(List<Guid> tenantWorkspaces, DocumentationStatusFilter filter);
        Task<IEnumerable<Link>> GetAllProviders(Guid workspaceId);
        Task<Link> GetProvider(Guid workspaceId, string name);
        Task<IEnumerable<Link>> GetAllClients(List<Guid> tenantWorkspaces);
        Task<int> CountAllProviders(List<Guid> tenantWorkspaces);
        Task<int> CountAllClients(List<Guid> tenantWorkspaces);
        Task<IEnumerable<Link>> GetAllClients(Guid workspaceId);
        Task<Link> GetClient(Guid workspaceId, string name);
        Task<LinkInvitation> GetInvitation(Guid id);
        Task<Link> Get(Guid workspaceId, Guid id);
        void AddInvitation(LinkInvitation invitation);
    }
}
