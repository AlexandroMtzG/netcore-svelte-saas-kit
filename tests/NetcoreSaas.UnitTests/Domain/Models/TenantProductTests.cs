using System;
using FluentAssertions;
using NetcoreSaas.Domain.Enums.Core.Subscriptions;
using NetcoreSaas.Domain.Models.Core.Subscriptions;
using NetcoreSaas.Domain.Models.Core.Tenants;
using Xunit;

namespace NetcoreSaas.UnitTests.Domain.Models
{
    public class TenantProductTests
    {
        [Fact]
        public void EndOfTrialDate_OneTimePaymentProduct_ReturnsNull()
        {
            // Arrange
            var tenantProduct = new TenantProduct();
            tenantProduct.CreatedAt = new DateTime(2020, 1, 1);
            tenantProduct.SubscriptionPrice = new SubscriptionPrice()
            {
                BillingPeriod = SubscriptionBillingPeriod.Once,
                TrialDays = 5
            };

            // Act
            var result = tenantProduct.EndOfTrialDate();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void EndOfTrialDate_14TrialDays_ReturnsDate()
        {
            // Arrange
            var tenantProduct = new TenantProduct();
            tenantProduct.CreatedAt = new DateTime(2020, 1, 1);
            tenantProduct.SubscriptionPrice = new SubscriptionPrice()
            {
                BillingPeriod = SubscriptionBillingPeriod.Monthly,
                TrialDays = 14
            };

            // Act
            var result = tenantProduct.EndOfTrialDate();

            // Assert
            result.Should().Be(new DateTime(2020, 1, 15));
        }

        [Fact]
        public void EndOfTrialDate_NoTrialDays_ReturnsNull()
        {
            // Arrange
            var tenantProduct = new TenantProduct();
            tenantProduct.CreatedAt = new DateTime(2020, 1, 1);
            tenantProduct.SubscriptionPrice = new SubscriptionPrice()
            {
                BillingPeriod = SubscriptionBillingPeriod.Monthly,
                TrialDays = 0
            };

            // Act
            var result = tenantProduct.EndOfTrialDate();

            // Assert
            result.Should().BeNull();
        }
    }
}
