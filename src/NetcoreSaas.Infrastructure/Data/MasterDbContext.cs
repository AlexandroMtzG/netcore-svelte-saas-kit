using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Linq;
using System;
using NetcoreSaas.Application.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;
using NetcoreSaas.Domain.Enums.Shared;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Infrastructure.Data
{
    public class MasterDbContext : BaseDbContext
    {
        private readonly IConfiguration _conf;
        private readonly IWebHostEnvironment _env;

        public MasterDbContext(DbContextOptions<MasterDbContext> options, IConfiguration conf = null, IWebHostEnvironment env = null) : base(options)
        {
            _conf = conf;
            _env = env;

            // ReSharper disable once VirtualMemberCallInConstructor
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.UseIdentityColumns();

            SeedUsers(builder);
        }

        private void SeedUsers(ModelBuilder builder)
        {
            var defaultUsers = _conf.GetSection("DefaultUsers")
                .GetChildren()
               .ToList()
               .Select(x =>
                     (
                      x.GetValue<string>("Uuid"),
                      x.GetValue<string>("Organization"),
                      x.GetValue<string>("Subdomain"),
                      x.GetValue<string>("Type"),
                      x.GetValue<string>("Email"),
                      x.GetValue<string>("Password"),
                      x.GetValue<string>("FirstName"),
                      x.GetValue<string>("LastName")
                      )
                )
                .ToList<(string Uuid, string Organization, string Subdomain, string Type, string Email, string Password, string FirstName, string LastName)>();
            foreach (var defaultUser in defaultUsers)
            {
                var tenant = new Tenant()
                {
                    Id = Guid.NewGuid(),
                    Uuid = Guid.Parse(defaultUser.Uuid),
                    Name = defaultUser.Organization,
                    Subdomain = defaultUser.Subdomain
                };
                var type = UserType.Admin;
                if (defaultUser.Type == "Admin") 
                {
                    type = UserType.Admin;
                } else if (defaultUser.Type == "Tenant") 
                {
                    type = UserType.Tenant;
                }
                var user = new User()
                {
                    Id = Guid.NewGuid(),
                    Email = defaultUser.Email,
                    Type = type,
                    Password = PasswordHasher.HashPassword(defaultUser.Password),
                    LoginType = UserLoginType.Password,
                    FirstName = defaultUser.FirstName,
                    LastName = defaultUser.LastName
                };
                var defaultWorkspace = new Workspace()
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenant.Id,
                    Name = "Default Workspace"
                };
                var workspaceUser = new WorkspaceUser()
                {
                    UserId = user.Id,
                    WorkspaceId = defaultWorkspace.Id,
                    Role = Role.Administrator
                };
                builder.Entity<Workspace>().HasData(defaultWorkspace);
                builder.Entity<WorkspaceUser>().HasData(workspaceUser);
                builder.Entity<User>().HasData(user);
                builder.Entity<Tenant>().HasData(tenant);
                builder.Entity<TenantUser>().HasData(new TenantUser()
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenant.Id,
                    UserId = user.Id,
                    Role = TenantUserRole.Owner,
                    Joined = TenantUserJoined.Creator,
                    Status = TenantUserStatus.Active
                });
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_env.IsDevelopment())
                optionsBuilder.EnableSensitiveDataLogging();
            if (ProjectConfiguration.GlobalConfiguration != null)
            {
                switch (ProjectConfiguration.GlobalConfiguration.DatabaseProvider)
                {
                    case DatabaseProvider.PostgreSql:
                        optionsBuilder!.UseNpgsql(ProjectConfiguration.GlobalConfiguration.MasterDbContext,
                            b => b.MigrationsAssembly("NetcoreSaas.WebApi"));
                        break;
                    case DatabaseProvider.MySql:
                        optionsBuilder!.UseMySql(ProjectConfiguration.GlobalConfiguration.MasterDbContext, ServerVersion.AutoDetect(ProjectConfiguration.GlobalConfiguration.MasterDbContext),
                            b => b.MigrationsAssembly("NetcoreSaas.WebApi"));
                        break;
                }
            }
        }

        public override int SaveChanges()
        {
            var auditEntries = OnBeforeSaveChanges();
            AddTenantToProperties();
            var result = base.SaveChanges();
            AuditOnAfterSaveChanges(auditEntries);
            return result;
        }
        
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, System.Threading.CancellationToken cancellationToken = default)
        {
            var auditEntries = OnBeforeSaveChanges();
            AddTenantToProperties();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            await AuditOnAfterSaveChanges(auditEntries);
            return result;
        }
    }
}
