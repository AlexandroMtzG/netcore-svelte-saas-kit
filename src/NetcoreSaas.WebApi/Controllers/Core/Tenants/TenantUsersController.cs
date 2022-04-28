using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetcoreSaas.Application.Contracts.Core.Tenants;
using NetcoreSaas.Application.Dtos.Core.Tenants;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Application.Services.Core;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Infrastructure.Extensions;

namespace NetcoreSaas.WebApi.Controllers.Core.Tenants
{
    [ApiController]
    [Authorize]
    public class TenantUsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly IWorkspaceService _workspaceService;

        public TenantUsersController(IMapper mapper, IMasterUnitOfWork masterUnitOfWork, IWorkspaceService workspaceService)
        {
            _mapper = mapper;
            _masterUnitOfWork = masterUnitOfWork;
            _workspaceService = workspaceService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet(ApiCoreRoutes.TenantUsers.AdminGetAll)]
        public async Task<IActionResult> AdminGetAll()
        {
            var users = await _masterUnitOfWork.Users.GetAllUsersWithTenants();
            return Ok(_mapper.Map<IEnumerable<User>, IEnumerable<UserDto>>(users));
        }

        [HttpGet(ApiCoreRoutes.TenantUsers.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var tenantUsers = await _masterUnitOfWork.Tenants.GetTenantUsers(HttpContext.GetTenantId());
            return Ok(_mapper.Map<IEnumerable<TenantUserDto>>(tenantUsers));
        }

        [HttpGet(ApiCoreRoutes.TenantUsers.Get)]
        public async Task<IActionResult> Get(Guid id)
        {
            var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(id);
            if (tenantUser == null)
                return NotFound();
            
            tenantUser.User.Workspaces = (await _masterUnitOfWork.Workspaces.GetAllUserWorkspaces(tenantUser)).ToList();
            
            return Ok(_mapper.Map<TenantUserDto>(tenantUser));
        }

        [HttpPut(ApiCoreRoutes.TenantUsers.ResetToken)]
        public async Task<IActionResult> ResetToken(Guid id)
        {
            var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(id);
            if (tenantUser == null)
                return NotFound();

            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            if (currentUser.Id != tenantUser.UserId)
                return BadRequest("api.errors.unauthorized");
            
            // tenantUser.ChatbotToken = new Guid();
            await _masterUnitOfWork.CommitAsync();
            
            return Ok(_mapper.Map<TenantUserDto>(tenantUser));
        }

        [HttpPut(ApiCoreRoutes.TenantUsers.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] TenantUserUpdateRequest request)
        {
            if (request.Workspaces.Count == 0)
                return BadRequest("api.errors.atLeastOneWorkspace");
            
            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            var currentTenant = HttpContext.GetTenantId();
            var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(id);
            if (tenantUser == null)
                return NotFound();
            var tenantUsers = await _masterUnitOfWork.Tenants.GetTenantUsers(tenantUser.Tenant.Id);
            var onlyOneOwner = tenantUsers.Count(f => f.Role == TenantUserRole.Owner) == 1;

            if (tenantUser.Role == TenantUserRole.Owner && request.Role != TenantUserRole.Owner && onlyOneOwner)
                return BadRequest("api.errors.cannotBeWithoutOwner");
            
            if (!currentUser.CanUpdateUserInTenant(tenantUser, currentTenant))
                return BadRequest("api.errors.unauthorized");

            if (currentUser.MembershipFromTenant(currentTenant).Role == TenantUserRole.Owner)
                tenantUser.Role = request.Role;
            
            // Only Owners can update Role, unless it's an admin, it can only update roles other than Owner
            else if (currentUser.MembershipFromTenant(currentTenant).Role == TenantUserRole.Admin && request.Role != TenantUserRole.Owner)
                tenantUser.Role = request.Role;

            var user = _masterUnitOfWork.Users.GetById(tenantUser.UserId);
            user.Phone = request.Phone;

            await _workspaceService.UpdateUserWorkspaces( tenantUser, request.Workspaces);
           
            await _masterUnitOfWork.CommitAsync();
            tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(id);

            return Ok(_mapper.Map<TenantUserDto>(tenantUser));
        }

        [HttpDelete(ApiCoreRoutes.TenantUsers.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(id);
            if (tenantUser == null)
                return NotFound();
            
            var currentTenant = HttpContext.GetTenantId();
            if (currentTenant == Guid.Empty || currentTenant != tenantUser.Tenant.Id)
                return BadRequest("api.errors.unauthorized");

            var tenantUsers = _masterUnitOfWork.Tenants.GetTenantUsers(currentTenant).Result.ToList();
            if (tenantUsers.Count == 1)
                return BadRequest("api.errors.cannotBeWithoutMembers");
            if (tenantUsers.Count(f => f.Role == TenantUserRole.Owner) <= 1 && tenantUser.Role == TenantUserRole.Owner)
                BadRequest("api.errors.cannotBeWithoutOwner");

            try
            {
                _masterUnitOfWork.Tenants.RemoveUser(tenantUser);
                await _masterUnitOfWork.CommitAsync();
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
    
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete(ApiCoreRoutes.TenantUsers.AdminDelete)]
        public async Task<IActionResult> AdminDelete(Guid id)
        {
            var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(id);
            if (tenantUser == null)
                return NotFound();
            try
            {
                _masterUnitOfWork.Tenants.RemoveUser(tenantUser);
                await _masterUnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}
