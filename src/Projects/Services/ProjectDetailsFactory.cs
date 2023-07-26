using AutoMapper;
using CommonDataModels;
using Jpp.Projects;
using Jpp.Projects.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
        public IProjectContactService _projectContactService;
        public IPurchaseOrderService _purchaseOrderService;
        private ILogger<ProjectDetailsFactory> _logger;

        public ProjectDetailsFactory(IProjectService projectService, IInvoiceService invoiceService, IMemoryCache memoryCache, IMapper mapper, IProjectContactService contactService, IPurchaseOrderService poService, ILogger<ProjectDetailsFactory> logger)
        {
            _projectService = projectService;
            _invoiceService = invoiceService;
            _purchaseOrderService = poService;
            _memoryCache = memoryCache;
            _mapper = mapper;
            _projectContactService = contactService;
            _logger = logger;
        }

        public async Task<ProjectDetails> GetProject(string projectCode)
        {
            return await _memoryCache.GetOrCreateAsync<ProjectDetails>($"ProjectDetails{projectCode}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var projects = await _projectService.ListAsync(Jpp.Projects.Company.All);
                Project? target = projects.FirstOrDefault(p => p.Code == projectCode);

                ProjectDetails details = _mapper.Map<Project, ProjectDetails>(target);
                details.AnticipatedFolderPath = GenerateAnticipatedFolderPath(target);
                details.DeltekId = target.DeltekId;
                details.Invoices = _mapper.Map<IList<InvoiceModel>, IList<Invoice>>(await _invoiceService.ListByProjectAsync(target.Code, null, null));
                details.PurchaseOrders = _mapper.Map<IList<PurchaseOrderModel>, IList<PurchaseOrder>>(await _purchaseOrderService.ListByProjectsAsync(new[] { target.Code }, null, null));

                details.ProjectContacts = _mapper.Map<IList<ProjectContactModel>, IList<ProjectContact>>(await _projectContactService.ListByProjectAsync(target.Code));
                details.ProjectOwners = details.ProjectContacts.Where(pc => pc.Role == Role.ProjectOwner).ToList();

                details.Workstages = _mapper.Map<IList<ProjectWorkstageModel>, IList<ProjectWorkstage>>(await _projectService.ListProjectWorkstages(target.Code));

                return details;
            });
        }

        private string GenerateAnticipatedFolderPath(Project proj)
        {
            int numericCode = 0;
            if (!int.TryParse(proj.Code, out numericCode))
            {
                _logger.LogError("Non numeric code {ProjectCode} requested", proj.Code);
                return "";
            }
            string basePath = "";
            string groupingPath = "";
            string projectPath = $"{proj.Code} - {proj.Name}";

            switch (proj.Company)
            {
                case Company.Consulting:
                    basePath = "N:\\Consulting";
                    groupingPath = "JPP Scheme 2023 (26053 -";

                    if (numericCode < 26052)
                        groupingPath = "JPP Scheme 2022 (24259-";
                    if (numericCode < 24259)
                        groupingPath = "JPP Scheme 2021 (22316 -";
                    if (numericCode < 22316)
                        groupingPath = "JPP Scheme 2020 (20676 -";
                    if (numericCode < 20676)
                        groupingPath = "JPP Scheme 2019 (11202 -";
                    if (numericCode < 11202)
                        groupingPath = "JPP Scheme 2018 (9508 -";
                    if (numericCode < 9508)
                        groupingPath = "JPP Scheme 2017 (8810-  )";
                    if (numericCode < 8810)
                        _logger.LogError("Pre deltek Consulting job {ProjectCode} requested", proj.Code);
                    break;


                case Company.SmithFoster:
                    basePath = "P:\\SF Consulting";
                    groupingPath = "SF Scheme 2023 (26052-";

                    if (numericCode < 26052)
                        groupingPath = "SF Scheme 2022 (24287-";
                    if (numericCode < 24287)
                        groupingPath = "SF Scheme 2021 (22283-";
                    if (numericCode < 22283)
                        groupingPath = "SF Scheme 2020 (20824 -";
                    if (numericCode < 20824)
                        _logger.LogError("Pre deltek SF job {ProjectCode} requested", proj.Code);
                    break;

                default:
                    _logger.LogError("Unkown company {Company} for {ProjectCode} requested", proj.Code, proj.Company);
                    return "";
            }

            return $"{basePath}\\{groupingPath}\\{projectPath}";

        }
    }
}
