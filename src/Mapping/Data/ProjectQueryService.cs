using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Mapping.Data
{
    public class ProjectQueryService
    {
        private ILogger<ProjectQueryService> _logger;

        public ProjectQueryService(ILogger<ProjectQueryService> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Project>> GetAllProjects()
        {
            List<Project> projects = new List<Project>();

            int records = 0;
            int rejected = 0;

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = "JPP-PIM-SVR.cedarbarn.local";
                builder.UserID = "jpp";
                builder.Password = "@deltek";
                builder.InitialCatalog = "DeltekPIM";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT [Name],[Project_Category],[udftext1],[udftext2],[Project_ID]");
                    sb.Append("FROM [dbo].[EXVW_Project_Data] WHERE [udftext1]!='NULL' AND [udftext2]!='NULL';");
                    String sql = sb.ToString();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int dummy;

                                bool valid = int.TryParse(reader["udftext1"].ToString(), out dummy) &&
                                             int.TryParse(reader["udftext2"].ToString(), out dummy);

                                records++;

                                if (valid)
                                {
                                    Project p = new Project()
                                    {
                                        Name = reader["Name"].ToString(),
                                        ID = reader["Project_ID"].ToString(),
                                        Location = new LatLong()
                                        {
                                            Easting = int.Parse(reader["udftext1"].ToString()),
                                            Northing = int.Parse(reader["udftext2"].ToString())
                                        },
                                        Category = reader["Project_Category"].ToString()
                                    };

                                    projects.Add(p);
                                }
                                else
                                {
                                    rejected++;
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                _logger.LogError(e, "Exception loading project information");
            }

            _logger.LogInformation($"{records} processed, {rejected} rejected.");

            return projects;
        }

        public async Task<string> GetAllProjectsAsGeoJson()
        {
            IEnumerable<Project> projects = await GetAllProjects();
            StringBuilder sb = new StringBuilder("[");
            foreach (Project project in projects)
            {
                string projectData = project.ToGeoJson();
                sb.Append(projectData);
                sb.Append(",");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            return sb.ToString();
        }
    }

    public class Project
    {
        public string Name { get; set; }
        
        public string Category { get; set; }

        public string ID { get; set; }

        public LatLong Location { get; set; }

        public string ToGeoJson()
        {
            return
                $"{{\"type\": \"Feature\",\"geometry\":{{\"type\": \"Point\",\"coordinates\": [{Location.Longitude}, {Location.Latitude}]}},\"properties\":{{\"name\":\"{Name}\",\"category\":\"{Category}\",\"id\":\"{ID}\"}}}}";
        }
    }
}
