using AutoMapper;
using CommonDataModels;
using Jpp.Projects;
using Jpp.Projects.Models;
using Microsoft.Extensions.Caching.Memory;
using Projects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projects.Services
{
    public class ProjectDetailsFactory
    {
        public IProjectService _projectService;
        public IInvoiceService _invoiceService;
        public IMemoryCache _memoryCache;
        public IMapper _mapper;

        public ProjectDetailsFactory(IProjectService projectService, IInvoiceService invoiceService, IMemoryCache memoryCache, IMapper mapper)
        {
            _projectService = projectService;
            _invoiceService = invoiceService;
            _memoryCache = memoryCache;
            _mapper = mapper;
        }

        public async Task<ProjectDetails> GetProject(string projectCode)
        {
            return await _memoryCache.GetOrCreateAsync<ProjectDetails>($"ProjectDetails{projectCode}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var projects = await _projectService.ListAsync(Jpp.Projects.Company.All);
                Project? target = projects.FirstOrDefault(p => p.Code == projectCode);               


                ProjectDetails details = _mapper.Map<Project, ProjectDetails>(target);
                details.Invoices = _mapper.Map<IList<InvoiceModel>, IList<Invoice>>(await _invoiceService.ListByProjectAsync(target.Code));

                return details;
            });            
        }        
    }
}
