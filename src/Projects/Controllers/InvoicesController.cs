using AutoMapper;
using Jpp.Common.DataModels;
using Jpp.Projects;
using Jpp.Projects.Models;
using Microsoft.AspNetCore.Mvc;
using Projects.Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
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
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IMapper _mapper;

        public InvoicesController(IInvoiceService invoiceService, IProjectService projectService, IMapper mapper, IPurchaseOrderService poService)
        {
            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _purchaseOrderService = poService ?? throw new ArgumentNullException(nameof(poService)); ;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Invoice>))]
        [SwaggerOperation(OperationId = "getProjectInvoices")]
        public async Task<IEnumerable<Invoice>> GetAsync(string projectCode, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var invoices = await _invoiceService.ListByProjectAsync(projectCode, fromDate, toDate);
            var resources = _mapper.Map<IEnumerable<InvoiceModel>, IEnumerable<Invoice>>(invoices);
            return resources;
        }

        [HttpGet("bycompany")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Invoice>))]
        [SwaggerOperation(OperationId = "getCompanyInvoices")]
        public async IAsyncEnumerable<Invoice> GetUnpaidAsync(Company company, bool unpaidonly = false, bool includedrafts = false, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var projects = await _projectService.ListAsync(company);
            List<string> projectCodes = new List<string>();

            foreach (Project p in projects)
            {
                projectCodes.Add(p.Code);
            }

            var invoices = await _invoiceService.ListByProjectsAsync(projectCodes, fromDate, toDate);

            foreach (InvoiceModel invoice in invoices)
            {
                Invoice i = _mapper.Map<InvoiceModel, Invoice>(invoice);

                if (unpaidonly)
                {
                    if (invoice.TotalUnpaid != 0)
                    {
                        yield return i;
                    }
                }
                else
                {
                    yield return i;
                }
            }

            if (includedrafts)
            {
                var draftinvoices = await _invoiceService.ListDraftsByProjectsAsync(projectCodes, fromDate, toDate);

                foreach (DraftInvoiceModel invoice in draftinvoices)
                {
                    Invoice i = _mapper.Map<DraftInvoiceModel, Invoice>(invoice);
                    yield return i;
                }
            }
        }

        [HttpGet("incomingbycompany")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Invoice>))]
        [SwaggerOperation(OperationId = "getIncomingCompanyInvoices")]
        public async IAsyncEnumerable<PurchaseOrderLineInvoice> GetIncomingAsync(Company company, bool includedrafts = false, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var projects = await _projectService.ListAsync(company);
            List<string> projectCodes = new List<string>();

            foreach (Project p in projects)
            {
                projectCodes.Add(p.Code);
            }

            var invoices = await _purchaseOrderService.ListInvoicesByProjectsAsync(projectCodes, fromDate, toDate);

            foreach (PurchaseOrderLineInvoiceModel invoice in invoices)
            {
                PurchaseOrderLineInvoice i = _mapper.Map<PurchaseOrderLineInvoiceModel, PurchaseOrderLineInvoice>(invoice);

                yield return i;
            }
        }
    }
}

