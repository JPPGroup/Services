using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDataModels
{
    public class PurchaseOrder
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }

        public string? Notes { get; set; }
        public string? InternalNotes { get; set; }

        public string? ShortNotes { get; set; }
        public string? ShortInternalNotes { get; set; }

        public IList<PurchaseOrderLines> Lines { get; set; }

        //TODO: Convert to enum? Need to work out applicable values
        public string? Status { get; set; }

    }

    public class PurchaseOrderLines
    {
        public decimal? Rate { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Value { get; set; }

        public decimal? OutstandingValue { get; set; }

        public string? Notes { get; set; }
        public string? ShortNotes { get; set; }
        public string? ShortOrderNotes { get; set; }

        public IList<PurchaseOrderLineInvoice> Invoices { get; set; }
    }

    public class PurchaseOrderLineInvoice
    {
        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }

        public DateTime? CreatedDate { get; set; }
        public decimal? Value { get; set; }
    }
}
