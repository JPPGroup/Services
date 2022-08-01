using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDataModels
{
    public class Invoice
    {        
        public DateTime? InvoiceDate { get; set; }
        public string? Client { get; set; }
        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ExportedDate { get; set; }

        public decimal? NettValue { get; set; }
        public decimal? GrossValue { get; set; }
        public decimal? TotalUnpaid { get; set; }

        public string? Notes { get; set; }
        public string? InternalNotes { get; set; }
        public string? FinanceNotes { get; set; }
    }
}
