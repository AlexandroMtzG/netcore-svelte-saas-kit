using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetcoreSaas.Application.Repositories.Core;
using NetcoreSaas.Domain.Models.Core.Subscriptions;
using NetcoreSaas.Infrastructure.Data;

namespace NetcoreSaas.Infrastructure.Repositories.Core
{
    public class SubscriptionProductRepository : MasterRepository<SubscriptionProduct>, ISubscriptionProductRepository
    {
        public SubscriptionProductRepository(MasterDbContext context) : base(context)
        {
        }

        public void AddProduct(SubscriptionProduct product)
        {
            Context.SubscriptionProducts.Add(product);
        }

        public void AddPrice(SubscriptionProduct product, SubscriptionPrice price)
        {
            price.SubscriptionProduct = product;
            Context.SubscriptionPrices.Add(price);
        }

        public void AddFeature(SubscriptionProduct product, SubscriptionFeature feature)
        {
            feature.SubscriptionProduct = product;
            Context.SubscriptionFeatures.Add(feature);
        }

        public async Task<SubscriptionProduct> GetProduct(Guid id)
        {
            return await Context.SubscriptionProducts
                .Include(f=>f.Prices)
                .Include(f=>f.Features)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<SubscriptionPrice> GetPrice(Guid id)
        {
            return await Context.SubscriptionPrices.Include(f=>f.SubscriptionProduct).FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<SubscriptionFeature> GetFeature(Guid id)
        {
            return await Context.SubscriptionFeatures.Include(f => f.SubscriptionProduct).FirstOrDefaultAsync(f => f.Id == id);
        }

        public void RemoveProduct(SubscriptionProduct product)
        {
            Context.SubscriptionProducts.Remove(product);
        }

        public void RemovePrice(SubscriptionProduct product, SubscriptionPrice price)
        {
            product.Prices.Remove(price);
            Context.SubscriptionPrices.Remove(price);
        }

        public void RemoveFeature(SubscriptionProduct product, SubscriptionFeature feature)
        {
            product.Features.Remove(feature);
            Context.SubscriptionFeatures.Remove(feature);
        }

        public async Task<IEnumerable<SubscriptionPrice>> GetPricesWithProducts()
        {
            return await Context.SubscriptionPrices.Include(f => f.SubscriptionProduct).ToListAsync();
            //return mapper.Map<IEnumerable<SubscriptionPrice>, IEnumerable<SubscriptionPriceDto>>(prices);
        }

        public async Task<IEnumerable<SubscriptionProduct>> GetProductsWithPricesAndFeatures()
        {
            return await Context.SubscriptionProducts
                .Include(f => f.Prices)
                .Include(f=>f.Features)
                .ToListAsync();
            //return mapper.Map<IEnumerable<SubscriptionProduct>, IEnumerable<SubscriptionProductDto>>(products);
        }

        public async Task<SubscriptionProduct> GetProductWithPricesAndFeatures(Guid id)
        {
            return await Context.SubscriptionProducts
                .Include(f => f.Prices)
                .Include(f => f.Features)
                .FirstOrDefaultAsync(f=>f.Id == id);
            //return mapper.Map<SubscriptionProduct, SubscriptionProductDto>(product);
        }

        public async Task<SubscriptionPrice> GetPriceWithProducts(Guid id)
        {
            return await Context.SubscriptionPrices.Include(f => f.SubscriptionProduct).FirstOrDefaultAsync(f => f.Id == id);
            //return mapper.Map<SubscriptionPrice, SubscriptionPriceDto>(price);
        }
    }
}
