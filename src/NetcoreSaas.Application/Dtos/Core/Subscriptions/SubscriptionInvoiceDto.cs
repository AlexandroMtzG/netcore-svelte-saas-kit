using System;
using System.Collections.Generic;

namespace NetcoreSaas.Application.Dtos.Core.Subscriptions
{
    public class SubscriptionInvoiceDto
    {
        public DateTime Created { get; set; }
        public string InvoicePdf { get; set; }
        public List<SubscriptionInvoiceLineDto> Lines { get; set; }
        public SubscriptionInvoiceDto(DateTime created, string invoicePdf, List<SubscriptionInvoiceLineDto> lines)
        {
            Created = created;
            InvoicePdf = invoicePdf;
            Lines = lines;
        }
    }
}
