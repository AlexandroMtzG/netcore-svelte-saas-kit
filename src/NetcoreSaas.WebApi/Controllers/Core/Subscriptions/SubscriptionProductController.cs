using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetcoreSaas.Application.Contracts.Core.Subscriptions;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;
using NetcoreSaas.Application.Services.Core.Subscriptions;
using NetcoreSaas.Application.UnitOfWork;
using NetcoreSaas.Domain.Helpers;
using NetcoreSaas.Domain.Models.Core.Subscriptions;

namespace NetcoreSaas.WebApi.Controllers.Core.Subscriptions
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SubscriptionProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMasterUnitOfWork _masterUnitOfWork;
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionProductController(IMapper mapper, IMasterUnitOfWork masterUnitOfWork, ISubscriptionService subscriptionService)
        {
            _mapper = mapper;
            _masterUnitOfWork = masterUnitOfWork;
            _subscriptionService = subscriptionService;
        }

        [HttpGet(ApiCoreRoutes.SubscriptionProduct.GetAll)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var products = (await _masterUnitOfWork.Subscriptions.GetProductsWithPricesAndFeatures()).ToList();
            if (products.Count == 0)
                return NoContent();
            foreach (var product in products)
            {
                product.Prices = product.Prices.OrderBy(f=>f.Type).ThenBy(f => f.BillingPeriod).ThenBy(f=>f.Currency).ThenBy(f=>f.Price).ToList();
                product.Features = product.Features.OrderBy(f => f.Order).ToList();
            }
            products = products.OrderBy(f => f.Tier).ToList();
            return Ok(_mapper.Map<IEnumerable<SubscriptionProductDto>>(products));
        }

        [HttpGet(ApiCoreRoutes.SubscriptionProduct.GetProduct)]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = await _masterUnitOfWork.Subscriptions.GetProductWithPricesAndFeatures(id);
            if (product == null)
                return NotFound();
            product.Prices = product.Prices.OrderBy(f=>f.Type).ThenBy(f => f.BillingPeriod).ThenBy(f => f.Currency).ThenBy(f => f.Price).ToList();
            product.Features = product.Features.OrderBy(f => f.Order).ToList();

            return Ok(_mapper.Map<SubscriptionProductDto>(product));
        }

        [HttpGet(ApiCoreRoutes.SubscriptionProduct.GetPrice)]
        [AllowAnonymous]
        public async Task<IActionResult> GetPrice(Guid id)
        {
            var price = await _masterUnitOfWork.Subscriptions.GetPrice(id);
            if (price == null)
                return NotFound();
            return Ok(_mapper.Map<SubscriptionPriceDto>(price));
        }

        [HttpGet(ApiCoreRoutes.SubscriptionProduct.GetFeature)]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeature(Guid id)
        {
            var feature = await _masterUnitOfWork.Subscriptions.GetFeature(id);
            if (feature == null)
                return NotFound();
            return Ok(_mapper.Map<SubscriptionFeatureDto>(feature));
        }

        [HttpPost(ApiCoreRoutes.SubscriptionProduct.CreateProduct)]
        public async Task<IActionResult> CreateProduct([FromBody] SubscriptionProductDto request)
        {
            var product = await _masterUnitOfWork.Subscriptions.GetProduct(request.Id);
            if(product != null)
                return BadRequest("api.errors.alreadyAdded");

            await _subscriptionService.SyncProduct(request);
            _masterUnitOfWork.Subscriptions.AddProduct(_mapper.Map<SubscriptionProduct>(request));
            await _masterUnitOfWork.CommitAsync();

            product = await _masterUnitOfWork.Subscriptions.GetProductWithPricesAndFeatures(request.Id);

            return Ok(_mapper.Map<SubscriptionProductDto>(product));
        }

        [HttpPost(ApiCoreRoutes.SubscriptionProduct.CreatePrice)]
        public async Task<IActionResult> CreatePrice([FromBody] SubscriptionPriceDto request)
        {
            var price = await _masterUnitOfWork.Subscriptions.GetPrice(request.Id);
            if (price != null)
                return BadRequest("api.errors.alreadyAdded");

            var product = await _masterUnitOfWork.Subscriptions.GetProduct(request.SubscriptionProductId);
            if (product == null)
                return NotFound();

            await _subscriptionService.SyncPrice(request, _mapper.Map<SubscriptionProductDto>(product));
            _masterUnitOfWork.Subscriptions.AddPrice(product, _mapper.Map<SubscriptionPrice>(request));
            await _masterUnitOfWork.CommitAsync();

            price = await _masterUnitOfWork.Subscriptions.GetPriceWithProducts(request.Id);
            return Ok(_mapper.Map<SubscriptionPriceDto>(price));
        }

        [HttpPost(ApiCoreRoutes.SubscriptionProduct.CreateFeature)]
        public async Task<IActionResult> CreateFeature([FromBody] SubscriptionFeatureDto request)
        {
            var feature = await _masterUnitOfWork.Subscriptions.GetFeature(request.Id);
            if (feature != null)
                return BadRequest("api.errors.alreadyAdded");

            var product = await _masterUnitOfWork.Subscriptions.GetProduct(request.SubscriptionProductId);
            if (product == null)
                return NotFound();

            _masterUnitOfWork.Subscriptions.AddFeature(product, _mapper.Map<SubscriptionFeature>(request));
            await _masterUnitOfWork.CommitAsync();

            feature = await _masterUnitOfWork.Subscriptions.GetFeature(request.Id);
            return Ok(_mapper.Map<SubscriptionFeatureDto>(feature));
        }

        [HttpPut(ApiCoreRoutes.SubscriptionProduct.UpdateProduct)]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] SubscriptionUpdateProductRequest request)
        {
            var product = await _masterUnitOfWork.Subscriptions.GetProduct(id);
            if (product == null)
                return NotFound();

            _mapper.Map(request, product);

            await _subscriptionService.SyncProduct(_mapper.Map<SubscriptionProductDto>(product));
            await _masterUnitOfWork.CommitAsync();

            product = await _masterUnitOfWork.Subscriptions.GetProductWithPricesAndFeatures(id);

            return Ok(_mapper.Map<SubscriptionProductDto>(product));
        }

        [HttpPut(ApiCoreRoutes.SubscriptionProduct.UpdatePrice)]
        public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] SubscriptionUpdatePriceRequest request)
        {
            var price = await _masterUnitOfWork.Subscriptions.GetPriceWithProducts(id);
            if (price == null)
                return NotFound();

            var product = _masterUnitOfWork.Subscriptions.GetProduct(price.SubscriptionProductId).Result;
            _mapper.Map(request, price);

            await _subscriptionService.SyncPrice(_mapper.Map<SubscriptionPriceDto>(price), _mapper.Map<SubscriptionProductDto>(product));
            await _masterUnitOfWork.CommitAsync();

            price = await _masterUnitOfWork.Subscriptions.GetPriceWithProducts(id);

            return Ok(_mapper.Map<SubscriptionPriceDto>(price));
        }

        [HttpPut(ApiCoreRoutes.SubscriptionProduct.UpdateFeature)]
        public async Task<IActionResult> UpdateFeature(Guid id, [FromBody] SubscriptionFeatureDto request)
        {
            var feature = await _masterUnitOfWork.Subscriptions.GetFeature(id);
            if (feature == null)
                return NotFound();

            var product = _masterUnitOfWork.Subscriptions.GetProduct(feature.SubscriptionProductId).Result;
            _mapper.Map(request, feature);

            feature.SubscriptionProduct = product;
            await _masterUnitOfWork.CommitAsync();

            feature = await _masterUnitOfWork.Subscriptions.GetFeature(id);

            return Ok(_mapper.Map<SubscriptionFeatureDto>(feature));
        }

        [HttpDelete(ApiCoreRoutes.SubscriptionProduct.DeleteProduct)]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _masterUnitOfWork.Subscriptions.GetProduct(id);
            if (product == null)
                return NotFound();

            _masterUnitOfWork.Subscriptions.RemoveProduct(product);
            foreach (var price in product.Prices)
                await _subscriptionService.DeletePrice(_mapper.Map<SubscriptionPriceDto>(price));
            
            await _subscriptionService.DeleteProduct(_mapper.Map<SubscriptionProductDto>(product));
            await _masterUnitOfWork.CommitAsync();

            return Ok();
        }

        [HttpDelete(ApiCoreRoutes.SubscriptionProduct.DeletePrice)]
        public async Task<IActionResult> DeletePrice(Guid id)
        {
            var price = await _masterUnitOfWork.Subscriptions.GetPrice(id);
            if (price == null)
                return NotFound();
            
            var product = await _masterUnitOfWork.Subscriptions.GetProduct(price.SubscriptionProductId);
            if (product == null)
                return NotFound();

            _masterUnitOfWork.Subscriptions.RemovePrice(product, price);
            await _subscriptionService.DeletePrice(_mapper.Map<SubscriptionPriceDto>(price));
            await _masterUnitOfWork.CommitAsync();

            return Ok();
        }

        [HttpDelete(ApiCoreRoutes.SubscriptionProduct.DeleteFeature)]
        public async Task<IActionResult> DeleteFeature(Guid id)
        {
            var feature = await _masterUnitOfWork.Subscriptions.GetFeature(id);
            if (feature == null)
                return NotFound();

            var product = await _masterUnitOfWork.Subscriptions.GetProduct(feature.SubscriptionProductId);
            if (product == null)
                return NotFound();

            _masterUnitOfWork.Subscriptions.RemoveFeature(product, feature);
            await _masterUnitOfWork.CommitAsync();

            return Ok();
        }
    }
}
