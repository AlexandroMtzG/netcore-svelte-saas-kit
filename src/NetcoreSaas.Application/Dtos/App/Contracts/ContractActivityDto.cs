using System;
using NetcoreSaas.Application.Dtos.Core;
using NetcoreSaas.Domain.Enums.App.Contracts;

namespace NetcoreSaas.Application.Dtos.App.Contracts
{
    public class ContractActivityDto : MasterEntityDto
    {
        public Guid ContractId { get; set; }
        public ContractDto Contract { get; set; }
        public ContractActivityType Type { get; set; }
    }
}