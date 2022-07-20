using AutoMapper;
using CommonDataModels;
using Jpp.Projects;
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
        private readonly IMapper _mapper;

        public InvoicesController(IInvoiceService invoiceService, IMapper mapper)
        {
            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
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
    }
}

