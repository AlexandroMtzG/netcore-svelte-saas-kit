using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NetcoreSaas.Domain.Enums.Core.Tenants;
using NetcoreSaas.Domain.Enums.Core.Users;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Workspaces;

namespace NetcoreSaas.Domain.Models.Core.Users
{
    public class User : MasterEntity
    {
        public string Email { get; set; }
        public UserType Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public UserLoginType LoginType { get; set; }
        public string Avatar { get; set; }
        public Guid Token { get; set; } = Guid.NewGuid();
        public Guid? DefaultTenantId { get; set; }
        public Tenant DefaultTenant { get; set; }
        public string Locale { get; set; }
        public char Gender { get; set; } = 'M';
        public ICollection<TenantUser> Tenants { get; set; }
        public ICollection<WorkspaceUser> Workspaces { get; set; }

        public User()
        {
            Tenants = new Collection<TenantUser>();
            Workspaces = new List<WorkspaceUser>();
        }

        public bool IsVerified()
        {
            return Token == Guid.Empty && string.IsNullOrEmpty(Password) == false;
        }

        public bool CanUpdateUserInTenant(TenantUser tenantUser, Guid tenantId)
        {
            if (Id == tenantUser.User.Id) return true;
            var currentUserInCurrentTenant = MembershipFromTenant(tenantId);
            return currentUserInCurrentTenant != null && new[] { TenantUserRole.Owner, TenantUserRole.Admin }.Contains(currentUserInCurrentTenant.Role);
        }

        public TenantUser MembershipFromTenant(Guid tenantId)
        {
            return Tenants.ToList().Find(f => f.TenantId == tenantId);
        }

        public string EmailTo()
        {
            if (string.IsNullOrEmpty(FirstName) == false && string.IsNullOrEmpty(LastName) == false)
                return $"{FirstName} {LastName} <{Email}>";
            if (string.IsNullOrEmpty(FirstName) == false)
                return $"{FirstName} <{Email}>";
            return $"{Email}";
        }
    }
}
