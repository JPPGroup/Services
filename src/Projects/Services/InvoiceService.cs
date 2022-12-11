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
    public class InvoiceService : IInvoiceService
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public InvoiceService(ILogger<ProjectService> logger, IConfiguration configuration, IMemoryCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<IList<InvoiceModel>> ListByProjectAsync(string ProjectId, DateTime? fromDate, DateTime? toDate)
        {
            //TODO: Implement dates
            try
            {
                return await _cache.GetOrCreateAsync($"Invoices{ProjectId}", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return GetInvoices(ProjectId);
                });

            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unable to get invoices.");
                return new List<InvoiceModel>();
            }
        }

        public async Task<IList<InvoiceModel>> ListByProjectsAsync(IEnumerable<string> ProjectIds, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                return await GetInvoiceCollection(ProjectIds, fromDate, toDate);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unable to get invoices.");
                return new List<InvoiceModel>();
            }
        }

        public async Task<IList<DraftInvoiceModel>> ListDraftsByProjectsAsync(IEnumerable<string> ProjectIds, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                return await GetDraftInvoiceCollection(ProjectIds, fromDate, toDate);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unable to get invoices.");
                return new List<DraftInvoiceModel>();
            }
        }

        private async Task<List<InvoiceModel>> GetInvoices(string ProjectId)
        {
            return await Task.Run(() =>
            {
                string connectionString = _configuration.GetConnectionString("PIM");

                using SqlConnection connection = new SqlConnection(connectionString);
                var command = new SqlCommand();
                command.CommandText = $"SELECT * FROM [DeltekPIM].[dbo].[EXVW_Project_Invoice_Exported_No_Contacts] WHERE [Project_Code]='{ProjectId}'";
                command.Connection = connection;

                using var dataSet = new DataSet("Invoices");
                using var adapter = new SqlDataAdapter { SelectCommand = command };
                adapter.Fill(dataSet);

                return BuildList(dataSet);
            });
        }

        private async Task<List<InvoiceModel>> GetInvoiceCollection(IEnumerable<string> ProjectIds, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return await Task.Run(() =>
            {
                string connectionString = _configuration.GetConnectionString("PIM");

                using SqlConnection connection = new SqlConnection(connectionString);
                var command = new SqlCommand();
                                
                StringBuilder sb = new StringBuilder("SELECT * FROM [DeltekPIM].[dbo].[EXVW_Project_Invoice_Exported_No_Contacts] WHERE [Project_Code] in (");

                sb.Append($"'{ProjectIds.ElementAt(0)}'");

                for (int i = 1; i < ProjectIds.Count(); i++)
                {
                    sb.Append($", '{ProjectIds.ElementAt(i)}'");
                }

                sb.Append(")");

                CultureInfo cInfo = new CultureInfo("en-us");

                if (fromDate != null)
                {
                    sb.Append($" AND [Document_Date] >= Convert(datetime, '{fromDate.Value.ToString("d", cInfo)}' )");
                }

                if (toDate != null)
                {
                    sb.Append($" AND [Document_Date] <= Convert(datetime, '{toDate.Value.ToString("d", cInfo)}' )");
                }

                command.CommandText = sb.ToString();
                command.Connection = connection;

                using var dataSet = new DataSet("Invoices");
                using var adapter = new SqlDataAdapter { SelectCommand = command };
                adapter.Fill(dataSet);

                return BuildList(dataSet);
            });
        }

        private async Task<List<DraftInvoiceModel>> GetDraftInvoiceCollection(IEnumerable<string> ProjectIds, DateTime? fromDate, DateTime? toDate)
        {
            return await Task.Run(() =>
            {
                string connectionString = _configuration.GetConnectionString("PIM");

                using SqlConnection connection = new SqlConnection(connectionString);
                var command = new SqlCommand();

                StringBuilder sb = new StringBuilder("SELECT * FROM [DeltekPIM].[dbo].[EXVW_Finance_Unapproved_Invoices] WHERE [Project_Code] in (");

                sb.Append($"'{ProjectIds.ElementAt(0)}'");

                for (int i = 1; i < ProjectIds.Count(); i++)
                {
                    sb.Append($", '{ProjectIds.ElementAt(i)}'");
                }

                sb.Append(")");

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

                using var dataSet = new DataSet("DraftInvoices");
                using var adapter = new SqlDataAdapter { SelectCommand = command };
                adapter.Fill(dataSet);

                return BuildDraftList(dataSet);
            });
        }

        private static List<InvoiceModel> BuildList(DataSet dataSet)
        {
            var list = new List<InvoiceModel>();
            foreach (DataRow? row in dataSet.Tables[0].Rows)
            {
                if (row is null)
                {
                    continue;
                }

                list.Add(new InvoiceModel(row));
            }

            return list;
        }

        private static List<DraftInvoiceModel> BuildDraftList(DataSet dataSet)
        {
            var list = new List<DraftInvoiceModel>();
            foreach (DataRow? row in dataSet.Tables[0].Rows)
            {
                if (row is null)
                {
                    continue;
                }

                list.Add(new DraftInvoiceModel(row));
            }

            return list;
        }
    }
}
