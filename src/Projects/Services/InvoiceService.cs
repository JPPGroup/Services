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

        public async Task<IList<InvoiceModel>> ListByProjectAsync(string ProjectId)
        {
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

        private async Task<List<InvoiceModel>> GetInvoices(string ProjectId)
        {
            return await Task.Run(() =>
            {
                string connectionString = _configuration.GetConnectionString("PIM");

                using SqlConnection connection = new SqlConnection(connectionString);
                var command = new SqlCommand(); 
                command.CommandText = $"SELECT * FROM [DeltekPIM].[dbo].[EXVW_Project_Invoice_Exported] WHERE [Project_Code]='{ProjectId}'";
                command.Connection = connection;

                using var dataSet = new DataSet("Invoices");
                using var adapter = new SqlDataAdapter { SelectCommand = command };
                adapter.Fill(dataSet);

                return BuildList(dataSet);
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
    }
}
