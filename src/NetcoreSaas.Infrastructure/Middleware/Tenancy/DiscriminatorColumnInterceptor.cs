//using System.Data.Common;
//using System.Threading;
//using System.Threading.Tasks;
//using NetcoreSaas.Domain.Models.Master.Tenants;
//using Microsoft.EntityFrameworkCore.Diagnostics;

//namespace NetcoreSaas.Infrastructure.Middleware.Tenancy
//{
//    public class DiscriminatorColumnInterceptor : DbCommandInterceptor
//    {
//        private readonly TenantAccessService<Tenant> _tenantAccessService;
//        //private readonly Tenant tenant;
//        public DiscriminatorColumnInterceptor(TenantAccessService<Tenant> tenantAccessService)
//        {
//            _tenantAccessService = tenantAccessService;
//            //tenant = _tenantAccessService.GetTenant();
//        }
//        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
//        {
//            return base.ReaderExecuting(AddTenantToCommandText(command), eventData, result);
//        }
//        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
//        {
//            return base.ReaderExecutingAsync(AddTenantToCommandText(command), eventData, result);
//        }
//        private DbCommand AddTenantToCommandText(DbCommand command)
//        {
//            // if (command.CommandText.Contains("SELECT") && command.CommandText.Contains("INSERT INTO") == false)
//            // {
//            //     if (command.CommandText.Contains("WHERE"))
//            //     {
//            //         command.CommandText += $" AND Tenant = '{tenant.ApiKey}'";
//            //     }
//            //     else
//            //     {
//            //         command.CommandText += $" WHERE Tenant = '{tenant.ApiKey}'";
//            //     }
//            // }
//            return command;
//        }
//    }
//}