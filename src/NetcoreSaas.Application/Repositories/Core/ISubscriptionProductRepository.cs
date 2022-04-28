using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetcoreSaas.Domain.Models.Core.Subscriptions;

namespace NetcoreSaas.Application.Repositories.Core
{
    public interface ISubscriptionProductRepository : IRepository<SubscriptionProduct>
    {
        Task<SubscriptionProduct> GetProduct(Guid id);
        Task<SubscriptionPrice> GetPrice(Guid id);
        Task<SubscriptionFeature> GetFeature(Guid id);
        void AddProduct(SubscriptionProduct product);
        void AddPrice(SubscriptionProduct product, SubscriptionPrice price);
        void AddFeature(SubscriptionProduct product, SubscriptionFeature feature);
        void RemoveProduct(SubscriptionProduct product);
        void RemovePrice(SubscriptionProduct product, SubscriptionPrice price);
        void RemoveFeature(SubscriptionProduct product, SubscriptionFeature feature);

        Task<IEnumerable<SubscriptionProduct>> GetProductsWithPricesAndFeatures();
        Task<IEnumerable<SubscriptionPrice>> GetPricesWithProducts();
        Task<SubscriptionProduct> GetProductWithPricesAndFeatures(Guid id);
        Task<SubscriptionPrice> GetPriceWithProducts(Guid id);
    }
}
