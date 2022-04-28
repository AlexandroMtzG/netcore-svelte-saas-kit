using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Application.Services.Core;
using NetcoreSaas.Application.Services.Core.Tenants;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Shared;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Infrastructure.Services.Core
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly ITenantService _tenantService;
        public WorkspaceService(IMasterUnitOfWork masterUnitOfWork, ITenantService tenantService)
        {
            _masterUnitOfWork = masterUnitOfWork;
            _tenantService = tenantService;
        }

        public async Task<Workspace> AddWorkspace(TenantUser tenatUser, Tenant tenant, string name)
        {
            var features = await _tenantService.GetFeatures(tenant);
            var workspaces = (await _masterUnitOfWork.Workspaces.GetUserWorkspaces(tenatUser)).ToList();
            
            if(features.MaxWorkspaces <= workspaces.Count())
                throw new Exception("api.errors.limitReachedWorkspaces");
            
            if(workspaces.Any(f=>f.Name == name.ToUpper()))
                return null;
            
            var company = new Workspace()
            {
                TenantId = tenant.Id,
                Name = name,
            };
            _masterUnitOfWork.Workspaces.Add(company);
            await _masterUnitOfWork.CommitAsync();

            return company;
        }

        public async Task UpdateUserWorkspaces( TenantUser tenantUser, List<WorkspaceDto> workspaces)
        {
            var tenantWorkspaces = await _masterUnitOfWork.Workspaces.GetTenantWorkspaces(tenantUser.TenantId);
            var userWorkspaces = (await _masterUnitOfWork.Workspaces.GetAllUserWorkspaces(tenantUser)).ToList();
            foreach (var tenantWorkspace in tenantWorkspaces)
            {
                var currentMember = userWorkspaces.FirstOrDefault(f => f.WorkspaceId == tenantWorkspace.Id);
                var setMember = workspaces.FirstOrDefault(f => f.Id == tenantWorkspace.Id);
                if (currentMember != null)
                {
                    // existing
                    if (setMember == null)
                    {
                        // delete
                        _masterUnitOfWork.Workspaces.RemoveWorkspaceUser(currentMember);
                    }
                }
                else
                {
                    // not a member
                    if (setMember != null)
                    {
                        // add
                        var workspaceUser = new WorkspaceUser()
                        {
                            WorkspaceId = tenantWorkspace.Id,
                            UserId = tenantUser.UserId,
                            Default = true,
                            Role = tenantUser.Role switch
                            {
                                TenantUserRole.Admin => Role.Administrator,
                                TenantUserRole.Owner => Role.Administrator,
                                TenantUserRole.Member => Role.Member,
                                _ => Role.Guest
                            }
                        };
                        _masterUnitOfWork.Workspaces.AddWorkspaceUser(workspaceUser);
                    }
                }
            }
        }

        public async Task UpdateWorkspaceUsers(Workspace workspace, List<UserDto> users)
        {
            var tenantUsers = await _masterUnitOfWork.Tenants.GetTenantUsers(workspace.TenantId);
            var workspaceUsers = (await _masterUnitOfWork.Workspaces.GetUsers(workspace.Id)).ToList();
            foreach (var tenantUser in tenantUsers)
            {
                var currentMember = workspaceUsers.FirstOrDefault(f => f.UserId == tenantUser.UserId);
                var setWorkspace = users.FirstOrDefault(f => f.Id == tenantUser.UserId);
                if (currentMember != null)
                {
                    // existing
                    if (setWorkspace == null)
                    {
                        // delete
                        _masterUnitOfWork.Workspaces.RemoveWorkspaceUser(currentMember);
                    }
                }
                else
                {
                    // not a member
                    if (setWorkspace != null)
                    {
                        // add
                        var workspaceUser = new WorkspaceUser()
                        {
                            WorkspaceId = workspace.Id,
                            UserId = tenantUser.UserId,
                            Default = true,
                            Role = tenantUser.Role switch
                            {
                                TenantUserRole.Admin => Role.Administrator,
                                TenantUserRole.Owner => Role.Administrator,
                                TenantUserRole.Member => Role.Member,
                                _ => Role.Guest
                            }
                        };
                        _masterUnitOfWork.Workspaces.AddWorkspaceUser(workspaceUser);
                    }
                }
            }
        }
    }
}
