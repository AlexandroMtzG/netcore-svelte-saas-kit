using System;
using NetcoreSaas.Domain.Enums.App.Contracts;
using NetcoreSaas.Domain.Models.Core;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Domain.Models.App.Contracts
{
    public class ContractActivity : MasterEntity
    {
        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }
        public Guid ContractId { get; set; }
        public Contract Contract { get; set; }
        public ContractActivityType Type { get; set; }
    }
}