using System;
using NetcoreSaas.Application.Dtos.App.Employees;

namespace NetcoreSaas.Application.Contracts.App.Contracts
{
    public class CreateContractRequest
    {
        public Guid LinkId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string File { get; set; }
        public AddContractMemberDto[] Members { get; set; }
        public EmployeeDto[] Employees { get; set; }
        public DateTime? EstimatedStartDate { get; set; }
        public DateTime? RealStartDate { get; set; }
        public DateTime? EstimatedTerminationDate { get; set; }
        public DateTime? RealTerminationDate { get; set; }
    }
}