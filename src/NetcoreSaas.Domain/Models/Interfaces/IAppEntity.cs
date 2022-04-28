using System;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;

namespace NetcoreSaas.Domain.Models.Interfaces
{
    public interface IAppEntity : IEntity
    {
        Guid? CreatedByUserId { get; set; }
        User CreatedByUser { get; set; }
        Guid? ModifiedByUserId { get; set; }
        User ModifiedByUser { get; set; }
        Guid TenantId { get; set; }
        Tenant Tenant { get; set; }
    }
}