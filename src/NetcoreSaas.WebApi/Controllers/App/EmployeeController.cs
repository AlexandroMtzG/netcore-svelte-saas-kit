using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetcoreSaas.Application.Contracts.App.Employees;
using NetcoreSaas.Application.Dtos.App.Employees;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Domain.Models.App.Employees;

namespace NetcoreSaas.WebApi.Controllers.App
{
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAppUnitOfWork _appUnitOfWork;

        public EmployeeController(IMapper mapper, IAppUnitOfWork appUnitOfWork)
        {
            _mapper = mapper;
            _appUnitOfWork = appUnitOfWork;
        }

        [HttpGet(ApiAppRoutes.Employee.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var records = await _appUnitOfWork.Employees.GetAllAsync();
            if (!records.Any())
                return NoContent();
            return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(records));
        }

        [HttpGet(ApiAppRoutes.Employee.Get)]
        public IActionResult Get(Guid id)
        {
            var record = _appUnitOfWork.Employees.GetById(id);
            if (record == null)
                return NotFound();

            return Ok(_mapper.Map<EmployeeDto>(record));
        }

        [HttpPost(ApiAppRoutes.Employee.Create)]
        public async Task<IActionResult> Create([FromBody] EmployeeDto request)
        {
            var record = _mapper.Map<Employee>(request);
            _appUnitOfWork.Employees.Add(record);
            if (await _appUnitOfWork.CommitAsync() == 0)
                return BadRequest();

            return Ok(record);
        }
        
        [HttpPost(ApiAppRoutes.Employee.CreateMultiple)]
        public async Task<IActionResult> CreateMultiple([FromBody] CreateEmployeesRequest request)
        {
            var emails = request.Employees.GroupBy(f => f.Email);
            foreach (var email in emails)
            {
                if (email.ToList().Count > 1)
                    return BadRequest("api.errors.duplicatedEmail");
            }
            var created = new List<Employee>();
            foreach (var employee in request.Employees)
            {
                var existing = await _appUnitOfWork.Employees.GetByEmail(employee.Email);
                if (existing != null)
                    return BadRequest("Existing employee with email: " + existing.Email);
                
                var record = _mapper.Map<Employee>(employee);
                _appUnitOfWork.Employees.Add(record);
                
                created.Add(record);
            }
            if (await _appUnitOfWork.CommitAsync() == 0)
                return BadRequest();
            
            return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(created));
        }

        [HttpPut(ApiAppRoutes.Employee.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request)
        {
            var existing = _appUnitOfWork.Employees.GetById(id);
            if (existing == null)
                return NotFound();

            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.Email = request.Email;
            
            if (await _appUnitOfWork.CommitAsync() == 0)
                return BadRequest();

            existing = _appUnitOfWork.Employees.GetById(id);
            return Ok(existing);
        }

        [HttpDelete(ApiAppRoutes.Employee.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var record = _appUnitOfWork.Employees.GetById(id);
            if (record == null)
                return NotFound();

            _appUnitOfWork.Employees.Remove(record);
            if (await _appUnitOfWork.CommitAsync() == 0)
                return BadRequest();

            return NoContent();
        }

        // [Authorize(Roles = "Admin")]
        // [HttpDelete(ApiRoutes.Employee.AdminDelete)]
        // public async Task<IActionResult> AdminDelete(Guid id)
        // {
        //     var record = _masterUnitOfWork.Employees.GetById(id);
        //     if (record == null)
        //         return NotFound();
        //
        //     _masterUnitOfWork.Employees.Remove(record);
        //     if (await _masterUnitOfWork.CommitAsync() == 0)
        //         return BadRequest();
        //
        //     return NoContent();
        // }
    }
}
