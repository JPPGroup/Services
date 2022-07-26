using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Jpp.Projects.Models;
using Jpp.Projects.Resources;
using CommonDataModels;
using Projects.Services;

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
