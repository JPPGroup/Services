using System;
using System.Data;

namespace Jpp.Projects.Models
{
    public class BaseModel
    {
        private readonly DataRow row;

        public BaseModel(DataRow row)
        {
            this.row = row ?? throw new ArgumentNullException(nameof(row));
        }

        protected bool TryGetRowValue(string field, out object rowValue)
        {
            rowValue = this.row[field];
            return !DBNull.Value.Equals(rowValue);
        }
    }
}
