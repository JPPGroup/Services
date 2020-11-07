using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Jpp.Projects.Models;
using Microsoft.Extensions.Logging;

namespace Jpp.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public ProjectService(ILogger<ProjectService> logger, IConfiguration configuration, IMemoryCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<IList<Project>> ListAsync(Company company)
        {
            return await GetProjectList(company);
        }

        private async Task<IList<Project>> GetProjectList(Company company)
        {
            try
            {
                return await _cache.GetOrCreateAsync($"Projects{company}", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return this.GetCompanyProjects(company);
                });

            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unable to get projects.");
                return new List<Project>();
            }
        }

        private static SqlCommand CreateProjectListSqlCommand(Company company)
        {
            var cmd = new SqlCommand();
            cmd.Parameters.Add("@company", SqlDbType.Int).Value = company;
            cmd.CommandText = "SELECT Project_Code, Name, Project_Category_ID FROM U2VW_Finance_Search_Project_Base WHERE Project_Code LIKE '[0-9]%' AND Finance_Company_ID = @company";
            return cmd;
        }

        private static List<Project> BuildProjectList(DataSet dataSet)
        {
            var list = new List<Project>();
            foreach (DataRow? row in dataSet.Tables[0].Rows)
            {
                if (row is null)
                {
                    continue;
                }

                list.Add(new Project(row));
            }

            return list;
        }

        private async Task<List<Project>> GetCompanyProjects(Company company)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("PIM"));
            var command = CreateProjectListSqlCommand(company);
            command.Connection = connection;

            using var dataSet = new DataSet("Projects");
            using var adapter = new SqlDataAdapter { SelectCommand = command };
            adapter.Fill(dataSet);

            return BuildProjectList(dataSet);
        }
    }
}
