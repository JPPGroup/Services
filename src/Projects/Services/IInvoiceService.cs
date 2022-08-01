using System.Collections.Generic;
using System.Threading.Tasks;
using CommonDataModels;
using Jpp.Projects.Models;
using Projects.Models;

namespace Jpp.Projects
{
    public interface IInvoiceService
    {
        Task<IList<InvoiceModel>> ListByProjectAsync(string ProjectId);

        Task<IList<InvoiceModel>> ListByProjectsAsync(IEnumerable<string> ProjectIds);
    }
}
