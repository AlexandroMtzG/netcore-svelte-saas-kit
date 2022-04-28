using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetcoreSaas.Application.Contracts.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Application.Helpers;
using NetcoreSaas.Application.Services.Core.Tenants;
using NetcoreSaas.Application.Services.Core.Users;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Infrastructure.Extensions;

namespace NetcoreSaas.WebApi.Controllers.Core.Users
{
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly IUserService _userService;
        private readonly ITenantService _tenantService;

        public UserController(IMapper mapper, IMasterUnitOfWork masterUnitOfWork, IUserService userService, ITenantService tenantService)
        {
            _mapper = mapper;
            _masterUnitOfWork = masterUnitOfWork;
            _userService = userService;
            _tenantService = tenantService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet(ApiCoreRoutes.User.AdminGetAll)]
        public IActionResult GetAll()
        {
            var users = _masterUnitOfWork.Users.GetAllUsersWithTenants().Result;
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }
        
        [HttpGet(ApiCoreRoutes.User.GetClaims)]
        public IActionResult GetClaims()
        {
            if (!HttpContext.User.Claims.Any())
                return NotFound();
            
            return Ok(HttpContext.User.Claims.Select(f => f.Type.ToString() + ": " + f.Value.ToString()));
        }

        [HttpGet(ApiCoreRoutes.User.GetUser)]
        public async Task<IActionResult> Get(Guid id)
        {
            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            var userLogged = await _userService.GetUserWithTenant(id);
            if (userLogged == null)
                return NotFound();

            if(currentUser.Type == UserType.Tenant)
            {
                var currentTenant = HttpContext.GetTenantId();
                var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(currentTenant, userLogged.User.Id);
                if (tenantUser == null)
                    return BadRequest("api.errors.unauthorized");
            }

            return Ok(userLogged.User);
        }
        
        [HttpGet(ApiCoreRoutes.User.GetUserAvatar)]
        public IActionResult GetUserAvatar(Guid id)
        {
            var user = _masterUnitOfWork.Users.GetById(id);
            if (user == null)
                return NotFound();
            
            return Ok(user.Avatar);
        }
        
        [HttpGet(ApiCoreRoutes.User.GetCurrent)]
        public async Task<IActionResult> GetCurrent()
        {
            var userLogged = await _userService.GetUserWithTenant(HttpContext.GetUserId());
            if (userLogged == null)
                return NotFound();

            if(userLogged.User.Type == UserType.Tenant)
            {
                var currentTenant = HttpContext.GetTenantId();
                var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(currentTenant, userLogged.User.Id);
                if (tenantUser == null)
                    return BadRequest("api.errors.unauthorized");
            }

            return Ok(userLogged);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost(ApiCoreRoutes.User.AdminUpdatePassword)]
        public async Task<IActionResult> AdminUpdatePassword(Guid userId, string password)
        {
            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            if (currentUser.Type != UserType.Admin)
                return BadRequest("api.errors.unauthorized");

            var user = _masterUnitOfWork.Users.GetById(userId);
            user.Password = PasswordHasher.HashPassword(password);
            await _masterUnitOfWork.CommitAsync();

            return Ok();
        }

        [HttpPost(ApiCoreRoutes.User.UpdateDefaultTenant)]
        public async Task<IActionResult> UpdateDefaultTenant(Guid userId, Guid tenantId)
        {
            var user = _masterUnitOfWork.Users.GetById(userId);
            var tenant = await _masterUnitOfWork.Tenants.Get(tenantId);
            if (tenant == null)
                return NotFound();
            
            _masterUnitOfWork.Users.ChangeUserDefaultTenant(user, tenant);
            await _masterUnitOfWork.CommitAsync();
            
            // await _userService.Authenticate(user);
            
            // var tenantDto = _mapper.Map<TenantDto>(tenant);
            // tenantDto.CurrentUser = tenantDto.Users.SingleOrDefault(f => f.UserId == user.Id);

            return Ok(await _userService.Authenticate(user, tenantId));
        }

        [HttpPost(ApiCoreRoutes.User.UpdateAvatar)]
        public async Task<IActionResult> UpdateAvatar([FromBody] UserUpdateAvatarRequest request)
        {
            var user = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            if (user == null)
                return NotFound();

            user.Avatar = request.Avatar;
            await _masterUnitOfWork.CommitAsync();

            return Ok(_mapper.Map<UserDto>(user));
        }
        
        
        [HttpPost(ApiCoreRoutes.User.UpdateLocale)]
        public async Task<IActionResult> UpdateLocale([FromBody] UserUpdateLocaleRequest request)
        {
            var user = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            if (user == null)
                return NotFound();

            user.Locale = request.Locale;
            
            if (await _masterUnitOfWork.CommitAsync() == 0)
                return BadRequest();

            return Ok();
        }

        [HttpPut(ApiCoreRoutes.User.Update)]
        public async Task<IActionResult> Update([FromBody] UserUpdateRequest request)
        {
            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            var user = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            if (user == null)
                return NotFound();

            if (currentUser.Type == UserType.Tenant)
            {
                var currentTenant = HttpContext.GetTenantId();
                var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(currentTenant, user.Id);
                if (tenantUser == null)
                    return BadRequest("api.errors.unauthorized");
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Phone = request.Phone;

            await _masterUnitOfWork.CommitAsync();

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost(ApiCoreRoutes.User.UpdatePassword)]
        public async Task<IActionResult> UpdatePassword([FromBody] UserUpdatePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.PasswordNew) || request.PasswordNew != request.PasswordConfirm)
                return BadRequest("api.errors.passwordMismatch");

            var user = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            if (user == null)
                return NotFound();
            if (user.LoginType == UserLoginType.Password || string.IsNullOrEmpty(user.Password) == false)
            {
                if (PasswordHasher.VerifyHashedPassword(user.Password, request.PasswordCurrent) != PasswordVerificationResult.Success)
                {
                    return BadRequest("api.errors.invalidPassword");
                }
            }

            user.Password = PasswordHasher.HashPassword(request.PasswordNew);
            if (await _masterUnitOfWork.CommitAsync() == 0)
                return BadRequest();

            return Ok();
        }
        
        [HttpDelete(ApiCoreRoutes.User.DeleteMe)]
        public async Task<IActionResult> DeleteMe()
        {
            var user = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            if (user == null)
                return NotFound();
            if (user.Type == UserType.Admin)
                return BadRequest("api.errors.cannotDeleteAdmin");
            
            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            if (currentUser.Id != user.Id && currentUser.Type != UserType.Admin)
                return BadRequest("api.errors.unauthorized");

            var tenantUsers = await _masterUnitOfWork.Users.GetUserTenants(user);
            foreach (var tenantUser in tenantUsers)
            {
                if (tenantUser.Role == TenantUserRole.Owner)
                {
                    await _tenantService.CancelAndDelete(tenantUser.Tenant);
                    var contracts = _masterUnitOfWork.Contracts.GetByCreatedUser(HttpContext.GetUserId()).ToList();
                    foreach (var contract in contracts)
                        _masterUnitOfWork.Contracts.Remove(contract);
                }

                _masterUnitOfWork.Tenants.RemoveUser(tenantUser);
            }
            
            _masterUnitOfWork.Users.RemoveUser(user);
            await _masterUnitOfWork.CommitAsync();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete(ApiCoreRoutes.User.AdminDelete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = _masterUnitOfWork.Users.GetById(id);
            if (user == null)
                return NotFound();
            
            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            if (currentUser.Id != user.Id && currentUser.Type != UserType.Admin)
                return BadRequest("api.errors.unauthorized");

            var tenantUsers = await _masterUnitOfWork.Users.GetUserTenants(user);
            foreach (var tenantUser in tenantUsers)
            {
                if (tenantUser.Role == TenantUserRole.Owner)
                    await _tenantService.CancelAndDelete(tenantUser.Tenant);
                _masterUnitOfWork.Tenants.RemoveUser(tenantUser);
            }
            _masterUnitOfWork.Users.RemoveUser(user);
            await _masterUnitOfWork.CommitAsync();

            return Ok();
        }
    }
}
