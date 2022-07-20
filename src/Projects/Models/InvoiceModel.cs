using System;
using System.Data;

namespace Projects.Models
{
    public class InvoiceModel
    {
        private readonly DataRow row;

        public DateTime? InvoiceDate { get { return TryGetRowValue("Document_Date", out var rowValue) ? (DateTime)rowValue : null; } }
        public DateTime? DueDate { get { return TryGetRowValue("Due_Date", out var rowValue) ? (DateTime)rowValue : null; } }
        public DateTime? ExportedDate { get { return TryGetRowValue("Exported_Date", out var rowValue) ? (DateTime)rowValue : null; } }

        public decimal? NettValue { get { return TryGetRowValue("Nett_Value", out var rowValue) ? (decimal) rowValue : null; } }
        public decimal? GrossValue { get { return TryGetRowValue("Gross_Value", out var rowValue) ? (decimal)rowValue : null; } }
        public decimal? TotalUnpaid { get { return TryGetRowValue("Total_Unpaid", out var rowValue) ? (decimal)rowValue : null; } }

        public string? Notes { get { return TryGetRowValue("Notes", out var rowValue) ? (string)rowValue : null; } }
        public string? InternalNotes { get { return TryGetRowValue("Internal_Notes", out var rowValue) ? (string)rowValue : null; } }
        public string? FinanceNotes { get { return TryGetRowValue("FDNotes", out var rowValue) ? (string)rowValue : null; } }

        public InvoiceModel(DataRow row)
        {
            this.row = row ?? throw new ArgumentNullException(nameof(row));
        }

        private bool TryGetRowValue(string field, out object rowValue)
        {
            rowValue = this.row[field];
            return !DBNull.Value.Equals(rowValue);
        }
    }
}
