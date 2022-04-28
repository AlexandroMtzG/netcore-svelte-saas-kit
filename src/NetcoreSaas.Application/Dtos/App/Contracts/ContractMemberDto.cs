using System;
using NetcoreSaas.Application.Dtos.Core;
using NetcoreSaas.Application.Dtos.Core.Users;
using NetcoreSaas.Domain.Enums.App.Contracts;

namespace NetcoreSaas.Application.Dtos.App.Contracts
{
    public class ContractMemberDto : MasterEntityDto
    {
        public Guid ContractId { get; set; }
        public ContractDto Contract { get; set; }
        public ContractMemberRole Role { get; set; }
        public Guid UserId { get; set; }
        public UserDto User { get; set; }
    }
}
