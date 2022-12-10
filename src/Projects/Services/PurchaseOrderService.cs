using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Jpp.Projects.Models;
using Microsoft.Extensions.Logging;
using Projects.Models;
using System.Text;
using System.Linq;
using System.Globalization;

namespace Jpp.Projects.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ILogger<PurchaseOrderService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public PurchaseOrderService(ILogger<PurchaseOrderService> logger, IConfiguration configuration, IMemoryCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }        

        public async Task<IList<PurchaseOrderModel>> ListByProjectsAsync(IEnumerable<string> ProjectIds, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                return await GetPoCollection(ProjectIds, fromDate, toDate);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unable to get purchase orders.");
                return new List<PurchaseOrderModel>();
            }
        }

        public async Task<IList<PurchaseOrderLineInvoiceModel>> ListInvoicesByProjectsAsync(IEnumerable<string> ProjectIds, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                return await GetInvoiceCollection(ProjectIds, fromDate, toDate);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unable to get purchase order invoices.");
                return new List<PurchaseOrderLineInvoiceModel>();
            }
        }

        private async Task<List<PurchaseOrderModel>> GetPoCollection(IEnumerable<string> ProjectIds, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return await Task.Run(async () =>
            {
                string connectionString = _configuration.GetConnectionString("PIM");

                using SqlConnection connection = new SqlConnection(connectionString);
                var command = new SqlCommand();
                var linecommand = new SqlCommand();

                StringBuilder linesb = new StringBuilder("SELECT * FROM [DeltekPIM].[dbo].[EXVW_ML_Purchase_Order_Lines] WHERE [Project_Code] in (");

                StringBuilder sb = new StringBuilder("SELECT * FROM [DeltekPIM].[dbo].[EXVW_ML_Purchase_Orders] WHERE [Project_Code] in (");

                sb.Append($"'{ProjectIds.ElementAt(0)}'");
                linesb.Append($"'{ProjectIds.ElementAt(0)}'");

                for (int i = 1; i < ProjectIds.Count(); i++)
                {
                    sb.Append($", '{ProjectIds.ElementAt(i)}'");
                    linesb.Append($", '{ProjectIds.ElementAt(i)}'");

                }

                sb.Append(")");
                linesb.Append(")");

                CultureInfo cInfo = new CultureInfo("en-us");

                if (fromDate != null)
                {
                    sb.Append($" AND [Created_Date] >= Convert(datetime, '{fromDate.Value.ToString("d", cInfo)}' )");
                }

                if (toDate != null)
                {
                    sb.Append($" AND [Created_Date] <= Convert(datetime, '{toDate.Value.ToString("d", cInfo)}' )");
                }

                command.CommandText = sb.ToString();
                command.Connection = connection;

                using var dataSet = new DataSet("PurchaseOrders");
                using var adapter = new SqlDataAdapter { SelectCommand = command };
                adapter.Fill(dataSet);

                linecommand.CommandText = linesb.ToString();
                linecommand.Connection = connection;

                using var linedataSet = new DataSet("PurchaseOrderLines");
                using var lineadapter = new SqlDataAdapter { SelectCommand = linecommand };
                lineadapter.Fill(linedataSet);

                var invoices = await GetInvoiceCollection(ProjectIds, null, null);

                return BuildList(dataSet, linedataSet, invoices);
            });
        }

        private async Task<List<PurchaseOrderLineInvoiceModel>> GetInvoiceCollection(IEnumerable<string> ProjectIds, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return await Task.Run(async () =>
            {
                string connectionString = _configuration.GetConnectionString("PIM");

                using SqlConnection connection = new SqlConnection(connectionString);                
                var linecommand = new SqlCommand();

                StringBuilder linesb = new StringBuilder("SELECT * FROM [DeltekPIM].[dbo].[EXVW_ML_Purchase_Order_Line_Invoices] WHERE [Project_Code] in (");                
                                
                linesb.Append($"'{ProjectIds.ElementAt(0)}'");

                for (int i = 1; i < ProjectIds.Count(); i++)
                {                    
                    linesb.Append($", '{ProjectIds.ElementAt(i)}'");

                }
                
                linesb.Append(")");

                CultureInfo cInfo = new CultureInfo("en-us");

                if (fromDate != null)
                {
                    linesb.Append($" AND [Invoice_Created_Date] >= Convert(datetime, '{fromDate.Value.ToString("d", cInfo)}' )");
                }

                if (toDate != null)
                {
                    linesb.Append($" AND [Invoice_Created_Date] <= Convert(datetime, '{toDate.Value.ToString("d", cInfo)}' )");
                }                

                linecommand.CommandText = linesb.ToString();
                linecommand.Connection = connection;

                using var linedataSet = new DataSet("PurchaseOrderLines");
                using var lineadapter = new SqlDataAdapter { SelectCommand = linecommand };
                lineadapter.Fill(linedataSet);                               

                return BuildInvoiceList(linedataSet);
            });
        }

        private static List<PurchaseOrderModel> BuildList(DataSet dataSet, DataSet linedataSet, IEnumerable<PurchaseOrderLineInvoiceModel> invoices)
        {
            var list = new List<PurchaseOrderModel>();
            var linelist = new List<PurchaseOrderLinesModel>();

            foreach (DataRow? row in linedataSet.Tables[0].Rows)
            {
                if (row is null)
                {
                    continue;
                }

                var polm = new PurchaseOrderLinesModel(row);
                polm.Invoices = invoices.Where(i => i.LineId == polm.LineId).ToList();
                linelist.Add(polm);
            }

            foreach (DataRow? row in dataSet.Tables[0].Rows)
            {
                if (row is null)
                {
                    continue;
                }

                var po = new PurchaseOrderModel(row);
                po.Lines = linelist.Where(poline => poline.ProjectCode == po.ProjectCode).ToList();

                list.Add(po);
            }

            return list;
        }

        private static List<PurchaseOrderLineInvoiceModel> BuildInvoiceList(DataSet linedataSet)
        {            
            var linelist = new List<PurchaseOrderLineInvoiceModel>();

            foreach (DataRow? row in linedataSet.Tables[0].Rows)
            {
                if (row is null)
                {
                    continue;
                }

                linelist.Add(new PurchaseOrderLineInvoiceModel(row));
            }

            return linelist;
        }
    }
}
