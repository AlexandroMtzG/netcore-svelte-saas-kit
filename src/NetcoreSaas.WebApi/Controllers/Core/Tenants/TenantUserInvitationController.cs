using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetcoreSaas.Application.Contracts.Core.Tenants;
using NetcoreSaas.Application.Contracts.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Tenants;
using NetcoreSaas.Application.Helpers;
using NetcoreSaas.Application.Services.Core;
using NetcoreSaas.Application.Services.Core.Users;
using NetcoreSaas.Application.Services.Messages;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Infrastructure.Extensions;

namespace NetcoreSaas.WebApi.Controllers.Core.Tenants
{
    [ApiController]
    [Authorize]
    public class TenantUserInvitationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IWorkspaceService _workspaceService;

        public TenantUserInvitationController(IMapper mapper, IMasterUnitOfWork masterUnitOfWork, IUserService userService, IEmailService emailService, IWorkspaceService workspaceService)
        {
            _mapper = mapper;
            _masterUnitOfWork = masterUnitOfWork;
            _userService = userService;
            _emailService = emailService;
            _workspaceService = workspaceService;
        }

        [AllowAnonymous]
        [HttpGet(ApiCoreRoutes.TenantUserInvitation.GetInvitation)]
        public async Task<IActionResult> GetInvitation(Guid invitationLink)
        {
            var invitation = await _masterUnitOfWork.Tenants.GetTenantUserByInvitationLink(invitationLink);
            if (invitation == null || invitation.Status != TenantUserStatus.PendingInvitation)
                return NotFound();

            var tenant = _masterUnitOfWork.Tenants.GetById(invitation.TenantId);
            return Ok(new TenantInvitationResponse(
                invitation: _mapper.Map<TenantUserDto>(invitation), 
                tenant: _mapper.Map<TenantDto>(tenant), 
                requiresVerify: invitation.RequiresVerify()
            ));
        }

        [AllowAnonymous]
        [HttpGet(ApiCoreRoutes.TenantUserInvitation.GetInviteUrl)]
        public async Task<IActionResult> GetInviteUrl(Guid linkUuid)
        {
            var tenantJoinSettings = await _masterUnitOfWork.Tenants.GetJoinSettingsByLink(linkUuid);
            if (tenantJoinSettings == null) return NotFound();
            var tenant = await _masterUnitOfWork.Tenants.Get(tenantJoinSettings.TenantId);
            var tenantDto = _mapper.Map<Tenant, TenantDto>(tenant);
            return Ok(tenantDto);
        }

        [AllowAnonymous]
        [HttpGet(ApiCoreRoutes.TenantUserInvitation.GetInvitationSettings)]
        public async Task<IActionResult> GetInvitationSettings(Guid tenantId)
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(tenantId);
            if (tenant.TenantJoinSettings == null)
            {
                _masterUnitOfWork.Tenants.AddJoinSettings(tenant);
                await _masterUnitOfWork.CommitAsync();
            }
            tenant = await _masterUnitOfWork.Tenants.Get(tenantId);
            return Ok(_mapper.Map<TenantJoinSettingsDto>(tenant.TenantJoinSettings));
        }

        [HttpPost(ApiCoreRoutes.TenantUserInvitation.InviteUser)]
        public async Task<IActionResult> InviteUser([FromBody] UserInviteRequest request)
        {
            if (request.Workspaces.Count == 0)
                return BadRequest("api.errors.atLeastOneWorkspace");
            
            var tenant = await _masterUnitOfWork.Tenants.Get(HttpContext.GetTenantId());
            
            var alreadyMember = await _masterUnitOfWork.Tenants.GetTenantUser(tenant.Id, request.Email);
            if (alreadyMember != null)
                return BadRequest("api.errors.existingUser");

            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());

            var invitedUser = await _masterUnitOfWork.Users.GetByEmailAsync(request.Email);
            if (invitedUser == null)
                invitedUser = _masterUnitOfWork.Users.AddNewUser(request.Email, UserType.Tenant, request.FirstName, request.LastName, request.Phone, UserLoginType.Password);

            var tenantUser = _masterUnitOfWork.Tenants.AddNewUser(tenant, invitedUser, invitedUser == null, request.Role, TenantUserJoined.JoinedByInvitation, TenantUserStatus.PendingInvitation);
            // foreach (var workspace in request.Workspaces)
            // {
            //     var workspaceUser = new WorkspaceUser()
            //     {
            //         WorkspaceId = workspace.Id,
            //         UserId = invitedUser.Id,
            //         Default = true,
            //         Role = request.Role switch
            //         {
            //             TenantUserRole.Admin => Role.Administrator,
            //             TenantUserRole.Owner => Role.Administrator,
            //             TenantUserRole.Member => Role.Member,
            //             _ => Role.Guest
            //         }
            //     };
            //     _masterUnitOfWork.Workspaces.AddWorkspaceUser(workspaceUser);
            // }
            await _workspaceService.UpdateUserWorkspaces( tenantUser, request.Workspaces);
            
            tenantUser.InvitationLink = Guid.NewGuid();
            await _masterUnitOfWork.CommitAsync();
            tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(tenantUser.Id);
            await _emailService.SendUserInvitation(tenant, currentUser, tenantUser);

            return Ok(_mapper.Map<TenantUserDto>(tenantUser));
        }

        [AllowAnonymous]
        [HttpPost(ApiCoreRoutes.TenantUserInvitation.RequestAccess)]
        public async Task<IActionResult> RequestAccess(Guid linkUuid, [FromBody] UserVerifyRequest request)
        {
            var tenantJoinSettings = await _masterUnitOfWork.Tenants.GetJoinSettingsByLink(linkUuid);
            if (tenantJoinSettings == null)
                return NotFound();

            var tenant = await _masterUnitOfWork.Tenants.Get(tenantJoinSettings.TenantId);
            var tenantDto = _mapper.Map<TenantDto>(tenant);

            if (tenant.IsAdmin() == false && (tenantDto.Products == null || tenantDto.Products.Count == 0))
                return BadRequest("shared.noSubscription");
            else if (tenant.HasMaxNumberOfUsers())
                return BadRequest("api.errors.maxNumberOfUsers");
            
            var user = await _masterUnitOfWork.Users.GetByEmailAsync(request.Email);
            TenantUser tenantUser = null;
            var userStatus = TenantUserStatus.Active;
            if (tenantJoinSettings.RequireAcceptance)
            {
                userStatus = TenantUserStatus.PendingAcceptance;
            }
            if (user == null)
            {
                user = _masterUnitOfWork.Users.AddNewUser(request.Email, UserType.Tenant, request.FirstName, request.LastName, request.Phone, UserLoginType.Password, PasswordHasher.HashPassword(request.Password), Guid.NewGuid());
                await _masterUnitOfWork.CommitAsync();
                _masterUnitOfWork.Users.ChangeUserDefaultTenant(user, tenant);
            }
            else
            {
                if (string.IsNullOrEmpty(request.Password) == false)
                {
                    var result = PasswordHasher.VerifyHashedPassword(user.Password, request.Password);
                    if (result != PasswordVerificationResult.Success)
                    {
                        return BadRequest("api.errors.invalidPassword");
                    }
                }
                tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(tenant.Id, user.Id);
                if (tenantUser != null && tenantUser.Status == TenantUserStatus.Active)
                {
                    _masterUnitOfWork.Users.ChangeUserDefaultTenant(user, tenant);
                    await _masterUnitOfWork.CommitAsync();
                    return Ok(_mapper.Map<TenantUserDto>(tenantUser));
                }
            }
            tenantUser = _masterUnitOfWork.Tenants.AddNewUser(tenant, user, false, TenantUserRole.Member, TenantUserJoined.JoinedByInvitation, userStatus);
            if (userStatus == TenantUserStatus.Active)
            {
                _masterUnitOfWork.Users.ChangeUserDefaultTenant(user, tenant);
            }
            else
            {
                if (tenantUser.Status == TenantUserStatus.PendingAcceptance)
                {
                    await _emailService.SendRequestedAccess(tenantUser, tenant);
                }
            }
            await _masterUnitOfWork.CommitAsync();

            // if (userStatus == TenantUserStatus.Active)
            // {
            //     return Ok(await _userService.Authenticate(user));
            // }

            return Ok(_mapper.Map<TenantUserDto>(tenantUser));
        }

        [AllowAnonymous]
        [HttpPost(ApiCoreRoutes.TenantUserInvitation.AcceptInvitation)]
        public async Task<IActionResult> AcceptInvitation(Guid invitationLink, [FromBody] UserVerifyRequest request)
        {
            var invitation = await _masterUnitOfWork.Tenants.GetTenantUserByInvitationLink(invitationLink);
            if (invitation == null || invitation.Status != TenantUserStatus.PendingInvitation)
                return NotFound();

            var user = await _masterUnitOfWork.Users.GetByEmailAsync(request.Email);

            if (string.IsNullOrEmpty(request.Password) == false)
            {
                user.Password = PasswordHasher.HashPassword(request.Password);
                user.Token = Guid.Empty;
            }
            if (!string.IsNullOrEmpty(request.FirstName))
                user.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName))
                user.LastName = request.LastName;

            user.DefaultTenantId = invitation.Tenant.Id;
            invitation.Joined = TenantUserJoined.JoinedByInvitation;
            invitation.Status = TenantUserStatus.Active;

            await _masterUnitOfWork.CommitAsync();

            return Ok(await _userService.Authenticate(user));
        }

        [HttpPost(ApiCoreRoutes.TenantUserInvitation.AcceptUser)]
        public async Task<IActionResult> AcceptUser(Guid tenantUserId, [FromBody] TenantUserDto request)
        {
            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(tenantUserId);
            if (tenantUser == null)
            {
                return NotFound();
            }
            var tenant = await _masterUnitOfWork.Tenants.Get(tenantUser.TenantId);
            if (tenant == null || tenant.TenantJoinSettings == null)
            {
                return NotFound();
            }

            if (!tenant.TenantJoinSettings.LinkActive || tenant.TenantJoinSettings.Link == Guid.Empty)
                return BadRequest("api.errors.invitationNotAvailable");
            
            if (request.Accepted)
            {
                tenantUser.Joined = TenantUserJoined.JoinedByLink;
                tenantUser.Status = TenantUserStatus.Active;
                _masterUnitOfWork.Users.ChangeUserDefaultTenant(tenantUser.User, tenantUser.Tenant);
                await _masterUnitOfWork.CommitAsync();
                
                await _emailService.SendUserAccepted(tenant, currentUser, tenantUser);

                return Ok(_mapper.Map<TenantUserDto>(tenantUser));
            }
            
            // Not accepted
            _masterUnitOfWork.Tenants.RemoveUser(tenantUser);
            await _masterUnitOfWork.CommitAsync();
            return NoContent();
        }

        [HttpPost(ApiCoreRoutes.TenantUserInvitation.UpdateInvitationSettings)]
        public async Task<IActionResult> UpdateInvitationSettings([FromBody] TenantUpdateJoinSettingsRequest request)
        {
            var tenant = await _masterUnitOfWork.Tenants.Get(HttpContext.GetTenantId());
            if (tenant.TenantJoinSettings == null)
            {
                _masterUnitOfWork.Tenants.AddJoinSettings(tenant, request.EnableLink, request.EnablePublicUrl, request.RequireAcceptance);
            }
            else
            {
                tenant.TenantJoinSettings.RequireAcceptance = request.RequireAcceptance;
                tenant.TenantJoinSettings.LinkActive = request.EnableLink;
                tenant.TenantJoinSettings.PublicUrl = request.EnablePublicUrl;
                if (request.ResetLink || (tenant.TenantJoinSettings.LinkActive && tenant.TenantJoinSettings.Link == Guid.Empty))
                {
                    tenant.TenantJoinSettings.Link = Guid.NewGuid();
                }
                //db.Entry(tenantJoinSettings).State = EntityState.Modified;
            }

            await _masterUnitOfWork.CommitAsync();
            return Ok(_mapper.Map<TenantJoinSettingsDto>(tenant.TenantJoinSettings));
        }
    }
}
