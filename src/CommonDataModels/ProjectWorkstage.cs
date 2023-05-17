namespace CommonDataModels
{
    public class ProjectWorkstage
    {
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }

        public decimal? TotalFee { get; set; }
        public decimal? Costs { get; set; }
        public decimal? Invoiced { get; set; }
    }
}
