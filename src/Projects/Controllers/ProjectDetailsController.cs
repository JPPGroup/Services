using Jpp.Common.DataModels;
using Microsoft.AspNetCore.Mvc;
using Projects.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Jpp.Projects.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectDetailsController : ControllerBase
    {
        private readonly ProjectDetailsFactory _factory;

        public ProjectDetailsController(ProjectDetailsFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ProjectDetails>))]
        [SwaggerOperation(OperationId = "getProjectDetails")]
        public async Task<ProjectDetails> GetAsync(string projectCode)
        {
            return await _factory.GetProject(projectCode);
        }
    }
}
