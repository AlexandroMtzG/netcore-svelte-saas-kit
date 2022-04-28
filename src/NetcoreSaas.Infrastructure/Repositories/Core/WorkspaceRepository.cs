using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetcoreSaas.Application.Repositories.App;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Domain.Models.Core.Workspaces;
using NetcoreSaas.Infrastructure.Data;

namespace NetcoreSaas.Infrastructure.Repositories.Core
{
    public class WorkspaceRepository : AppRepository<Workspace>, IWorkspaceRepository
    {
        public WorkspaceRepository(BaseDbContext context) : base(context)
        {
            
        }

        public async Task<IEnumerable<Workspace>> GetTenantWorkspaces(Guid tenantId)
        {
            return await Context.Workspaces
                .Where(f => f.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Workspace>> GetUserWorkspaces(TenantUser tenantUser, Guid? tenantId = null)
        {
            var list = Context.Workspaces
                // .Include(f=>f.Transactions)
                .Include(f=>f.Tenant)
                .Include(f => f.Users)
                .AsQueryable();
            
            if (tenantId.HasValue)
                list = list.Where(f => f.TenantId == tenantId.Value);

            list = list.Select(f => new Workspace(){
                Id = f.Id,
                Name = f.Name,
                Type = f.Type,
                BusinessMainActivity = f.BusinessMainActivity,
                RegistrationNumber = f.RegistrationNumber,
                RegistrationDate = f.RegistrationDate,
                Default = f.Default,
                CreatedByUser = f.CreatedByUser != null ? new User()
                {
                    Id = f.CreatedByUser.Id,
                    FirstName = f.CreatedByUser.FirstName,
                    LastName = f.CreatedByUser.LastName,
                    Email = f.CreatedByUser.Email,
                } : null,
                Users = f.Users.Select(x => new WorkspaceUser()
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    WorkspaceId = x.WorkspaceId,
                    Role = x.Role,
                    Default = x.Default,
                    User = x.User != null ? new User()
                    {
                        Id = x.User.Id,
                        FirstName = x.User.FirstName,
                        LastName = x.User.LastName,
                        Email = x.User.Email,
                    } : null
                }).ToList()
            });
            
            if (!new[]{TenantUserRole.Admin, TenantUserRole.Owner}.Contains(tenantUser.Role))
            {
                list = list.Where(f => f.Users.Any(x => x.UserId == tenantUser.UserId));
            }

            return await list.ToListAsync();
        }

        public async Task<IEnumerable<WorkspaceUser>> GetAllUserWorkspaces(TenantUser tenantUser)
        {
            return await Context.WorkspaceUsers
                .Include(f=>f.Workspace)
                .Where(f => f.UserId == tenantUser.UserId && f.Workspace.TenantId == tenantUser.TenantId)
                .ToListAsync();
        }

        public async Task<Workspace> GetWorkspaceByName(TenantUser tenantUser, string workspaceName)
        {
            return await Context.Workspaces.FirstOrDefaultAsync(f => f.Name == workspaceName.ToUpper());
        }

        public async Task<Workspace> Get(Guid id)
        {
            return await Context.Workspaces
                .Include(f => f.Users)
                .ThenInclude(f=>f.User)
                .Where(f => f.Id == id)
                .FirstOrDefaultAsync();
        }
        
        public async Task<WorkspaceUser> GetWorkspaceUserById(Guid id)
        {
            return await Context.WorkspaceUsers
                .Include(f => f.User)
                .Where(f => f.Id == id)
                .FirstOrDefaultAsync();
        }
        

        public async Task<IEnumerable<WorkspaceUser>> GetUsers(Guid workspaceId)
        {
            return await Context.WorkspaceUsers
                .Include(f => f.User)
                .Where(f => f.WorkspaceId == workspaceId)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<WorkspaceUser>> GetUsersWithWorkspace(Guid workspaceId)
        {
            return await Context.WorkspaceUsers
                .Include(f => f.User)
                .Include(f => f.Workspace)
                .Where(f => f.WorkspaceId == workspaceId)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<WorkspaceUser>> GetMembersWithTenant(Guid workspaceId)
        {
            return await Context.WorkspaceUsers
                .Include(f => f.User)
                .Include(f=>f.Workspace)
                .ThenInclude(f=>f.Tenant)
                .Where(f => f.WorkspaceId == workspaceId)
                .ToListAsync();
        }
        
        public void AddWorkspaceUser(WorkspaceUser workspaceUser)
        {
            Context.WorkspaceUsers.Add(workspaceUser);
        }
        
        public void RemoveWorkspaceUser(WorkspaceUser workspaceUser)
        {
            Context.WorkspaceUsers.Remove(workspaceUser);
        }
    }
}
