using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NetcoreSaas.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using NetcoreSaas.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using NetcoreSaas.Domain.Models.Core;

namespace NetcoreSaas.Infrastructure.Data
{
    public class AppDbContext : BaseDbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AppDbContext(
            DbContextOptions<AppDbContext> options, 
            IHttpContextAccessor httpContextAccessor, 
            IConfiguration conf,
            ILoggerFactory loggerFactory
            ) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _loggerFactory = loggerFactory;
            
            // ReSharper disable once VirtualMemberCallInConstructor
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.UseIdentityColumns();

            // Set QueryFilter for Tenancy
            var entityTypes = GetEntityTypes();
            foreach (var type in entityTypes)
            {
                if (typeof(AppEntity).IsAssignableFrom(type))
                {
                    var setTenantGlobalQueryMethod = SetTenantGlobalQueryMethod.MakeGenericMethod(type);
                    setTenantGlobalQueryMethod.Invoke(this, new object[] {builder});
                }

                if (typeof(AppWorkspaceEntity).IsAssignableFrom(type))
                {
                    var setWorkspaceGlobalQueryMethod = SetWorkspaceGlobalQueryMethod.MakeGenericMethod(type);
                    setWorkspaceGlobalQueryMethod.Invoke(this, new object[] {builder});
                }
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLoggerFactory(_loggerFactory).EnableSensitiveDataLogging();
            //optionsBuilder.UseLowerCaseNamingConvention();
            var connectionString = ProjectConfiguration.GlobalConfiguration.MasterDbContext;
            if (ProjectConfiguration.GlobalConfiguration.MultiTenancy == MultiTenancy.DatabasePerTenant)
            {
                var tenantUuid = _httpContextAccessor.HttpContext.GetTenantUserId();
                if (tenantUuid != Guid.Empty)
                {
                    connectionString = ProjectConfiguration.GlobalConfiguration.GetTenantContext(tenantUuid);
                }
            }
            switch (ProjectConfiguration.GlobalConfiguration.DatabaseProvider)
            {
                case DatabaseProvider.PostgreSql: optionsBuilder.UseNpgsql(connectionString,
                        b => b.MigrationsAssembly("NetcoreSaas.WebApi"));
                    break;
                case DatabaseProvider.MySql: optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                        b => b.MigrationsAssembly("NetcoreSaas.WebApi"));
                    break;
            }


        }
        public override int SaveChanges()
        {
            var auditEntries = OnBeforeSaveChanges();
            AddTenantToProperties(_httpContextAccessor.HttpContext.GetUserId(), _httpContextAccessor.HttpContext.GetTenantId(), _httpContextAccessor.HttpContext.GetWorkspaceId());
            var result = base.SaveChanges();
            AuditOnAfterSaveChanges(auditEntries);
            return result;
        }
        
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, System.Threading.CancellationToken cancellationToken = default)
        {
            var auditEntries = OnBeforeSaveChanges();
            AddTenantToProperties(_httpContextAccessor.HttpContext.GetUserId(), _httpContextAccessor.HttpContext.GetTenantId(), _httpContextAccessor.HttpContext.GetWorkspaceId());
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            await AuditOnAfterSaveChanges(auditEntries);
            return result;
        }
        
        //https://www.codingame.com/playgrounds/5514/multi-tenant-asp-net-core-4---applying-tenant-rules-to-all-enitites
        // ReSharper disable once InconsistentNaming
        private static IList<Type> urlEntityTypeCache;
        private static IList<Type> GetEntityTypes()
        {
            if (urlEntityTypeCache != null)
            {
                return urlEntityTypeCache.ToList();
            }
            try
            {
                urlEntityTypeCache = new List<Type>();
                var assemblies = GetReferencingAssemblies();
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        foreach (var type in assembly.DefinedTypes)
                        {
                            if (type.BaseType == typeof(IAppEntity) || type.BaseType == typeof(IAppWorkspaceEntity)
                            || type.BaseType == typeof(AppEntity) || type.BaseType == typeof(AppWorkspaceEntity))
                                urlEntityTypeCache.Add(type.AsType());
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {
                
            }
            return urlEntityTypeCache;
        }
        
        private static IEnumerable<Assembly> GetReferencingAssemblies()
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;

            foreach (var library in dependencies)
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
                catch (System.IO.FileNotFoundException ex) 
                {
                    Console.WriteLine($"Assembly {library.Name} not found: " + ex.Message);
                }
            }
            return assemblies;
        }
        // Applying BaseEntity rules to all entities that inherit from it and Define MethodInfo member that is used when model is built.
        static readonly MethodInfo SetTenantGlobalQueryMethod = 
            typeof(AppDbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance).Single(t => 
            t.IsGenericMethod && t.Name == "SetTenantGlobalQuery");
        
        static readonly MethodInfo SetWorkspaceGlobalQueryMethod = 
            typeof(AppDbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance).Single(t => 
                t.IsGenericMethod && t.Name == "SetWorkspaceGlobalQuery");

        // This method is called for every loaded entity type in OnModelCreating method.
        // Here type is known through generic parameter and we can use EF Core methods.
        public void SetTenantGlobalQuery<T>(ModelBuilder builder) where T : AppEntity
        {
            builder.Entity<T>().HasKey(e => e.Id);
            builder.Entity<T>().HasQueryFilter(e => e.TenantId == _httpContextAccessor.HttpContext.GetTenantId());
        }
        
        public void SetWorkspaceGlobalQuery<T>(ModelBuilder builder) where T : AppWorkspaceEntity
        {
            builder.Entity<T>().HasKey(e => e.Id);
            builder.Entity<T>().HasQueryFilter(e => 
                e.TenantId == _httpContextAccessor.HttpContext.GetTenantId()
                && e.WorkspaceId == _httpContextAccessor.HttpContext.GetWorkspaceId());
        }
    }
}
