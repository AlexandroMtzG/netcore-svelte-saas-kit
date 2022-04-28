using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;
using NetcoreSaas.Domain.Enums.Core.Subscriptions;
using NetcoreSaas.Domain.Helpers;
using Xunit;

namespace NetcoreSaas.IntegrationTests.Core.Subscriptions.ProductController
{
    public class SubscriptionProductControllerTests : SubscriptionTestBase
    {
        [Fact]
        public async Task GetAll_WithExistingProductAndPrices_ReturnsAllProducts()
        {
            // Arrange
            SetupTestProductsWithPrices(); // Adds your TestProducts with their prices

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.SubscriptionProduct.GetAll);
            var products = await response.Content.ReadAsAsync<List<SubscriptionProductDto>>();

            // Assert
            products.Should().HaveCount(TestProducts.Count);
        }

        [Fact]
        public async Task GetProduct_NonExistingProduct_ReturnsNotFound()
        {
            // Arrange

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.SubscriptionProduct.GetProduct.Replace("{id}", Guid.Empty.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetProduct_WithExistingProduct_ReturnsProduct()
        {
            // Arrange
            SetupTestProductsWithPrices();
            var testProduct1 = TestProducts.First();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.SubscriptionProduct.GetProduct.Replace("{id}", testProduct1.Id.ToString()));
            var product = await response.Content.ReadAsAsync<SubscriptionProductDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            product.ServiceId.Should().NotBeEmpty();
            product.Prices.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task GetPrice_WithNonExistingPrice_ReturnsNotFound()
        {
            // Arrange

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.SubscriptionProduct.GetPrice.Replace("{id}", Guid.Empty.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetPrice_WithExistingPrice_ReturnsPrice()
        {
            // Arrange
            SetupTestProductsWithPrices();
            var testPrice1 = TestProducts.First().Prices.First();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.SubscriptionProduct.GetPrice.Replace("{id}", testPrice1.Id.ToString()));
            var product = await response.Content.ReadAsAsync<SubscriptionPriceDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            product.ServiceId.Should().NotBeEmpty();
            product.SubscriptionProduct.Should().NotBeNull();
        }

        [Fact]
        public async Task GetFeature_WithExistingFeature_ReturnsFeature()
        {
            // Arrange
            SetupTestProductsWithPrices();
            var testFeature1 = TestProducts.First().Features.First();

            // Act
            var response = await TestClient.GetAsync(ApiCoreRoutes.SubscriptionProduct.GetFeature.Replace("{id}", testFeature1.Id.ToString()));
            var feature = await response.Content.ReadAsAsync<SubscriptionFeatureDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            feature.Value.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CreateProduct_SubscriptionProductWith2Prices_ReturnsCreatedProduct()
        {
            // Arrange
            SetupTestProductsWithPrices();
            AuthenticateAsAdmin();

            var product = new SubscriptionProductDto()
            {
                Id = Guid.NewGuid(),
                Title = "Test product ABC",
                Active = true,
            };
            product.Prices.Add(new SubscriptionPriceDto()
            {
                Currency = DefaultCurrency,
                Price = 1,
                Type = SubscriptionPriceType.Recurring,
                BillingPeriod = SubscriptionBillingPeriod.Monthly,
                Active = true,
                TrialDays = 14,
            });
            product.Prices.Add(new SubscriptionPriceDto()
            {
                Currency = DefaultCurrency,
                Price = 10,
                Type = SubscriptionPriceType.Recurring,
                BillingPeriod = SubscriptionBillingPeriod.Yearly,
                Active = true,
                TrialDays = 14,
            });
            product.Features.Add(new SubscriptionFeatureDto()
            {
                Key = "MaxNumberOfUsers",
                Value = "3",
                Included = true
            });

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.CreateProduct, product);
            var responseContent = await response.Content.ReadAsAsync<SubscriptionProductDto>();

            // Assert
            responseContent.Id.Should().NotBe(Guid.Empty);
            responseContent.ServiceId.Should().NotBe(string.Empty);
            responseContent.Prices.Should().HaveCount(2);
        }

        [Fact]
        public async Task CreatePrice_ToProductWithNoPrices_ReturnsCreatedPrice()
        {
            // Arrange
            SetupTestProductsWithPrices();
            AuthenticateAsAdmin();

            var product = new SubscriptionProductDto()
            {
                Id = Guid.NewGuid(),
                Title = "Test product ABC",
                Active = true,
            };

            var createdProductResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.CreateProduct, product);
            var createdProduct = await createdProductResponse.Content.ReadAsAsync<SubscriptionProductDto>();

            var newPrice = new SubscriptionPriceDto()
            {
                SubscriptionProductId = createdProduct.Id,
                Currency = DefaultCurrency,
                Price = 1,
                Type = SubscriptionPriceType.Recurring,
                BillingPeriod = SubscriptionBillingPeriod.Monthly,
                Active = true,
                TrialDays = 14,
            };

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.CreatePrice, newPrice);
            var createdPrice = await response.Content.ReadAsAsync<SubscriptionPriceDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            createdPrice.ServiceId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CreateFeature_ToProductWith1Feature_ReturnsCreatedFeature()
        {
            // Arrange
            SetupTestProductsWithPrices();
            AuthenticateAsAdmin();

            var product = new SubscriptionProductDto()
            {
                Id = Guid.NewGuid(),
                Title = "Test product ABC",
                Active = true,
            };
            product.Features.Add(new SubscriptionFeatureDto()
            {
                Order = 1,
                Key = "Test feature 1",
                Value = "Test value 1"
            });
            product.Features.Add(new SubscriptionFeatureDto()
            {
                Order = 2,
                Key = "Test feature 2",
                Value = "Test value 2"
            });

            var createdProductResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.CreateProduct, product);
            var createdProduct = await createdProductResponse.Content.ReadAsAsync<SubscriptionProductDto>();

            var newFeature = new SubscriptionFeatureDto()
            {
                SubscriptionProductId = createdProduct.Id,
                Order = 3,
                Key = "Test feature 3",
                Value = "Test value 3"
            };

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.CreateFeature, newFeature);
            var createdFeature = await response.Content.ReadAsAsync<SubscriptionFeatureDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            createdFeature.Value.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task UpdateProduct_ChangeAllProperties_ReturnsUpdatedProduct()
        {
            // Arrange
            SetupTestProductsWithPrices();
            AuthenticateAsAdmin();

            var product = new SubscriptionProductDto()
            {
                Id = Guid.NewGuid(),
                Title = "Test product ABC",
                Active = true,
            };

            var createdProductResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.CreateProduct, product);
            var createdProduct = await createdProductResponse.Content.ReadAsAsync<SubscriptionProductDto>();

            // Act
            var updateProductRequest = new SubscriptionUpdateProductRequest();
            updateProductRequest.Id = createdProduct.Id;
            updateProductRequest.Tier = 10;
            updateProductRequest.Title = "New title";
            updateProductRequest.Description = "New description";
            updateProductRequest.Badge = "New badge";
            updateProductRequest.Active = false;
            updateProductRequest.Image = "";
            updateProductRequest.MaxUsers = 100;
            var response = await TestClient.PutAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.UpdateProduct
                .Replace("{id}", createdProduct.Id.ToString()), updateProductRequest);
            var updatedProduct = await response.Content.ReadAsAsync<SubscriptionProductDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            updatedProduct.Tier.Should().Be(10);
            updatedProduct.Title.Should().Be("New title");
            updatedProduct.Description.Should().Be("New description");
            updatedProduct.Badge.Should().Be("New badge");
            updatedProduct.Active.Should().Be(false);
            updatedProduct.MaxUsers.Should().Be(100);
        }

        [Fact]
        public async Task UpdatePrice_ToInactive_ReturnsUpdatedPrice()
        {
            // Arrange
            SetupTestProductsWithPrices();
            AuthenticateAsAdmin();

            var product = new SubscriptionProductDto()
            {
                Id = Guid.NewGuid(),
                Title = "Test product ABC",
                Active = true,
            };
            product.Prices.Add(new SubscriptionPriceDto()
            {
                Currency = DefaultCurrency,
                Price = 1,
                Type = SubscriptionPriceType.Recurring,
                BillingPeriod = SubscriptionBillingPeriod.Monthly,
                Active = true,
                TrialDays = 14,
            });

            var createdProductResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.CreateProduct, product);
            var createdProduct = await createdProductResponse.Content.ReadAsAsync<SubscriptionProductDto>();

            var priceToUpdate = createdProduct.Prices.First();

            // Act
            var updatePriceRequest = new SubscriptionUpdatePriceRequest();
            updatePriceRequest.Id = priceToUpdate.Id;
            updatePriceRequest.Active = false;
            var response = await TestClient.PutAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.UpdatePrice
                .Replace("{id}", priceToUpdate.Id.ToString()), updatePriceRequest);
            var updatedPrice = await response.Content.ReadAsAsync<SubscriptionPriceDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            updatedPrice.Active.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateFeature_ChangeOrderAndValue_ReturnsUpdatedFeature()
        {
            // Arrange
            SetupTestProductsWithPrices();
            AuthenticateAsAdmin();

            var product = new SubscriptionProductDto()
            {
                Id = Guid.NewGuid(),
                Title = "Test product ABC",
                Active = true,
            };
            product.Features.Add(new SubscriptionFeatureDto()
            {
                Order = 1,
                Key = "Test feature 1",
                Value = "Test value 1"
            });

            var createdProductResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.CreateProduct, product);
            var createdProduct = await createdProductResponse.Content.ReadAsAsync<SubscriptionProductDto>();

            var featureToUpdate = createdProduct.Features.First();

            // Act
            featureToUpdate.Order = 2;
            featureToUpdate.Value = "New feature 1 value";
            var response = await TestClient.PutAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.UpdateFeature
                .Replace("{id}", featureToUpdate.Id.ToString()), featureToUpdate);
            var updatedFeature = await response.Content.ReadAsAsync<SubscriptionFeatureDto>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            updatedFeature.Order.Should().Be(2);
            updatedFeature.Value.Should().Be("New feature 1 value");
        }

        [Fact]
        public async Task DeleteProduct_ExistingProduct_ReturnsOk()
        {
            // Arrange
            SetupTestProductsWithPrices();
            AuthenticateAsAdmin();
            var product = new SubscriptionProductDto()
            {
                Id = Guid.NewGuid(),
                Title = "Test product ABC",
                Active = true,
            };
            product.Prices.Add(new SubscriptionPriceDto()
            {
                Currency = DefaultCurrency,
                Price = 1,
                Type = SubscriptionPriceType.Recurring,
                BillingPeriod = SubscriptionBillingPeriod.Monthly,
                Active = true,
                TrialDays = 14,
            });
            var createProductResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.CreateProduct, product);
            var createdProduct = await createProductResponse.Content.ReadAsAsync<SubscriptionProductDto>();

            // Act
            var response = await TestClient.DeleteAsync(ApiCoreRoutes.SubscriptionProduct.DeleteProduct
                .Replace("{id}", createdProduct.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeletePrice_ExistingPrice_ReturnsOk()
        {
            // Arrange
            SetupTestProductsWithPrices();
            AuthenticateAsAdmin();

            var product = new SubscriptionProductDto()
            {
                Id = Guid.NewGuid(),
                Title = "Test product ABC",
                Active = true,
            };
            product.Prices.Add(new SubscriptionPriceDto()
            {
                Currency = DefaultCurrency,
                Price = 1,
                Type = SubscriptionPriceType.Recurring,
                BillingPeriod = SubscriptionBillingPeriod.Monthly,
                Active = true,
                TrialDays = 14,
            });

            var createdProductResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.CreateProduct, product);
            var createdProduct = await createdProductResponse.Content.ReadAsAsync<SubscriptionProductDto>();

            var priceToDelete = createdProduct.Prices.First();

            // Act
            var response = await TestClient.DeleteAsync(ApiCoreRoutes.SubscriptionProduct.DeletePrice
                .Replace("{id}", priceToDelete.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteFeature_ExistingFeature_ReturnsOk()
        {
            // Arrange
            SetupTestProductsWithPrices();
            AuthenticateAsAdmin();

            var product = new SubscriptionProductDto()
            {
                Id = Guid.NewGuid(),
                Title = "Test product ABC",
                Active = true,
            };
            product.Features.Add(new SubscriptionFeatureDto()
            {
                Order = 1,
                Key = "Test feature 1",
                Value = "Test value 1"
            });

            var createdProductResponse = await TestClient.PostAsJsonAsync(ApiCoreRoutes.SubscriptionProduct.CreateProduct, product);
            var createdProduct = await createdProductResponse.Content.ReadAsAsync<SubscriptionProductDto>();

            var featureToDelete = createdProduct.Features.First();

            // Act
            var response = await TestClient.DeleteAsync(ApiCoreRoutes.SubscriptionProduct.DeleteFeature
                .Replace("{id}", featureToDelete.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
