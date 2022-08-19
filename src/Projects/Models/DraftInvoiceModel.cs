using System;
using System.Data;
using System.Xml;

namespace Jpp.Projects.Models
{
    public class DraftInvoiceModel
    {
        private readonly DataRow row;

        public DateTime? InvoiceDate { get { return TryGetRowValue("Created_Date", out var rowValue) ? (DateTime)rowValue : null; } }

        public string? Client = "Default Client";

        public decimal? NettValue = 0;
        public decimal? GrossValue = 0;

        public string? ProjectCode { get { return TryGetRowValue("Project_Code", out var rowValue) ? (string)rowValue : null; } }
        public string? ProjectName { get { return TryGetRowValue("Name", out var rowValue) ? (string)rowValue : null; } }

        private string? invoiceXML { get { return TryGetRowValue("Invoice_XML", out var rowValue) ? (string)rowValue : null; } }

        public string? Forename { get { return TryGetRowValue("Forename", out var rowValue) ? (string)rowValue : null; } }
        public string? Surname { get { return TryGetRowValue("Surname", out var rowValue) ? (string)rowValue : null; } }

        public DraftInvoiceModel(DataRow row)
        {
            this.row = row ?? throw new ArgumentNullException(nameof(row));
            ParseXML();
        }

        private bool TryGetRowValue(string field, out object rowValue)
        {
            rowValue = this.row[field];
            return !DBNull.Value.Equals(rowValue);
        }

        private void ParseXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(invoiceXML);
            XmlNode root = doc.DocumentElement;
            var nodes = root.SelectNodes("//taxCode");

            foreach (XmlNode n in nodes)
            {
                string? nett = n?.Attributes["nett"]?.Value;
                string? tax = n?.Attributes["tax"]?.Value;
                if (!string.IsNullOrEmpty(nett))
                {
                    NettValue += decimal.Parse(nett);
                    GrossValue += decimal.Parse(nett) + decimal.Parse(tax);
                }
            }

            Client = root.SelectSingleNode("//billingClient").InnerText;
        }
    }
}

