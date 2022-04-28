using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetcoreSaas.Application.Contracts.Core.Workspaces;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Application.Services.Core;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Domain.Models.Core.Workspaces;
using NetcoreSaas.Infrastructure.Extensions;

namespace NetcoreSaas.WebApi.Controllers.Core.Workspaces
{
    [ApiController]
    [Authorize]
    public class WorkspaceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly IAppUnitOfWork _appUnitOfWork;
        private readonly IWorkspaceService _workspaceService;

        public WorkspaceController(IMapper mapper, IMasterUnitOfWork masterUnitOfWork, IAppUnitOfWork appUnitOfWork, IWorkspaceService workspaceService)
        {
            _mapper = mapper;
            _masterUnitOfWork = masterUnitOfWork;
            _appUnitOfWork = appUnitOfWork;
            _workspaceService = workspaceService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet(ApiCoreRoutes.Workspace.AdminGetAll)]
        public async Task<IActionResult> AdminGetAll()
        {
            var records = await _masterUnitOfWork.Workspaces.GetAllAsync();
            if (!records.Any())
                return NoContent();
            return Ok(_mapper.Map<IEnumerable<WorkspaceDto>>(records));
        }

        [HttpGet(ApiCoreRoutes.Workspace.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var tenantUser = await _masterUnitOfWork.Tenants.GetTenantUser(HttpContext.GetTenantId(), HttpContext.GetUserId());
            if (tenantUser == null)
                return BadRequest("api.errors.unauthorized");
            var records = await _appUnitOfWork.Workspaces.GetUserWorkspaces(tenantUser);
            return Ok(_mapper.Map<IEnumerable<WorkspaceDto>>(records));
        }

        [HttpGet(ApiCoreRoutes.Workspace.Get)]
        public async Task<IActionResult> Get(Guid id)
        {
            var record = await _appUnitOfWork.Workspaces.Get(id);
            if (record == null)
                return NotFound();

            return Ok(_mapper.Map<WorkspaceDto>(record));
        }

        [HttpPost(ApiCoreRoutes.Workspace.Create)]
        public async Task<IActionResult> Create([FromBody] CreateWorkspaceRequest request)
        {
            var workspace = new Workspace()
            {
                TenantId = HttpContext.GetTenantId(),
                CreatedByUserId = HttpContext.GetUserId(),
                CreatedAt = new DateTime(),
                Name = request.Name,
                Type = request.Type,
                BusinessMainActivity = request.BusinessMainActivity,
                RegistrationNumber = request.RegistrationNumber,
                RegistrationDate = request.RegistrationDate,
            };
            
            _masterUnitOfWork.Workspaces.Add(workspace);
            await _workspaceService.UpdateWorkspaceUsers(workspace, request.Users);

            if (await _masterUnitOfWork.CommitAsync() == 0)
                return BadRequest();

            return Ok(workspace);
        }
        
        [HttpPost(ApiCoreRoutes.Workspace.AddUser)]
        public async Task<IActionResult> AddUser([FromBody] WorkspaceUserDto request)
        {
            var company = await _appUnitOfWork.Workspaces.Get(request.WorkspaceId);
            if (company == null)
                return NotFound();
            
            var user = _masterUnitOfWork.Users.GetById(request.UserId);
            if (user == null)
                return NotFound();

            var companyUser = company.Users.FirstOrDefault(f => f.UserId == request.UserId);
            if (companyUser != null)
                return BadRequest("api.errors.alreadyAMember");
            
            var record = _mapper.Map<WorkspaceUser>(request);
            _appUnitOfWork.Workspaces.AddWorkspaceUser(record);
            if (await _appUnitOfWork.CommitAsync() == 0)
                return BadRequest("api.errors.noChanges");

            record.User = user;
            return Ok(_mapper.Map<WorkspaceUserDto>(record));
        }

        [HttpPut(ApiCoreRoutes.Workspace.RemoveUser)]
        public async Task<IActionResult> RemoveUser(Guid id)
        {
            var companyUser = await _appUnitOfWork.Workspaces.GetWorkspaceUserById(id);
            if (companyUser == null)
                return NotFound();
            
            var company = await _appUnitOfWork.Workspaces.Get(companyUser.WorkspaceId);
            if (company == null)
                return NotFound();

            var usuario = company.Users.SingleOrDefault(f => f.Id == id);
            company.Users.Remove(usuario);
            if (await _appUnitOfWork.CommitAsync() == 0)
                return BadRequest("api.errors.noChanges");

            return Ok();
        }

        [HttpPut(ApiCoreRoutes.Workspace.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkspaceRequest request)
        {
            var existing = _masterUnitOfWork.Workspaces.GetById(id);
            
            if (existing == null)
                return NotFound();
            
            existing.Name = request.Name;
            existing.Type = request.Type;
            existing.BusinessMainActivity = request.BusinessMainActivity;
            existing.RegistrationNumber = request.RegistrationNumber;
            existing.RegistrationDate = request.RegistrationDate;

            await _workspaceService.UpdateWorkspaceUsers(existing, request.Users);
            
            if (await _masterUnitOfWork.CommitAsync() == 0)
                return BadRequest("api.errors.noChanges");

            existing = _masterUnitOfWork.Workspaces.GetById(id);
            return Ok(existing);
        }

        [HttpPut(ApiCoreRoutes.Workspace.UpdateType)]
        public async Task<IActionResult> UpdateType(Guid id, WorkspaceType type)
        {
            var existing = _appUnitOfWork.Workspaces.GetById(id);
            if (existing == null)
                return NotFound();

            if (existing.Type == type)
                return Ok();

            existing.Type = type;
            
            if (await _appUnitOfWork.CommitAsync() == 0)
                return BadRequest("api.errors.noChanges");

            return Ok();
        }

        [HttpDelete(ApiCoreRoutes.Workspace.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var record = _appUnitOfWork.Workspaces.GetById(id);
            if (record == null)
                return NotFound();

            _appUnitOfWork.Workspaces.Remove(record);
            if (await _appUnitOfWork.CommitAsync() == 0)
                return BadRequest();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete(ApiCoreRoutes.Workspace.AdminDelete)]
        public async Task<IActionResult> AdminDelete(Guid id)
        {
            var record = _masterUnitOfWork.Workspaces.GetById(id);
            if (record == null)
                return NotFound();

            _masterUnitOfWork.Workspaces.Remove(record);
            if (await _masterUnitOfWork.CommitAsync() == 0)
                return BadRequest();

            return NoContent();
        }
    }
}
