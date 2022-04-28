using FluentAssertions;
using NetcoreSaas.Domain.Models.Core.Subscriptions;
using NetcoreSaas.Domain.Models.Core.Tenants;
using Xunit;

namespace NetcoreSaas.UnitTests.Domain.Models
{
    public class TenantTests
    {
        [Fact]
        public void HasMaxNumberOfUsers_WithMaxAtZero_ReturnsFalse()
        {
            // Arrange
            var tenant = new Tenant();
            tenant.Products.Add(new TenantProduct()
            {
                SubscriptionPrice = new SubscriptionPrice()
                {
                    SubscriptionProduct = new SubscriptionProduct()
                    {
                        MaxUsers = 0
                    }
                }
            });

            // Act
            var result = tenant.HasMaxNumberOfUsers();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasMaxNumberOfUsers_WithMaxUsersSetAndEqualsNumberOfUsers_ReturnsTrue()
        {
            // Arrange
            var tenant = new Tenant();
            tenant.Products.Add(new TenantProduct()
            {
                SubscriptionPrice = new SubscriptionPrice()
                {
                    SubscriptionProduct = new SubscriptionProduct()
                    {
                        MaxUsers = 1
                    }
                }
            });
            tenant.Users.Add(new TenantUser());
           
            // Act
            var result = tenant.HasMaxNumberOfUsers();

            // Assert
            result.Should().BeTrue();
        }
    }
}
