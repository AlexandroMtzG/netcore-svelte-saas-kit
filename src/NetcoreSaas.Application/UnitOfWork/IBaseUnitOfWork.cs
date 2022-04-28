using System.Threading.Tasks;
using NetcoreSaas.Application.Repositories.App;

namespace NetcoreSaas.Application.UnitOfWork
{
    public interface IBaseUnitOfWork
    {
        // Core
        IWorkspaceRepository Workspaces { get; }
        // App
        IEmployeeRepository Employees { get; }
        Task<int> CommitAsync();
    }
}