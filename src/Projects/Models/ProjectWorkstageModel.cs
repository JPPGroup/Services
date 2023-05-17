using System.Data;

namespace Jpp.Projects.Models
{
    public class ProjectWorkstageModel : BaseModel
    {
        public string? Name { get { return TryGetRowValue("name", out var rowValue) ? (string)rowValue : null; } }
        public string? Abbreviation { get { return TryGetRowValue("abbreviation", out var rowValue) ? (string)rowValue : null; } }

        public decimal? TotalFee { get { return TryGetRowValue("reporting_total_fee", out var rowValue) ? (decimal)rowValue : null; } }

        public decimal? Costs { get { return TryGetRowValue("total_timecost_todate", out var rowValue) ? (decimal)rowValue : null; } }

        public decimal? Invoiced { get { return TryGetRowValue("total_invoices", out var rowValue) ? (decimal)rowValue : null; } }

        public ProjectWorkstageModel(DataRow row) : base(row)
        { }
    }
}
