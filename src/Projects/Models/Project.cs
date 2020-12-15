using System;
using System.Data;

namespace Jpp.Projects.Models
{
    public class Project
    {
        private readonly DataRow row;

        public Project(DataRow row)
        {
            this.row = row ?? throw new ArgumentNullException(nameof(row));
        }

        public string Code => this.GetCode();
        public string Name => this.GetName();
        public int Category => this.GetCategory();

        private bool TryGetRowValue(string field, out object rowValue)
        {
            rowValue = this.row[field];
            return !DBNull.Value.Equals(rowValue);
        }

        private string GetCode()
        {
            return this.TryGetRowValue("Project_Code", out var rowValue) ? (string)rowValue : string.Empty;
        }

        private string GetName()
        {
            return this.TryGetRowValue("Name", out var rowValue) ? (string)rowValue : string.Empty;
        }

        private int GetCategory()
        {
            return this.TryGetRowValue("Project_Category_ID", out var rowValue) ? (int)rowValue : default;
        }
    }
}
