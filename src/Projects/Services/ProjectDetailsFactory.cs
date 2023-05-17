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
        public IProjectContactService _projectContactService;
        public IPurchaseOrderService _purchaseOrderService;

        public ProjectDetailsFactory(IProjectService projectService, IInvoiceService invoiceService, IMemoryCache memoryCache, IMapper mapper, IProjectContactService contactService, IPurchaseOrderService poService)
        {
            _projectService = projectService;
            _invoiceService = invoiceService;
            _purchaseOrderService = poService;
            _memoryCache = memoryCache;
            _mapper = mapper;
            _projectContactService = contactService;
        }

        public async Task<ProjectDetails> GetProject(string projectCode)
        {
            return await _memoryCache.GetOrCreateAsync<ProjectDetails>($"ProjectDetails{projectCode}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var projects = await _projectService.ListAsync(Jpp.Projects.Company.All);
                Project? target = projects.FirstOrDefault(p => p.Code == projectCode);

                ProjectDetails details = _mapper.Map<Project, ProjectDetails>(target);
                details.DeltekId = target.DeltekId;
                details.Invoices = _mapper.Map<IList<InvoiceModel>, IList<Invoice>>(await _invoiceService.ListByProjectAsync(target.Code, null, null));
                details.PurchaseOrders = _mapper.Map<IList<PurchaseOrderModel>, IList<PurchaseOrder>>(await _purchaseOrderService.ListByProjectsAsync(new[] { target.Code }, null, null));

                details.ProjectContacts = _mapper.Map<IList<ProjectContactModel>, IList<ProjectContact>>(await _projectContactService.ListByProjectAsync(target.Code));
                details.ProjectOwners = details.ProjectContacts.Where(pc => pc.Role == Role.ProjectOwner).ToList();

                details.Workstages = _mapper.Map<IList<ProjectWorkstageModel>, IList<ProjectWorkstage>>(await _projectService.ListProjectWorkstages(target.Code));

                return details;
            });
        }
    }
}
