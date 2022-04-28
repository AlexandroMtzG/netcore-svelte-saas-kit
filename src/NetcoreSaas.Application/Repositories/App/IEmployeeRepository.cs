using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetcoreSaas.Domain.Models.App.Contracts;
using NetcoreSaas.Domain.Models.App.Employees;

namespace NetcoreSaas.Application.Repositories.App
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetAll(Guid tenantId);
        Task<int> CountAll(Guid tenantId);
        Task<ContractEmployee> GetContractEmployee(Guid id);
        Task<Employee> GetByEmail(string email);
    }
}
