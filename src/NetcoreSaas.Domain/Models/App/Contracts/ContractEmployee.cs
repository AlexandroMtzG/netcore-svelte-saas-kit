using System;
using NetcoreSaas.Domain.Models.App.Employees;
using NetcoreSaas.Domain.Models.Core;

namespace NetcoreSaas.Domain.Models.App.Contracts
{
    public class ContractEmployee : MasterEntity
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
