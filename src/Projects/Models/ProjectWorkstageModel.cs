using System.Data;

namespace Jpp.Projects.Models
{
    public class ProjectWorkstageModel : BaseModel
    {
        public string? Name { get { return TryGetRowValue("name", out var rowValue) ? (string)rowValue : null; } }
        public string? Abbreviation { get { return TryGetRowValue("abbreviation", out var rowValue) ? (string)rowValue : null; } }

        public ProjectWorkstageModel(DataRow row) : base(row)
        { }
    }
}
