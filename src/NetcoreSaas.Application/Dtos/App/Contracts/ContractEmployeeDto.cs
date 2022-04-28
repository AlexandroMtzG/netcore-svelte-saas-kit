using System;
using NetcoreSaas.Application.Dtos.App.Employees;
using NetcoreSaas.Application.Dtos.Core;

namespace NetcoreSaas.Application.Dtos.App.Contracts
{
    public class ContractEmployeeDto : MasterEntityDto
    {
        public Guid EmployeeId { get; set; }
        public EmployeeDto Employee { get; set; }
    }
}
