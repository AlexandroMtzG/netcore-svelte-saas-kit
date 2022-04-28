using System;
using NetcoreSaas.Domain.Enums.App.Contracts;

namespace NetcoreSaas.Application.Contracts.App.Contracts
{
    public class AddContractMemberDto
    {
        public Guid UserId { get; set; }
        public ContractMemberRole Role { get; set; }
    }
}