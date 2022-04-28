using System.Collections.Generic;
using NetcoreSaas.Application.Dtos.Core.Subscriptions;
using NetcoreSaas.Application.Dtos.Core.Tenants;

namespace NetcoreSaas.Application.Contracts.Core.Subscriptions
{
    public class SubscriptionGetCurrentResponse
    {
        public TenantProductDto ActiveProduct { get; set; }
        public IEnumerable<TenantProductDto> MyProducts { get; set; }
        public SubscriptionCustomerDto Customer { get; set; }
        public IEnumerable<SubscriptionInvoiceDto> Invoices { get; set; }
        public IEnumerable<SubscriptionCardDto> Cards { get; set; }
        public IEnumerable<SubscriptionPaymentMethodDto> PaymentMethods { get; set; }
        public SubscriptionGetCurrentResponse(TenantProductDto activeProduct, IEnumerable<TenantProductDto> myProducts, SubscriptionCustomerDto customer, IEnumerable<SubscriptionInvoiceDto> invoices, IEnumerable<SubscriptionCardDto> cards, IEnumerable<SubscriptionPaymentMethodDto> paymentMethods)
        {
            ActiveProduct = activeProduct;
            MyProducts = myProducts;
            Customer = customer;
            Invoices = invoices;
            Cards = cards;
            PaymentMethods = paymentMethods;
        }
    }
}
