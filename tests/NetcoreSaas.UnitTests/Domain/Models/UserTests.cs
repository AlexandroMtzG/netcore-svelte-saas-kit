using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NetcoreSaas.Domain.Models.Core.Tenants;
using NetcoreSaas.Domain.Models.Core.Users;
using Xunit;

namespace NetcoreSaas.UnitTests.Domain.Models
{
    public class UserTests
    {
        [Fact]
        public void CanUpdateUserInTenant_UserInTenant_ReturnsTrue()
        {
            // Arrange
            var user1 = new User();
            var tenant1 = new Tenant();
            var tenantUser1 = new TenantUser() { User = user1, Tenant = tenant1 };
            tenant1.Users = new List<TenantUser>()
            {
                tenantUser1
            };

            // Act
            var result = user1.CanUpdateUserInTenant(tenantUser1, tenant1.Id);

            // Assert
            result.Should().BeTrue();
        }
        
        [Fact]
        public void CanUpdateUserInTenant_UserNotInTenant_ReturnsFalse()
        {
            // Arrange
            var tenant1 = new Tenant();
            var tenant2 = new Tenant();
            
            tenant1.Users = new List<TenantUser>()
            {
                new TenantUser(){User = new User(), Tenant = tenant1}
            };
            tenant2.Users = new List<TenantUser>()
            {
                new TenantUser(){User = new User(), Tenant = tenant2}
            };
            
            // Act
            var tenantUser1 = tenant1.Users.First();
            var tenantUser2 = tenant2.Users.First();
            var result = tenantUser1.User.CanUpdateUserInTenant(tenantUser2, tenantUser2.Tenant.Id);

            // Assert
            result.Should().BeFalse();
        }
    }
}