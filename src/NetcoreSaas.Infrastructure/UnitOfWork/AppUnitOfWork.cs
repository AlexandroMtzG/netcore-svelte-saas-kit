using System.Threading.Tasks;
using NetcoreSaas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NetcoreSaas.Application.Repositories.App;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Infrastructure.Repositories.App;
using NetcoreSaas.Infrastructure.Repositories.Core;

namespace NetcoreSaas.Infrastructure.UnitOfWork
{
    public sealed class AppUnitOfWork : IAppUnitOfWork
    {
        private readonly AppDbContext _context;
        // Core
        public IWorkspaceRepository Workspaces { get; }
        // App
        public IEmployeeRepository Employees { get; }

        public AppUnitOfWork(AppDbContext context)
        {
            _context = context;
            // Core
            Workspaces = new WorkspaceRepository(context);
            // App
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