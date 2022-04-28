using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetcoreSaas.Application.Repositories.App;
using NetcoreSaas.Domain.Models.App.Contracts;
using NetcoreSaas.Domain.Models.App.Employees;
using NetcoreSaas.Infrastructure.Data;

namespace NetcoreSaas.Infrastructure.Repositories.App
{
    public class EmployeeRepository : AppRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(BaseDbContext context) : base(context)
        {
            
        }

        public async Task<IEnumerable<Employee>> GetAll(Guid tenantId)
        {
            return await Context.Employees.Where(f => f.TenantId == tenantId).ToListAsync();
        }
        
        public async Task<int> CountAll(Guid tenantId)
        {
            return await Context.Employees.Where(f => f.TenantId == tenantId).CountAsync();
        }

        public async Task<ContractEmployee> GetContractEmployee(Guid id)
        {
            return await Context.ContractEmployee.Include(f=>f.Employee).FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Employee> GetByEmail(string email)
        {
            return await Context.Employees.FirstOrDefaultAsync(f => f.Email == email);
        }
    }
}
