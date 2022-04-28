using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetcoreSaas.Application.Contracts.App.Contracts;
using NetcoreSaas.Application.Repositories.App;
using NetcoreSaas.Domain.Enums.App.Contracts;
using NetcoreSaas.Domain.Models.App.Contracts;
using NetcoreSaas.Infrastructure.Data;

namespace NetcoreSaas.Infrastructure.Repositories.App
{
    public class ContractRepository : MasterRepository<Contract>, IContractRepository
    {
        public ContractRepository(MasterDbContext context) : base(context)
        {
            
        }

        private IQueryable<Contract> GetQueryable(Guid workspaceId)
        {
            var list = Context.Contracts
                .Include(f => f.Link)
                .Include(f => f.Activity)
                .Include(f => f.Members)
                .AsQueryable();
            
            list = list
                .Include(f => f.Employees)
                .ThenInclude(f=>f.Employee);
            list = list.Include(f => f.Link).ThenInclude(f => f.ProviderWorkspace);
            list = list.Include(f => f.Link).ThenInclude(f => f.ClientWorkspace);
            
            return list.Where(f => f.Link.ProviderWorkspaceId == workspaceId || f.Link.ClientWorkspaceId == workspaceId);
        }

        public async Task<int> CountAll(List<Guid> tenantWorkspaces, DateTime? fromDate, DateTime? toDate)
        {
            var list = Context.Contracts
                .Include(f => f.Link)
                .Where(f => tenantWorkspaces.Any(x => x == f.Link.ProviderWorkspaceId) ||
                            tenantWorkspaces.Any(x => x == f.Link.ClientWorkspaceId));
            if (fromDate.HasValue)
                list = list.Where(f => f.CreatedAt >= fromDate.Value.Date);
            if (toDate.HasValue)
                list = list.Where(f => f.CreatedAt <= toDate.Value.Date);
            return await list.CountAsync();
        }

        public async Task<IEnumerable<Contract>> GetAllByStatusFilter(Guid workspaceId, ContractStatusFilter filter)
        {
            var list = Context.Contracts
                .AsQueryable();
            
            list = list.Include(f => f.Link).ThenInclude(f => f.ProviderWorkspace);
            list = list.Include(f => f.Link).ThenInclude(f => f.ClientWorkspace);
            
            list = list.Where(f => f.Link.ProviderWorkspaceId == workspaceId || f.Link.ClientWorkspaceId == workspaceId);

            switch (filter)
            {
                case ContractStatusFilter.Pending:
                    list = list.Where(f => f.Status == ContractStatus.Pending);
                    break;
                case ContractStatusFilter.Signed:
                    list = list.Where(f => f.Status == ContractStatus.Signed);
                    break;
                case ContractStatusFilter.Archived:
                    list = list.Where(f => f.Status == ContractStatus.Archived);
                    break;
            }
            
            return await list.Select(f=>new Contract()
            {
                Id = f.Id,
                Status = f.Status,
                Name = f.Name,
                Description = f.Description,
                Link = f.Link,
                LinkId = f.LinkId,
                CreatedAt = f.CreatedAt
            }).ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetAllByLink(Guid workspaceId, Guid linkId)
        {
            var list = GetQueryable(workspaceId)
                .Select(f=>new Contract()
                {
                    Id = f.Id,
                    Status = f.Status,
                    Name = f.Name,
                    Description = f.Description,
                    Link = f.Link,
                    LinkId = f.LinkId,
                    CreatedAt = f.CreatedAt
                }).Where(f=>f.LinkId == linkId);
            return await list.ToListAsync();
        }

        public IEnumerable<Contract> GetByCreatedUser(Guid userId)
        {
            return Context.Contracts
                .Include(f=>f.Employees)
                .Where(f=>f.CreatedByUserId == userId)
                .AsEnumerable();
        }

        public async Task<Contract> Get(Guid workspaceId, Guid id)
        {
            var list = Context.Contracts
                .Include(f => f.Link).ThenInclude(f => f.ProviderWorkspace)
                .Include(f => f.Link).ThenInclude(f => f.ClientWorkspace)
                .Include(f => f.Activity)
                .Include(f => f.Employees).ThenInclude(f=>f.Employee)
                .Include(f=>f.Members).ThenInclude(f=>f.User)
                .Where(f => f.Id == id)
                .AsQueryable();

            return await list.FirstOrDefaultAsync();
        }

        public void AddActivity(ContractActivity activity)
        {
            Context.ContractActivity.Add(activity);
        }
    }
}
