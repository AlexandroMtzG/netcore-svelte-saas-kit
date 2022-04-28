using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetcoreSaas.Application.Contracts.App.Contracts;
using NetcoreSaas.Application.Dtos.App.Contracts;
using NetcoreSaas.Application.Services.App;
using NetcoreSaas.Application.Services.Messages;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Enums.App.Contracts;
using NetcoreSaas.Domain.Enums.App.Links;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Infrastructure.Extensions;

namespace NetcoreSaas.WebApi.Controllers.App
{
    [ApiController]
    [Authorize]
    public class ContractController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly IContractService _contractService;
        private readonly IEmailService _emailService;

        public ContractController(IMapper mapper, IMasterUnitOfWork masterUnitOfWork, IContractService contractService, IEmailService emailService)
        {
            _mapper = mapper;
            _masterUnitOfWork = masterUnitOfWork;
            _contractService = contractService;
            _emailService = emailService;
        }

        [HttpGet(ApiAppRoutes.Contract.GetAllByStatusFilter)]
        public async Task<IActionResult> GetAllByStatusFilter(ContractStatusFilter filter)
        {
            var records = await _masterUnitOfWork.Contracts.GetAllByStatusFilter(HttpContext.GetWorkspaceId(), filter);
            if (!records.Any())
                return NoContent();
            
            return Ok(_mapper.Map<IEnumerable<ContractDto>>(records).OrderByDescending(f=>f.CreatedAt));
        }
        
        [HttpGet(ApiAppRoutes.Contract.GetAllByLink)]
        public async Task<IActionResult> GetAllByLink(Guid linkId)
        {
            var records = await _masterUnitOfWork.Contracts.GetAllByLink(HttpContext.GetWorkspaceId(), linkId);
            if (!records.Any())
                return NoContent();
            return Ok(_mapper.Map<IEnumerable<ContractDto>>(records));
        }

        [HttpGet(ApiAppRoutes.Contract.Get)]
        public async Task<IActionResult> Get(Guid id)
        {
            var record = await _masterUnitOfWork.Contracts.Get(HttpContext.GetWorkspaceId(), id);
            if (record == null)
                return NotFound();

            return Ok(_mapper.Map<ContractDto>(record));
        }
        
        [HttpPost(ApiAppRoutes.Contract.Create)]
        public async Task<IActionResult> Create([FromBody] CreateContractRequest request)
        {
            var mb = 4*Math.Ceiling(((double)request.File.Length/3));
            if((mb/1000) / 2 > 1024 * 20)
                return BadRequest("api.errors.maxFileReached");
            
            var link = await _masterUnitOfWork.Links.Get(HttpContext.GetWorkspaceId(), request.LinkId);
            if (link.Status != LinkStatus.Linked)
                return BadRequest("api.errors.notLinked");

            var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
            
            try
            {
                var record = await _contractService.Create(currentUser, link, request);
                return Ok(_mapper.Map<ContractDto>(record));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost(ApiAppRoutes.Contract.Download)]
        public async Task<IActionResult> Download(Guid id)
        {
            var record = await _masterUnitOfWork.Contracts.Get(HttpContext.GetWorkspaceId(), id);
            if (record == null)
                return NotFound();

            return Ok(record.File);
        }

        [HttpPost(ApiAppRoutes.Contract.Send)]
        public async Task<IActionResult> Send(Guid id, [FromBody] SendContractRequest request)
        {
            try
            {
                var contract = await _masterUnitOfWork.Contracts.Get(HttpContext.GetWorkspaceId(), id);
                if (contract == null)
                    return BadRequest();
                
                var currentUser = _masterUnitOfWork.Users.GetById(HttpContext.GetUserId());
                if (currentUser == null)
                    return BadRequest();
                
                var link = await _masterUnitOfWork.Links.Get(HttpContext.GetWorkspaceId(), contract.LinkId);
                
                await _emailService.SendContractNew(currentUser, link, contract);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut(ApiAppRoutes.Contract.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContractRequest request)
        {
            var existing = await _masterUnitOfWork.Contracts.Get(HttpContext.GetWorkspaceId(), id);
            if (existing == null)
                return NotFound();

            if (string.IsNullOrEmpty(request.Name))
                return BadRequest("api.errors.nameCannotBeEmpty");
            if (string.IsNullOrEmpty(request.Description))
                return BadRequest("api.errors.descriptionCannotBeEmpty");
            if (string.IsNullOrEmpty(request.File))
                return BadRequest("api.errors.fileCannotBeEmpty");

            existing.Name = request.Name;
            existing.Status = request.Status;
            existing.Description = request.Description;
            existing.File = request.File;
            await _masterUnitOfWork.CommitAsync();

            existing = await _masterUnitOfWork.Contracts.Get(HttpContext.GetWorkspaceId(), id);
            return Ok(_mapper.Map<ContractDto>(existing));
        }

        [HttpDelete(ApiAppRoutes.Contract.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var record = await _masterUnitOfWork.Contracts.Get(HttpContext.GetWorkspaceId(), id);
            if (record == null)
                return NotFound();

            _masterUnitOfWork.Contracts.Remove(record);
            
            if (await _masterUnitOfWork.CommitAsync() == 0)
                return BadRequest();

            return NoContent();
        }
    }
}
