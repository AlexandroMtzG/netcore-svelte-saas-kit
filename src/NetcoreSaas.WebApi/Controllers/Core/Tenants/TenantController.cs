using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetcoreSaas.Application.Contracts.Core.Tenants;
using NetcoreSaas.Application.Dtos.Core.Tenants;
using NetcoreSaas.Application.Services.Core.Subscriptions;
using NetcoreSaas.Application.Services.Core.Tenants;
using NetcoreSaas.Application.Services.Core.Users;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.App.Usages;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;
using NetcoreSaas.Domain.Enums.Shared;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Domain.Models.Core.Workspaces;
using NetcoreSaas.Infrastructure.Extensions;

namespace NetcoreSaas.WebApi.Controllers.Core.Tenants
{
    [ApiController]
    [Authorize]
    public class TenantController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly ITenantService _tenantService;
        private readonly IUserService _userService;
        private readonly ISubscriptionService _subscriptionService;

        public TenantController(IMapper mapper, IMasterUnitOfWork masterUnitOfWork, ITenantService tenantService, IUserService userService, ISubscriptionService subscriptionService)
        {
            _mapper = mapper;
            _masterUnitOfWork = masterUnitOfWork;
            _tenantService = tenantService;
            _userService = userService;
            _subscriptionService = subscriptionService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet(ApiCoreRoutes.Tenant.AdminGetAll)]
        public async Task<IActionResult> AdminGetAll()
        {
            var tenants = await _masterUnitOfWork.Tenants.GetTenantsWithUsers();
            return Ok(_mapper.Map<IEnumerable<TenantDto>>(tenants));
        }

        [HttpGet(ApiCoreRoutes.Tenant.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var user = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            var myTenants = await _tenantService.GetUserTenants(user);
            if (myTenants.ToList().Count == 0)
                return NoContent();

            var tenantsDto = _mapper.Map<IEnumerable<TenantSimpleDto>>(myTenants);
            return Ok(tenantsDto);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet(ApiCoreRoutes.Tenant.AdminGetFeatures)]
        public async Task<IActionResult> GetFeatures(Guid id)
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(id);
            if (tenant == null)
                return NotFound();
            var features = await _tenantService.GetFeatures(tenant);
            return Ok(features);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet(ApiCoreRoutes.Tenant.AdminGetProducts)]
        public async Task<IActionResult> AdminGetProducts(Guid id)
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(id);
            if (tenant == null)
                return NotFound();
            var features = await _masterUnitOfWork.Tenants.GetTenantProducts(tenant, false);
            return Ok(features);
        }

        [HttpGet(ApiCoreRoutes.Tenant.GetFeatures)]
        public async Task<IActionResult> GetFeatures()
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(HttpContext.GetTenantId());
            var features = await _tenantService.GetFeatures(tenant);
            return Ok(features);
        }
        
        [HttpGet(ApiCoreRoutes.Tenant.GetCurrentUsage)]
        public async Task<IActionResult> GetCurrentUsage(AppUsageType type)
        {
            try
            {
                var tenant = _masterUnitOfWork.Tenants.GetById(HttpContext.GetTenantId());
                tenant.Workspaces = (await _masterUnitOfWork.Workspaces.GetTenantWorkspaces(tenant.Id)).ToList();
                var resumen = (await _tenantService.GetTenantUsageSummary(tenant, type));
                return Ok(resumen);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet(ApiCoreRoutes.Tenant.Get)]
        public async Task<IActionResult> Get(Guid id)
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(id);
            if (tenant == null)
                return NotFound();

            return Ok(_mapper.Map<TenantSimpleDto>(tenant));
        }

        [HttpGet(ApiCoreRoutes.Tenant.GetCurrent)]
        public IActionResult Get()
        {
            var tenant = _masterUnitOfWork.Tenants.GetById(HttpContext.GetTenantId());
            if (tenant == null)
                return NotFound();

            return Ok(_mapper.Map<TenantSimpleDto>(tenant));
        }

        [HttpPost(ApiCoreRoutes.Tenant.Create)]
        public async Task<IActionResult> Create([FromBody] TenantCreateRequest request)
        {
            if (request.SelectedSubscription.SubscriptionPriceId == Guid.Empty)
                return BadRequest("api.errors.invalidSubscription");
            var price = await _masterUnitOfWork.Subscriptions.GetPrice(request.SelectedSubscription.SubscriptionPriceId);
            if (price == null)
                return BadRequest("api.errors.invalidSubscription");
            var email = HttpContext.GetUserEmail();
            var user = await _masterUnitOfWork.Users.GetByEmailAsync(email);
            try
            {
                var tenant = await _tenantService.AddNewTenant(request.Name, user.Email, request.SelectedSubscription, request.Subdomain);
                
                var workspace = new Workspace()
                {
                    TenantId = tenant.Id,
                    Name = request.Name,
                };
                var workspaceUser = new WorkspaceUser()
                {
                    WorkspaceId = workspace.Id,
                    UserId = user.Id,
                    Default = true,
                    Role = Role.Administrator
                };
                _masterUnitOfWork.Workspaces.Add(workspace);
                _masterUnitOfWork.Workspaces.AddWorkspaceUser(workspaceUser);
                _masterUnitOfWork.Tenants.AddNewUser(tenant, user, false, TenantUserRole.Owner, TenantUserJoined.Creator, TenantUserStatus.Active);
                _masterUnitOfWork.Tenants.Add(tenant);
                _masterUnitOfWork.Users.ChangeUserDefaultTenant(user, tenant);
                await _masterUnitOfWork.CommitAsync();

                return Ok(await _userService.Authenticate(user));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut(ApiCoreRoutes.Tenant.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] TenantDto modifiedTenant)
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(id);
            if (tenant == null)
                return NotFound();

            tenant.Name = modifiedTenant.Name;
            tenant.Subdomain = modifiedTenant.Subdomain;

            await _masterUnitOfWork.CommitAsync();

            if (string.IsNullOrEmpty(tenant.SubscriptionCustomerId) == false)
            {
                await _subscriptionService.UpdateCustomer(tenant);
            }
            tenant = await _masterUnitOfWork.Tenants.Get(id);
            return Ok(_mapper.Map<TenantSimpleDto>(tenant));
        }

        [HttpPut(ApiCoreRoutes.Tenant.UpdateImage)]
        public async Task<IActionResult> UpdateImage(Guid id, [FromBody] TenantUpdateImageRequest request)
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(id);
            switch (request.Type)
            {
                case "icon":
                    tenant.Icon = request.Icon;
                    break;
                case "logo":
                    tenant.Logo = request.Logo;
                    break;
                case "logoDarkmode":
                    tenant.LogoDarkmode = request.LogoDarkmode;
                    break;
                default:
                    return BadRequest();
            }
            await _masterUnitOfWork.CommitAsync();
            return Ok(_mapper.Map<TenantDto>(tenant));
        }

        [HttpDelete(ApiCoreRoutes.Tenant.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(id);
            if (tenant == null)
                return NotFound();
            var user = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());

            var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(tenant.Id, user.Id);
            var tenantUsers = (await _masterUnitOfWork.Tenants.GetTenantUsers(tenant.Id)).ToList();
            if (tenantUsers.Count == 1)
                throw new Exception("api.errors.cannotBeWithoutMembers");
            if (tenantUsers.Count(f => f.Role == TenantUserRole.Owner) <= 1 && tenantUser.Role == TenantUserRole.Owner)
                throw new Exception("api.errors.cannotBeWithoutOwner");

            if (tenantUser == null || tenantUser.Role != TenantUserRole.Admin)
                return BadRequest("api.errors.unauthorized");

            await _tenantService.CancelAndDelete(tenant);
            await _masterUnitOfWork.CommitAsync();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete(ApiCoreRoutes.Tenant.AdminDelete)]
        public async Task<IActionResult> AdminDelete(Guid id)
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(id);
            if (tenant == null)
                return NotFound();
            var user = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(tenant.Id, user.Id);
            if (user.Type != UserType.Admin && (tenantUser == null || tenantUser.Role != TenantUserRole.Admin))
                return BadRequest("api.errors.unauthorized");

            await _tenantService.CancelAndDelete(tenant);
            await _masterUnitOfWork.CommitAsync();

            return Ok();
        }
    }
}
