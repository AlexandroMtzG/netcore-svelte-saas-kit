using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NetcoreSaas.Application.Contracts.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Tenants;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Application.Helpers;
using NetcoreSaas.Application.Services.Core.Tenants;
using NetcoreSaas.Application.Services.Core.Users;
using NetcoreSaas.Application.Services.Messages;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.App.Links;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;
using NetcoreSaas.Domain.Enums.Shared;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Infrastructure.Services.Core
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _conf;
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly ITenantService _tenantService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IMapper mapper, IConfiguration conf, IMasterUnitOfWork masterUnitOfWork, ITenantService tenantService, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _conf = conf;
            _masterUnitOfWork = masterUnitOfWork;
            _tenantService = tenantService;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserLoggedResponse> Authenticate(User user, Guid? tenantId = null)
        {
            var userLogged = await GetUserWithTenant(user.Id, tenantId);
            userLogged.Token = GenerateJsonWebToken(userLogged.User, _conf["Jwt:Key"], _conf["Jwt:Issuer"]);
            return userLogged;
        }

        public async Task<User> Register(UserRegisterRequest request, LinkInvitation linkInvitation)
        {
            var user = new User()
            {
                Email = request.Email.ToLower().Trim(),
                Type = UserType.Tenant,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = request.Password,
                Phone = request.Phone,
                LoginType = request.LoginType
            };
            var tenant = await _tenantService.AddNewTenant(request.WorkspaceName, request.Email, request.SelectedSubscription);
            if (string.IsNullOrEmpty(request.Password) == false)
            {
                user.Password = PasswordHasher.HashPassword(request.Password);
            }
            var workspace = new Workspace()
            {
                TenantId = tenant.Id,
                Name = request.WorkspaceName,
                Type = request.WorkspaceType,
                Default = true
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
            _masterUnitOfWork.Users.Add(user);

            if (linkInvitation != null)
            {
                var link = new Link()
                {
                    CreatedByUser = linkInvitation.CreatedByUser,
                    CreatedByUserId = linkInvitation.CreatedByUserId,
                    CreatedByWorkspaceId = linkInvitation.CreatedByWorkspaceId,
                    ProviderWorkspaceId = linkInvitation.InviteeIsProvider ? workspace.Id : linkInvitation.CreatedByWorkspaceId,
                    ClientWorkspaceId = !linkInvitation.InviteeIsProvider ? workspace.Id : linkInvitation.CreatedByWorkspaceId,
                    Status = LinkStatus.Linked
                };
                linkInvitation.CreatedLinkId = link.Id;
                linkInvitation.CreatedLink = link;
                linkInvitation.Status = LinkStatus.Linked;
                _masterUnitOfWork.Links.Add(link);
            }
            
            await _masterUnitOfWork.CommitAsync();
            if (linkInvitation?.CreatedLink != null)
            {
                await _emailService.SendLinkInvitationAccepted(user, linkInvitation.CreatedLink, linkInvitation.InviteeIsProvider);    
            }
            
            await _emailService.SendWelcome(user);

            return user;
        }

        public async Task<UserLoggedResponse> GetUserWithTenant(Guid id, Guid? tenantId = null)
        {
            var user = _masterUnitOfWork.Users.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            var tenants = (await _tenantService.GetUserTenants(user)).ToList();
            if (tenants == null || !tenants.Any())
                throw new Exception("api.errors.noTenants");
            // var tenantDtos = _mapper.Map<IEnumerable<Tenant>, IEnumerable<TenantDto>>(tenants).ToList();

            Tenant defaultTenant = null;
            if (tenantId.HasValue)
            {
                defaultTenant = tenants.SingleOrDefault(f => f.Id == tenantId);
            } 
            else if (user.DefaultTenantId.HasValue)
            {
                try
                {
                    defaultTenant = tenants.SingleOrDefault(f => f.Id == user.DefaultTenantId);
                }
                catch
                {
                    // ignored
                }
            }
            if (defaultTenant == null && tenants.Count > 0)
                defaultTenant = tenants.First();

            if (defaultTenant != null)
            {
                defaultTenant.CurrentUser = defaultTenant.Users.SingleOrDefault(f => f.UserId == userDto.Id);
                defaultTenant.Workspaces = (await _masterUnitOfWork.Workspaces.GetUserWorkspaces(defaultTenant.CurrentUser, defaultTenant.Id)).ToList();
            }
            

            Workspace defaultWorkspace = null;
            if (defaultTenant != null)
            {
                defaultWorkspace = defaultTenant.Workspaces.FirstOrDefault(f => f.Default) ??
                                   defaultTenant.Workspaces.FirstOrDefault();
                if (defaultWorkspace != null && defaultWorkspace.Tenant == null)
                    defaultWorkspace.Tenant = defaultTenant;
            }
            
            userDto.CurrentTenant = _mapper.Map<TenantSimpleDto>(defaultTenant);
            var userLogged = new UserLoggedResponse()
            {
                User = userDto,
                DefaultWorkspace = _mapper.Map<WorkspaceDto>(defaultWorkspace)
            };
            
            return userLogged;
        }

        private string GenerateJsonWebToken(UserDto user, string jwtKey, string jwtIssuer)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>() {
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim(ClaimTypes.Role, user.Type.ToString()),
            };
            if (user.CurrentTenant != null)
            {
                claims.Add(new Claim("TenantId", user.CurrentTenant.Id.ToString()));
                claims.Add(new Claim("TenantUuid", user.CurrentTenant.Uuid.ToString()));
                claims.Add(new Claim("TenantUserId", user.CurrentTenant.CurrentUser.UserId.ToString()));
                claims.Add(new Claim("TenantUserRole", user.CurrentTenant.CurrentUser.Role.ToString()));
            }

            var token = new JwtSecurityToken(
              jwtIssuer,
              jwtIssuer,
              claims.ToArray(),
              expires: DateTime.Now.AddMonths(12),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Guid? GetClaim(string name)
        {
            if (_httpContextAccessor.HttpContext == null) return null;
            var claims = _httpContextAccessor.HttpContext.User.Claims;
            var value = claims.FirstOrDefault(f => f.Type == name)?.Value;
            if (!string.IsNullOrEmpty(value)) 
                return Guid.Parse(value);

            return null;
        }

        public void UpdateClaim(string name, object value)
        {
            if (!(_httpContextAccessor.HttpContext?.User.Identity is ClaimsIdentity userIdentity)) return;
            userIdentity.RemoveClaim(userIdentity.FindFirst(name));
            userIdentity.AddClaim(new Claim(name, value.ToString() ?? ""));
        }
    }
}
