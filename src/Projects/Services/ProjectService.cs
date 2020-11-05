using Jpp.Web.Service.Adapters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Jpp.Web.Service.Models;

namespace Jpp.Web.Service
{
    public class ProjectService : IProjectService
    {
        private readonly ILoggerAdapter<ProjectService> logger;
        private readonly IConfiguration configuration;
        private readonly IMemoryCache cache;

        public ProjectService(ILoggerAdapter<ProjectService> logger, IConfiguration configuration, IMemoryCache cache)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public Task<IList<Project>> ListAsync(Company company)
        {
            return Task.FromResult(GetProjectList(company));
        }

        private IList<Project> GetProjectList(Company company)
        {
            try
            {
                return cache.GetOrCreate($"Projects{company}", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return this.GetCompanyProjects(company);
                });

            }
            catch (SqlException ex)
            {
                this.logger.LogError(ex, "Unable to get projects.");
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

        private List<Project> GetCompanyProjects(Company company)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("PIM"));
            var command = CreateProjectListSqlCommand(company);
            command.Connection = connection;

            using var dataSet = new DataSet("Projects");
            using var adapter = new SqlDataAdapter { SelectCommand = command };
            adapter.Fill(dataSet);

            return BuildProjectList(dataSet);
        }
    }
}
