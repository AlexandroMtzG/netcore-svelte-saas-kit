using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetcoreSaas.Application.Contracts.App.Contracts;
using NetcoreSaas.Domain.Models.App.Contracts;

namespace NetcoreSaas.Application.Repositories.App
{
    public interface IContractRepository : IRepository<Contract>
    {
        Task<int> CountAll(List<Guid> tenantWorkspaces, DateTime? fromDate, DateTime? toDate);
        Task<IEnumerable<Contract>> GetAllByStatusFilter(Guid workspaceId, ContractStatusFilter filter);
        Task<IEnumerable<Contract>> GetAllByLink(Guid workspaceId, Guid linkId);
        IEnumerable<Contract> GetByCreatedUser(Guid userId);
        Task<Contract> Get(Guid workspaceId, Guid id);
        void AddActivity(ContractActivity activity);
    }
}
