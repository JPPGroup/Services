using Jpp.Projects.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Jpp.Projects.Services
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
            try
            {
                return await _cache.GetOrCreateAsync($"Projects{company}", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return GetProjectsByCompany(company);
                });

            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unable to get projects.");
                return new List<Project>();
            }
        }

        public async Task<IList<Project>> ListAsyncByUser(string firstname, string lastname)
        {
            try
            {
                return await _cache.GetOrCreateAsync($"Projects-{firstname}-{lastname}", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return GetProjectsByUser(firstname, lastname);
                });

            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unable to get projects.");
                return new List<Project>();
            }
        }

        private async Task<List<Project>> GetProjectsByCompany(Company company)
        {
            return await Task.Run(() =>
            {
                string connectionString = _configuration.GetConnectionString("PIM");

                using SqlConnection connection = new SqlConnection(connectionString);
                var command = CreateProjectListSqlCommand(company);
                command.Connection = connection;

                using var dataSet = new DataSet("Projects");
                using var adapter = new SqlDataAdapter { SelectCommand = command };
                adapter.Fill(dataSet);

                return BuildProjectList(dataSet);
            });
        }

        private async Task<List<Project>> GetProjectsByUser(string firstname, string lastname)
        {
            return await Task.Run(() =>
            {
                string connectionString = _configuration.GetConnectionString("PIM");

                using SqlConnection connection = new SqlConnection(connectionString);
                var command = new SqlCommand($"SELECT [DeltekPIM].[dbo].[EXVW_Project_Data].[Project_ID],[Contact_ID], [EXVW_Project_Data.Project_Status_ID], [Active],[Forename],[Surname],[NT_User],[Project_Code],[DeltekPIM].[dbo].[EXVW_Project_Data].[Name],[Project_Category],[Finance_Company_ID] FROM[DeltekPIM].[dbo].[EXVW_Project_Contacts] INNER JOIN[DeltekPIM].[dbo].[EXVW_Project_Data] ON[DeltekPIM].[dbo].[EXVW_Project_Contacts].[Project_ID] = [DeltekPIM].[dbo].[EXVW_Project_Data].[Project_ID] JOIN [DeltekPIM].[dbo].[EXVW_Project_Service] ON [DeltekPIM].[dbo].[EXVW_Project_Data].[Project_Code]=[DeltekPIM].[dbo].[EXVW_Project_Service].[Expr1] WHERE UPPER([Forename]) LIKE UPPER('{firstname}') AND UPPER([Surname]) LIKE UPPER('{lastname}')");
                command.Connection = connection;

                using var dataSet = new DataSet("UserProjects");
                using var adapter = new SqlDataAdapter { SelectCommand = command };
                adapter.Fill(dataSet);

                return BuildProjectList(dataSet);
            });
        }

        private static SqlCommand CreateProjectListSqlCommand(Company company)
        {
            var cmd = new SqlCommand();

            switch (company)
            {
                case Company.All:
                    cmd.CommandText =
                        "SELECT Project_ID, EXVW_Project_Data.Project_Status_ID, EXVW_Project_Data.Project_Code, EXVW_Project_Data.Name, EXVW_Project_Data.Project_Category, EXVW_Project_Service.Finance_Company_ID FROM EXVW_Project_Data JOIN EXVW_Project_Service ON EXVW_Project_Data.Project_Code=EXVW_Project_Service.Expr1 WHERE Project_Code LIKE '[0-9]%'";
                    break;

                case Company.Consulting:
                    cmd.CommandText =
                        "SELECT Project_ID, EXVW_Project_Data.Project_Status_ID, EXVW_Project_Data.Project_Code, EXVW_Project_Data.Name, EXVW_Project_Data.Project_Category, EXVW_Project_Service.Finance_Company_ID FROM EXVW_Project_Data JOIN EXVW_Project_Service ON EXVW_Project_Data.Project_Code=EXVW_Project_Service.Expr1 WHERE Project_Code LIKE '[0-9]%' AND Finance_Company_ID = 1";
                    break;

                case Company.Geo:
                    cmd.CommandText =
                        "SELECT Project_ID, EXVW_Project_Data.Project_Status_ID, EXVW_Project_Data.Project_Code, EXVW_Project_Data.Name, EXVW_Project_Data.Project_Category, EXVW_Project_Service.Finance_Company_ID FROM EXVW_Project_Data JOIN EXVW_Project_Service ON EXVW_Project_Data.Project_Code=EXVW_Project_Service.Expr1 WHERE Project_Code LIKE '[0-9]%' AND Finance_Company_ID = 2";
                    break;

                case Company.SmithFoster:
                    cmd.CommandText =
                        "SELECT Project_ID, EXVW_Project_Data.Project_Status_ID, EXVW_Project_Data.Project_Code, EXVW_Project_Data.Name, EXVW_Project_Data.Project_Category, EXVW_Project_Service.Finance_Company_ID FROM EXVW_Project_Data JOIN EXVW_Project_Service ON EXVW_Project_Data.Project_Code=EXVW_Project_Service.Expr1 WHERE Project_Code LIKE '[0-9]%' AND Finance_Company_ID = 5";
                    break;

                case Company.Surveying:
                    cmd.CommandText =
                        "SELECT Project_ID, EXVW_Project_Data.Project_Status_ID, EXVW_Project_Data.Project_Code, EXVW_Project_Data.Name, EXVW_Project_Data.Project_Category, EXVW_Project_Service.Finance_Company_ID FROM EXVW_Project_Data JOIN EXVW_Project_Service ON EXVW_Project_Data.Project_Code=EXVW_Project_Service.Expr1 WHERE Project_Code LIKE '[0-9]%' AND Finance_Company_ID = 3";
                    break;

                case Company.WeArchitects:
                    cmd.CommandText =
                        "SELECT Project_ID, EXVW_Project_Data.Project_Status_ID, EXVW_Project_Data.Project_Code, EXVW_Project_Data.Name, EXVW_Project_Data.Project_Category, EXVW_Project_Service.Finance_Company_ID FROM EXVW_Project_Data JOIN EXVW_Project_Service ON EXVW_Project_Data.Project_Code=EXVW_Project_Service.Expr1 WHERE Project_Code LIKE '[0-9]%' AND Finance_Company_ID = 4";
                    break;
            }

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

        public async Task<IList<ProjectWorkstageModel>> ListProjectWorkstages(string projectCode)
        {
            try
            {
                return await _cache.GetOrCreateAsync($"ProjectWorkstages{projectCode}", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return GetWorkstages(projectCode);
                });

            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Unable to get project workstages.");
                return new List<ProjectWorkstageModel>();
            }
        }

        private async Task<List<ProjectWorkstageModel>> GetWorkstages(string ProjectCode)
        {
            return await Task.Run(() =>
            {
                string connectionString = _configuration.GetConnectionString("PIM");

                using SqlConnection connection = new SqlConnection(connectionString);
                var command = new SqlCommand($"SELECT [DeltekPIM].[dbo].[EXVW_Finance_Workstages].[name], [DeltekPIM].[dbo].[EXVW_Finance_Workstages].[abbreviation], [DeltekPIM].[dbo].[EXVW_Finance_Workstages].[reporting_total_fee], [DeltekPIM].[dbo].[EXVW_Finance_Workstages].[total_invoices], [DeltekPIM].[dbo].[EXVW_Finance_Workstages].[total_timecost_todate] FROM [DeltekPIM].[dbo].[EXVW_Finance_Workstages] INNER JOIN [DeltekPIM].[dbo].[EXVW_Project_Data] ON [DeltekPIM].[dbo].[EXVW_Finance_Workstages].[entity_identifier] = [DeltekPIM].[dbo].[EXVW_Project_Data].[Project_ID] WHERE [DeltekPIM].[dbo].[EXVW_Project_Data].[Project_Code]='{ProjectCode}'");
                command.Connection = connection;

                using var dataSet = new DataSet("Workstages");
                using var adapter = new SqlDataAdapter { SelectCommand = command };
                adapter.Fill(dataSet);

                return BuildWorkstageList(dataSet);
            });
        }

        private static List<ProjectWorkstageModel> BuildWorkstageList(DataSet dataSet)
        {
            var list = new List<ProjectWorkstageModel>();
            foreach (DataRow? row in dataSet.Tables[0].Rows)
            {
                if (row is null)
                {
                    continue;
                }

                list.Add(new ProjectWorkstageModel(row));
            }

            return list;
        }
    }
}
