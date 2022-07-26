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
    public class ProjectContactService : IProjectContactService
    {
        private readonly ILogger<ProjectContactService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public ProjectContactService(ILogger<ProjectContactService> logger, IConfiguration configuration, IMemoryCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<IList<ProjectContactModel>> ListByProjectAsync(string ProjectId)
        {
            try
            {
                return await _cache.GetOrCreateAsync($"Contacts{ProjectId}", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return GetContacts(ProjectId);
                });

            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unable to get contacts.");
                return new List<ProjectContactModel>();
            }
        }

        private async Task<List<ProjectContactModel>> GetContacts(string ProjectId)
        {
            return await Task.Run(() =>
            {
                string connectionString = _configuration.GetConnectionString("PIM");

                using SqlConnection connection = new SqlConnection(connectionString);
                var command = new SqlCommand(); 
                //TODO: Optimise this, way too heavy for regular use!!
                command.CommandText = $"SELECT * FROM [DeltekPIM].[dbo].[EXVW_Project_Contacts] INNER JOIN [DeltekPIM].[dbo].[EXVW_Project_Data] ON [DeltekPIM].[dbo].[EXVW_Project_Contacts].[Project_ID] = [DeltekPIM].[dbo].[EXVW_Project_Data].[Project_ID] WHERE [Project_Code]='{ProjectId}'";
                command.Connection = connection;

                using var dataSet = new DataSet("Contacts");
                using var adapter = new SqlDataAdapter { SelectCommand = command };
                adapter.Fill(dataSet);

                return BuildList(dataSet);
            });
        }

        private static List<ProjectContactModel> BuildList(DataSet dataSet)
        {
            var list = new List<ProjectContactModel>();
            foreach (DataRow? row in dataSet.Tables[0].Rows)
            {
                if (row is null)
                {
                    continue;
                }

                list.Add(new ProjectContactModel(row));
            }

            return list;
        }
    }
}
