using System;
using System.Collections.Generic;
using NetcoreSaas.Application.Dtos.Core;
using NetcoreSaas.Application.Dtos.Core.Links;
using NetcoreSaas.Domain.Enums.App.Contracts;

namespace NetcoreSaas.Application.Dtos.App.Contracts
{
    public class ContractDto : MasterEntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid LinkId { get; set; }
        public LinkDto Link { get; set; }
        public bool HasFile { get; set; }
        public ContractStatus Status { get; set; }
        public ICollection<ContractMemberDto> Members { get; set; }
        public ICollection<ContractEmployeeDto> Employees { get; set; }
        public ICollection<ContractActivityDto> Activity { get; set; }
    }
}