using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetcoreSaas.Application.Repositories.Core;
using NetcoreSaas.Domain.Enums.App.Links;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Infrastructure.Data;

namespace NetcoreSaas.Infrastructure.Repositories.Core
{
    public class LinkRepository : MasterRepository<Link>, ILinkRepository
    {
        public LinkRepository(MasterDbContext context) : base(context)
        {
        }

        public IQueryable<Link> GetMyLinks(Guid workspaceId)
        {
            return Context.Links
                .Include(f => f.CreatedByUser)
                .Include(f => f.ProviderWorkspace)
                .Include(f => f.ClientWorkspace)
                .Where(f => f.ProviderWorkspaceId == workspaceId || f.ClientWorkspaceId == workspaceId);
        }

        public async Task<IEnumerable<Link>> GetAll(Guid workspaceId, LinkStatus status)
        {
            return await GetMyLinks(workspaceId).Where(f => f.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Link>> GetAll(List<Guid> tenantWorkspaces, LinkStatus status)
        {
            return await Context.Links
                .Where(f => f.Status == LinkStatus.Pending && (tenantWorkspaces.Any(x => x == f.ClientWorkspaceId) ||
                                                               tenantWorkspaces.Any(x => x == f.ProviderWorkspaceId)))
                .ToListAsync();
        }
        
        public async Task<int> CountAll(List<Guid> tenantWorkspaces, LinkStatus status)
        {
            return await Context.Links
                .Where(f => f.Status == status && (tenantWorkspaces.Any(x => x == f.ClientWorkspaceId) ||
                                                   tenantWorkspaces.Any(x => x == f.ProviderWorkspaceId)))
                .CountAsync();
        }

        public async Task<IEnumerable<Link>> GetAllProviders(List<Guid> tenantWorkspaces)
        {
            return await Context.Links
                .Include(f => f.ProviderWorkspace)
                .Where(f => f.Status == LinkStatus.Linked && tenantWorkspaces.Any(x => x == f.ClientWorkspaceId))
                .ToListAsync();
        }

        // public Task<IEnumerable<Link>> GetAllProvidersInCompliance(List<Guid> tenantWorkspaces, DocumentationStatusFilter filter)
        // {
        //     var providers = await Context.Links
        //         .Where(f => f.Status == LinkStatus.Linked && tenantWorkspaces.Any(x=>x == f.ClientWorkspaceId))
        //         .ToListAsync();
        // }

        public async Task<Link> GetProvider(Guid workspaceId, string workspaceName)
        {
            return await Context.Links
                .Include(f => f.ProviderWorkspace)
                .FirstOrDefaultAsync(f => f.ClientWorkspaceId == workspaceId && f.ProviderWorkspace.Name == workspaceName);
        }

        public async Task<IEnumerable<Link>> GetAllClients(List<Guid> tenantWorkspaces)
        {
            return await Context.Links
                .Where(f => f.Status == LinkStatus.Linked && tenantWorkspaces.Any(x => x == f.ProviderWorkspaceId))
                .ToListAsync();
        }
        
        public async Task<int> CountAllProviders(List<Guid> tenantWorkspaces)
        {
            return await Context.Links
                .Where(f => f.Status == LinkStatus.Linked && tenantWorkspaces.Any(x => x == f.ClientWorkspaceId))
                .CountAsync();
        }
        
        public async Task<int> CountAllClients(List<Guid> tenantWorkspaces)
        {
            return await Context.Links
                .Where(f => f.Status == LinkStatus.Linked && tenantWorkspaces.Any(x => x == f.ProviderWorkspaceId))
                .CountAsync();
        }

        public async Task<IEnumerable<Link>> GetAllProviders(Guid workspaceId)
        {
            return await GetMyLinks(workspaceId)
                .Where(f => f.Status == LinkStatus.Linked && f.ClientWorkspaceId == workspaceId)
                .Include(f => f.ProviderWorkspace)
                .ToListAsync();
        }

        public async Task<IEnumerable<Link>> GetAllClients(Guid workspaceId)
        {
            return await GetMyLinks(workspaceId)
                .Where(f => f.Status == LinkStatus.Linked && f.ProviderWorkspaceId == workspaceId)
                .Include(f => f.ClientWorkspace)
                .ToListAsync();
        }

        public async Task<Link> GetClient(Guid workspaceId, string workspaceName)
        {
            return await Context.Links
                .Include(f => f.ClientWorkspace)
                .FirstOrDefaultAsync(f => f.ProviderWorkspaceId == workspaceId && f.ClientWorkspace.Name == workspaceName);
        }

        public async Task<LinkInvitation> GetInvitation(Guid id)
        {
            return await Context.LinkInvitations
                .Include(f=>f.CreatedByWorkspace)
                .Include(f=>f.CreatedByUser)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Link> Get(Guid workspaceId, Guid id)
        {
            return await Context.Links
                .Where(f => f.Id == id && (f.ClientWorkspaceId == workspaceId || f.ProviderWorkspaceId == workspaceId))
                .Include(f=>f.CreatedByWorkspace)
                .Include(f=>f.CreatedByUser)
                .Include(f=>f.ClientWorkspace)
                .FirstOrDefaultAsync();
        }

        public void AddInvitation(LinkInvitation invitation)
        {
            Context.LinkInvitations.Add(invitation);
        }
    }
}
