using System;
using System.Collections.Generic;
using System.Linq;
using NetcoreSaas.Application.Dtos.Core.Tenants;
using NetcoreSaas.Application.Dtos.Core.Workspaces;
using NetcoreSaas.Domain.Enums.Core.Users;

namespace NetcoreSaas.Application.Dtos.Core.Users
{
    public class UserDto : MasterEntityDto
    {
        public string Email { get; set; }
        public UserType Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public UserLoginType LoginType { get; set; }
        public string Avatar { get; set; }
        public Guid Token { get; set; }
        public string Locale { get; set; }
        public char Gender { get; set; } = 'M';
        public int? DefaultTenantId { get; set; }
        public TenantSimpleDto DefaultTenant { get; set; }
        public ICollection<TenantUserDto> Tenants { get; set; }
        public TenantSimpleDto CurrentTenant { get; set; }
        public List<WorkspaceUserDto> Workspaces { get; set; }

        public TenantUserDto MembershipFromTenantId(Guid tenantId)
        {
            return Tenants.SingleOrDefault(f => f.TenantId == tenantId);
        }

        public TenantUserDto MembershipFromCurrentTenant()
        {
            return Tenants.SingleOrDefault(f => f.TenantId == CurrentTenant.Id);
        }
    }
}
