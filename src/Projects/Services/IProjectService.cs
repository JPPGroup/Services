using System.Collections.Generic;
using System.Threading.Tasks;
using Jpp.Web.Service.Models;

namespace Jpp.Web.Service
{
    public interface IProjectService
    {
        Task<IList<Project>> ListAsync(Company company);
    }
}
