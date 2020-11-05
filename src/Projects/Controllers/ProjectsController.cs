using AutoMapper;
using Jpp.Web.Service.Models;
using Jpp.Web.Service.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Jpp.Web.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService projectService;
        private readonly IMapper mapper;

        public ProjectsController(IProjectService projectService, IMapper mapper)
        {
            this.projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("Cons")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ProjectResource>))]
        [SwaggerOperation(OperationId = "getConsultingProjects")]
        public async Task<IEnumerable<ProjectResource>> GetConsultingAsync()
        {
            var projects = await projectService.ListAsync(Company.Cons);
            var resources = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectResource>>(projects);
            return resources;
        }

        [HttpGet("Geo")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ProjectResource>))]
        [SwaggerOperation(OperationId = "getGeotechnicalProjects")]
        public async Task<IEnumerable<ProjectResource>> GetGeotechnicalAsync()
        {
            var projects = await projectService.ListAsync(Company.Geo);
            var resources = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectResource>>(projects);
            return resources;
        }

        [HttpGet("Surv")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ProjectResource>))]
        [SwaggerOperation(OperationId = "getSurveyingProjects")]
        public async Task<IEnumerable<ProjectResource>> GetSurveyingAsync()
        {
            var projects = await projectService.ListAsync(Company.Surv);
            var resources = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectResource>>(projects);
            return resources;
        }

        [HttpGet("Wea")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ProjectResource>))]
        [SwaggerOperation(OperationId = "getArchitectureProjects")]
        public async Task<IEnumerable<ProjectResource>> GetArchitectureAsync()
        {
            var projects = await projectService.ListAsync(Company.Wea);
            var resources = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectResource>>(projects);
            return resources;
        }

        [HttpGet("Sf")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ProjectResource>))]
        [SwaggerOperation(OperationId = "getSmithFosterProjects")]
        public async Task<IEnumerable<ProjectResource>> GetSmithFosterAsync()
        {
            var projects = await projectService.ListAsync(Company.Sf);
            var resources = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectResource>>(projects);
            return resources;
        }

        [HttpGet("Fp")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ProjectResource>))]
        [SwaggerOperation(OperationId = "getFlowerpotProjects")]
        public async Task<IEnumerable<ProjectResource>> GetFlowerpotAsync()
        {
            var projects = await projectService.ListAsync(Company.Fp);
            var resources = mapper.Map<IEnumerable<Project>, IEnumerable<ProjectResource>>(projects);
            return resources;
        }
    }
}
