﻿using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Jpp.Projects.Models;
using Microsoft.Extensions.Logging;

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

        private static SqlCommand CreateProjectListSqlCommand(Company company)
        {
            var cmd = new SqlCommand();

            switch (company)
            {
                case Company.All:
                    cmd.CommandText =
                        "SELECT Project_Code, Name, Project_Category FROM EXVW_Project_Data WHERE Project_Code LIKE '[0-9]%'";
                    break;

                case Company.Consulting:
                    cmd.CommandText =
                        "SELECT Project_Code, Name, Project_Category FROM EXVW_Project_Data WHERE Project_Code LIKE '[0-9]%' AND Project_Code NOT LIKE '%SF' AND (Project_Category = 'Civil Engineering' OR Project_Category = 'Structural Engineering'  OR Project_Category = 'Structural Surveys' OR Project_Category = 'Development Planning')";
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
    }
}
