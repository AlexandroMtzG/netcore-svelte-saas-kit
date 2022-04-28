using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetcoreSaas.Application.Contracts.Core.Links;
using NetcoreSaas.Application.Dtos.Core.Links;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Application.Services.Core;
using NetcoreSaas.Application.Services.Messages;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.App.Links;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Shared;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Domain.Models.Core.Workspaces;
using NetcoreSaas.Infrastructure.Extensions;

namespace NetcoreSaas.WebApi.Controllers.Core.Workspaces
{
    [ApiController]
    [Authorize]
    public class LinkController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly ILinkService _linkService;
        private readonly IEmailService _emailService;

        public LinkController(IMapper mapper, IMasterUnitOfWork masterUnitOfWork, ILinkService linkService, IEmailService emailService)
        {
            _mapper = mapper;
            _masterUnitOfWork = masterUnitOfWork;
            _linkService = linkService;
            _emailService = emailService;
        }

        [HttpGet(ApiCoreRoutes.Link.GetAllLinked)]
        public async Task<IActionResult> GetAllLinked()
        {
            var records = await _masterUnitOfWork.Links.GetAll(HttpContext.GetWorkspaceId(), LinkStatus.Linked);
            if (!records.Any())
                return NoContent();
            return Ok(_mapper.Map<IEnumerable<LinkDto>>(records));
        }
        
        [HttpGet(ApiCoreRoutes.Link.GetAllPending)]
        public async Task<IActionResult> GetAllPending()
        {
            var records = await _masterUnitOfWork.Links.GetAll(HttpContext.GetWorkspaceId(), LinkStatus.Pending);
            if (!records.Any())
                return NoContent();
            return Ok(_mapper.Map<IEnumerable<LinkDto>>(records));
        }
        
        [HttpGet(ApiCoreRoutes.Link.GetAllProviders)]
        public async Task<IActionResult> GetAllProviders()
        {
            var records = await _masterUnitOfWork.Links.GetAllProviders(HttpContext.GetWorkspaceId());
            if (!records.Any())
                return NoContent();
            return Ok(records);
        }
        
        [HttpGet(ApiCoreRoutes.Link.GetAllClients)]
        public async Task<IActionResult> GetAllClients()
        {
            var records = await _masterUnitOfWork.Links.GetAllClients(HttpContext.GetWorkspaceId());
            if (!records.Any())
                return NoContent();
            return Ok(_mapper.Map<IEnumerable<LinkDto>>(records));
        }
        
        [HttpGet(ApiCoreRoutes.Link.GetLinkUsers)]
        public async Task<IActionResult> GetAllClients(Guid linkId)
        {
            var link = await _masterUnitOfWork.Links.Get(HttpContext.GetWorkspaceId(), linkId);
            
            var users = new List<WorkspaceUser>();

            var providerUsers = (await _masterUnitOfWork.Workspaces.GetMembersWithTenant(link.ProviderWorkspaceId)).ToList();
            var clientUsers = (await _masterUnitOfWork.Workspaces.GetMembersWithTenant(link.ClientWorkspaceId)).ToList();

            foreach (var providerUser in providerUsers)
            {
                var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(providerUser.Workspace.TenantId, providerUser.UserId);
                if (tenantUser != null && providerUser.Role != Role.Guest && tenantUser.Role != TenantUserRole.Guest)
                {
                    users.Add(providerUser);
                }
            }
            foreach (var clientUser in clientUsers)
            {
                var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(clientUser.Workspace.TenantId, clientUser.UserId);
                if (tenantUser != null && clientUser.Role != Role.Guest && tenantUser.Role != TenantUserRole.Guest)
                {
                    users.Add(clientUser);
                }
            }
            
            return Ok(_mapper.Map<IEnumerable<WorkspaceUserDto>>(users));
        }

        [AllowAnonymous]
        [HttpGet(ApiCoreRoutes.Link.GetInvitation)]
        public async Task<IActionResult> GetInvitation(Guid id)
        {
            var record = await _masterUnitOfWork.Links.GetInvitation(id);
            if (record == null)
                return NoContent();
            
            return Ok(_mapper.Map<LinkInvitationDto>(record));
        }
        
        [HttpPost(ApiCoreRoutes.Link.CreateInvitation)]
        public async Task<IActionResult> CreateInvitation([FromBody] LinkInvitationDto request)
        {
            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            var currentWorkspace = _masterUnitOfWork.Workspaces.GetById(HttpContext.GetWorkspaceId());
            
            try
            {
                var invitation = await _linkService.InviteUser(currentUser, currentWorkspace, request);
                return Ok(_mapper.Map<LinkInvitationDto>(invitation));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet(ApiCoreRoutes.Link.GetWorkspace)]
        public async Task<IActionResult> GetWorkspace(Guid linkId)
        {
            try
            {
                var workspace = await _linkService.GetWorkspace(HttpContext.GetWorkspaceId(), linkId);
                if (workspace == null)
                    return NotFound();

                return Ok(_mapper.Map<WorkspaceDto>(workspace));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet(ApiCoreRoutes.Link.Get)]
        public async Task<IActionResult> Get(Guid id)
        {
            var record = await _masterUnitOfWork.Links.Get(HttpContext.GetWorkspaceId(), id);
            if (record == null)
                return NotFound();

            return Ok(_mapper.Map<LinkDto>(record));
        }
        
        [HttpGet(ApiCoreRoutes.Link.SearchUser)]
        public async Task<IActionResult> Get(string email)
        {
            var record = await _masterUnitOfWork.Users.GetByEmailAsync(email);
            if (record == null)
                return NotFound();

            return Ok(_mapper.Map<UserSimpleDto>(record));
        }
        
        [HttpGet(ApiCoreRoutes.Link.SearchMember)]
        public async Task<IActionResult> SearchMember(string email, string workspaceName)
        {
            var user = await _masterUnitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
                return NotFound();

            try
            {
                var record = await _linkService.SearchMember(user, workspaceName);
                if (record == null)
                    return BadRequest("api.errors.userNotInWorkspace");

                return Ok(_mapper.Map<UserSimpleDto>(record));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet(ApiCoreRoutes.Link.GetMember)]
        public async Task<IActionResult> GetMember(Guid linkId, string email)
        {
            var user = await _masterUnitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
                return NotFound();
            
            var link = await _masterUnitOfWork.Links.Get(HttpContext.GetWorkspaceId(), linkId);
            if (link == null)
                return NotFound();
            
            if (link.Status != LinkStatus.Linked)
                return BadRequest("api.errors.notLinked");
            
            var record = await _linkService.GetMember(link.ProviderWorkspaceId == HttpContext.GetWorkspaceId() ? link.ClientWorkspace : link.ProviderWorkspace, email);
            if (record == null)
                return NotFound();

            return Ok(_mapper.Map<UserSimpleDto>(record));
        }
        
        [AllowAnonymous]
        [HttpPost(ApiCoreRoutes.Link.RejectInvitation)]
        public async Task<IActionResult> RejectInvitation(Guid id)
        {
            var link = await _masterUnitOfWork.Links.GetInvitation(id);
            if (link == null)
                return NotFound();

            link.Status = LinkStatus.Rejected;
            await _masterUnitOfWork.CommitAsync();
            
            await _emailService.SendLinkInvitationRejected(link.Email, link.WorkspaceName, link.CreatedByUser);
            
            return Ok();
        }

        [HttpPost(ApiCoreRoutes.Link.Create)]
        public async Task<IActionResult> Create([FromBody] CreateLinkRequest request)
        {
            request.Email = request.Email.ToLower();
            var user = await _masterUnitOfWork.Users.GetByEmailAsync(request.Email.ToLower());
            if (user == null)
                return BadRequest($"User {request.Email} not found");

            if (request.AsProvider)
            {
                var existingLinkWithProvider =
                    await _masterUnitOfWork.Links.GetProvider(HttpContext.GetWorkspaceId(), request.WorkspaceName);
                if (existingLinkWithProvider is { Status: LinkStatus.Linked })
                    return BadRequest("api.errors.alreadyLinkedProvider");
            }
            else
            {
                var existingLinkWithClient =
                    await _masterUnitOfWork.Links.GetClient(HttpContext.GetWorkspaceId(), request.WorkspaceName);
                if (existingLinkWithClient is { Status: LinkStatus.Linked })
                    return BadRequest("api.errors.alreadyLinkedClient");
            }

            var userTenants = await _masterUnitOfWork.Users.GetUserTenants(user);
            Workspace workspace = null;
            foreach (var tenant in userTenants)
            {
                var workspaces = await _masterUnitOfWork.Workspaces.GetTenantWorkspaces(tenant.TenantId);
                workspace = workspaces.FirstOrDefault(f => f.Name == request.WorkspaceName);
                if (workspace != null)
                    break;
            }
            if (workspace == null)
                return BadRequest($"User {request.Email} does not have workspace {request.WorkspaceName}");
            
            try
            {
                var currentWorkspace = _masterUnitOfWork.Workspaces.GetById(HttpContext.GetWorkspaceId());
                var link = new Link()
                {
                    CreatedByUserId = HttpContext.GetUserId(),
                    CreatedByWorkspaceId = currentWorkspace.Id,
                    ProviderWorkspaceId = !request.AsProvider ? HttpContext.GetWorkspaceId() : workspace.Id,
                    ClientWorkspaceId = request.AsProvider ? HttpContext.GetWorkspaceId() : workspace.Id,
                    Status = LinkStatus.Pending
                };
                _masterUnitOfWork.Links.Add(link);

                await _masterUnitOfWork.CommitAsync();
                
                var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
                await _emailService.SendInviteUserToLinkWorkspace(currentUser, currentWorkspace, link, user, workspace, request.AsProvider);

                return Ok(_mapper.Map<LinkDto>(link));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut(ApiCoreRoutes.Link.AcceptOrReject)]
        public async Task<IActionResult> AcceptOrReject(Guid id, [FromBody] UpdateLinkRequest request)
        {
            var existing = await _masterUnitOfWork.Links.Get(HttpContext.GetWorkspaceId(), id);
            if (existing == null)
                return NotFound();
            
            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            var currentWorkspace = _masterUnitOfWork.Workspaces.GetById(HttpContext.GetWorkspaceId());
            
            existing.Status = request.Accepted ? LinkStatus.Linked : LinkStatus.Rejected;

            if (await _masterUnitOfWork.CommitAsync() == 0)
                return BadRequest();
            
            if (request.Accepted)
                await _emailService.SendLinkInvitationAccepted(currentUser, existing, existing.ProviderWorkspaceId == HttpContext.GetWorkspaceId());
            else
                await _emailService.SendLinkInvitationRejected(currentUser.Email, currentWorkspace.Name,  existing.CreatedByUser);

            existing = await _masterUnitOfWork.Links.Get(HttpContext.GetWorkspaceId(), id);
            return Ok(_mapper.Map<LinkDto>(existing));
        }

        [HttpDelete(ApiCoreRoutes.Link.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var record = await _masterUnitOfWork.Links.Get(HttpContext.GetWorkspaceId(), id);
            if (record == null)
                return NotFound();

            _masterUnitOfWork.Links.Remove(record);
            if (await _masterUnitOfWork.CommitAsync() == 0)
                return BadRequest();

            return NoContent();
        }

        // [Authorize(Roles = "Admin")]
        // [HttpDelete(ApiRoutes.Link.AdminDelete)]
        // public async Task<IActionResult> AdminDelete(Guid id)
        // {
        //     var record = _masterUnitOfWork.Links.GetById(id);
        //     if (record == null)
        //         return NotFound();
        //
        //     _masterUnitOfWork.Links.Remove(record);
        //     if (await _masterUnitOfWork.CommitAsync() == 0)
        //         return BadRequest();
        //
        //     return NoContent();
        // }
    }
}
