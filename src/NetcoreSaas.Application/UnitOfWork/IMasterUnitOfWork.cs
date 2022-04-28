using NetcoreSaas.Application.Repositories.App;
using NetcoreSaas.Application.Repositories.Core;

namespace NetcoreSaas.Application.UnitOfWork
{
    public interface IMasterUnitOfWork : IBaseUnitOfWork
    {
        // Core
        ISubscriptionProductRepository Subscriptions { get; }
        ITenantRepository Tenants { get; }
        IUserRepository Users { get; }
        // App
        ILinkRepository Links { get; }
        IContractRepository Contracts { get; }
    }
}