using System;
using NetcoreSaas.Application.Dtos.Core.Tenants;

namespace NetcoreSaas.Application.Dtos.Core
{
    public class AppEntityDto : EntityDto
    {
        public Guid TenantId { get; set; }
        public TenantSimpleDto Tenant;
    }
}