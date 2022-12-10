using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Jpp.Projects.Models
{
    public class PurchaseOrderModel : BaseModel
    {
        public DateTime? CreatedDate { get { return TryGetRowValue("Created_Date", out var rowValue) ? (DateTime)rowValue : null; } }
        public DateTime? ApprovedDate { get { return TryGetRowValue("Approved_Date", out var rowValue) ? (DateTime)rowValue : null; } }

        public string? ProjectCode { get { return TryGetRowValue("Project_Code", out var rowValue) ? (string)rowValue : null; } }
        public string? ProjectName { get { return TryGetRowValue("Name", out var rowValue) ? (string)rowValue : null; } }

        public string? Notes { get { return TryGetRowValue("Notes", out var rowValue) ? (string)rowValue : null; } }
        public string? InternalNotes { get { return TryGetRowValue("Internal_Notes", out var rowValue) ? (string)rowValue : null; } }

        public string? ShortNotes { get { return TryGetRowValue("Short_Notes", out var rowValue) ? (string)rowValue : null; } }
        public string? ShortInternalNotes { get { return TryGetRowValue("Short_Internal_Notes", out var rowValue) ? (string)rowValue : null; } }

        //TODO: Convert to enum? Need to work out applicable values
        public string? Status { get { return TryGetRowValue("StatusTXT", out var rowValue) ? (string)rowValue : null; } }

        public IList<PurchaseOrderLinesModel> Lines { get; set; }

        public PurchaseOrderModel(DataRow row) : base(row)
        {    }
    }

    public class PurchaseOrderLinesModel : BaseModel
    {
        public string? ProjectCode { get { return TryGetRowValue("Project_Code", out var rowValue) ? (string)rowValue : null; } }

        public decimal? Rate { get { return TryGetRowValue("Rate", out var rowValue) ? (decimal)rowValue : null; } }
        public decimal? Quantity { get { return TryGetRowValue("quantity", out var rowValue) ? (decimal)rowValue : null; } }
        public decimal? Value { get { return TryGetRowValue("Value", out var rowValue) ? (decimal)rowValue : null; } }

        public decimal? OutstandingValue { get { return TryGetRowValue("Outstanding_Value", out var rowValue) ? (decimal)rowValue : null; } }

        public string? Notes { get { return TryGetRowValue("Notes", out var rowValue) ? (string)rowValue : null; } }
        public string? ShortNotes { get { return TryGetRowValue("Short_Notes", out var rowValue) ? (string)rowValue : null; } }
        public string? ShortOrderNotes { get { return TryGetRowValue("Short_Order_Notes", out var rowValue) ? (string)rowValue : null; } }

        public int? LineId { get { return TryGetRowValue("Finance_Purchase_Order_Line_ID", out var rowValue) ? (int)rowValue : null; } }

        public IList<PurchaseOrderLineInvoiceModel> Invoices { get; set; }

        public PurchaseOrderLinesModel(DataRow row) : base(row)
        { }
    }

    public class PurchaseOrderLineInvoiceModel : BaseModel
    {
        public string? ProjectCode { get { return TryGetRowValue("Project_Code", out var rowValue) ? (string)rowValue : null; } }
        public string? ProjectName { get { return TryGetRowValue("Name", out var rowValue) ? (string)rowValue : null; } }

        public int? LineId { get { return TryGetRowValue("Finance_Purchase_Order_Line_ID", out var rowValue) ? (int)rowValue : null; } }
        public DateTime? CreatedDate { get { return TryGetRowValue("Invoice_Created_Date", out var rowValue) ? (DateTime)rowValue : null; } }

        public decimal? Value { get { return TryGetRowValue("Value", out var rowValue) ? (decimal)rowValue : null; } }

        public PurchaseOrderLineInvoiceModel(DataRow row) : base(row)
        { }
    }
}
