using AutoMapper;
using CommonDataModels;
using Jpp.Projects;
using Jpp.Projects.Models;
using Microsoft.AspNetCore.Mvc;
using Projects.Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Projects.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class InvoicesController
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public InvoicesController(IInvoiceService invoiceService, IProjectService projectService, IMapper mapper)
        {
            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Invoice>))]
        [SwaggerOperation(OperationId = "getProjectInvoices")]
        public async Task<IEnumerable<Invoice>> GetAsync(string projectCode)
        {
            var invoices = await _invoiceService.ListByProjectAsync(projectCode);
            var resources = _mapper.Map<IEnumerable<InvoiceModel>, IEnumerable<Invoice>>(invoices);
            return resources;
        }

        [HttpGet("overdue")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Invoice>))]
        [SwaggerOperation(OperationId = "getCompanyUnpaidInvoices")]
        public async IAsyncEnumerable<Invoice> GetUnpaidAsync(Company company)
        {
            var projects = await _projectService.ListAsync(company);            

            foreach (Project p in projects)
            {

                var invoices = await _invoiceService.ListByProjectAsync(p.Code);
                var resources = _mapper.Map<IEnumerable<InvoiceModel>, IEnumerable<Invoice>>(invoices);
                foreach (Invoice invoice in resources)
                {
                    if (invoice.TotalUnpaid > 0)
                    {
                        yield return invoice;
                    }
                }
            }
        }
    }
}

