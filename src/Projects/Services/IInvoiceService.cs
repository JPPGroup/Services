using Jpp.Projects.Models;
using Projects.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jpp.Projects
{
    public interface IInvoiceService
    {
        Task<IList<InvoiceModel>> ListByProjectAsync(string ProjectId, DateTime? fromDate, DateTime? toDate);

        Task<IList<InvoiceModel>> ListByProjectsAsync(IEnumerable<string> ProjectIds, DateTime? fromDate, DateTime? toDate);

        Task<IList<DraftInvoiceModel>> ListDraftsByProjectsAsync(IEnumerable<string> ProjectIds, DateTime? fromDate, DateTime? toDate);
    }
}
