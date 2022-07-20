using System.ComponentModel;

namespace CommonDataModels
{
    public enum FinanceCompany : byte
    {
        All = 0,
        [Description("JPP Consulting Ltd")] Consulting = 1,
        [Description("JPP Geotechnical & Environmental Ltd")] Geo = 2,
        [Description("JPP Surveying Ltd")] Surveying = 3,
        [Description("W E Architecture Ltd")] WeArchitects = 4,
        [Description("Smith Foster")] SmithFoster = 5,
        [Description("Flowerpot Holdings Limited")] Flowerpot = 6
    }
}
