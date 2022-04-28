using System;
using NetcoreSaas.Domain.Enums.App.Contracts;
using NetcoreSaas.Domain.Models.Core;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Domain.Models.App.Contracts
{
    public class ContractMember : MasterEntity
    {
        public Guid ContractId { get; set; }
        public Contract Contract { get; set; }
        public ContractMemberRole Role { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string WorkspaceName;
    }
}
