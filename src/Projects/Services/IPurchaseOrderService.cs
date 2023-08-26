using Jpp.Projects.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jpp.Projects
{
    public interface IPurchaseOrderService
    {
        Task<IList<PurchaseOrderModel>> ListByProjectsAsync(IEnumerable<string> ProjectIds, DateTime? fromDate, DateTime? toDate);

        Task<IList<PurchaseOrderLineInvoiceModel>> ListInvoicesByProjectsAsync(IEnumerable<string> ProjectIds, DateTime? fromDate, DateTime? toDate);
    }
}
