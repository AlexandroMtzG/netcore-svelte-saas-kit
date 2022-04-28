using System.Threading.Tasks;
using NetcoreSaas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NetcoreSaas.Application.Repositories.App;
using NetcoreSaas.Application.Repositories.Core;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Infrastructure.Repositories.App;
using NetcoreSaas.Infrastructure.Repositories.Core;

namespace NetcoreSaas.Infrastructure.UnitOfWork
{
    public sealed class MasterUnitOfWork : IMasterUnitOfWork
    {
        private readonly MasterDbContext _context;
        
        // Master Repositories
        public ISubscriptionProductRepository Subscriptions { get; }
        public ITenantRepository Tenants { get; }
        public IUserRepository Users { get; }
        
        // App Repositories
        public IWorkspaceRepository Workspaces { get; }
        
        // v3
        public IContractRepository Contracts { get; set; }
        public IEmployeeRepository Employees { get; set; }
        public ILinkRepository Links { get; }
        // End NetcoreSaas

        public MasterUnitOfWork(MasterDbContext context)
        {
            _context = context;
            
            // Core
            Subscriptions = new SubscriptionProductRepository(context);
            Tenants = new TenantRepository(context);
            Users = new UserRepository(context);
            Workspaces = new WorkspaceRepository(context);
            
            // App
            Links = new LinkRepository(context);
            Contracts = new ContractRepository(context);
            Employees = new EmployeeRepository(context);
        }
        
        public int Commit()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void EntryModified(object entry)
        {
            _context.Entry(entry).State = EntityState.Modified;
        }
    }
}