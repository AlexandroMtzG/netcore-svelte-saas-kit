using System;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;
using NetcoreSaas.Domain.Models.Interfaces;

namespace NetcoreSaas.Domain.Models.Core
{
    public abstract class AppEntity : Entity, IAppEntity
    {
        public Guid? CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }
        public Guid? ModifiedByUserId { get; set; }
        public User ModifiedByUser { get; set; }
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}