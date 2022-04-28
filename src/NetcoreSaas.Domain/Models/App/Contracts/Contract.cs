using System;
using System.Collections.Generic;
using NetcoreSaas.Domain.Enums.App.Contracts;
using NetcoreSaas.Domain.Models.Core;
using NetcoreSaas.Domain.Models.Core.Links;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Domain.Models.App.Contracts
{
    public class Contract : MasterEntity
    {
        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid LinkId { get; set; }
        public Link Link { get; set; }
        public string File { get; set; }
        public ContractStatus Status { get; set; }
        public ICollection<ContractMember> Members { get; set; }
        public ICollection<ContractEmployee> Employees { get; set; }
        public ICollection<ContractActivity> Activity { get; set; }

        public Contract()
        {
            Members = new List<ContractMember>();
            Employees = new List<ContractEmployee>();
            Activity = new List<ContractActivity>();
        }

        public byte[] GetFileData()
        {
            var file = File.Replace("data:application/pdf;base64,", "").Replace('-', '+').Replace('_', '/');
            return Convert.FromBase64String(file);
        }
    }
}