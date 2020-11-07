using System.Collections.Generic;
using System.Threading.Tasks;
using Jpp.Projects.Models;

namespace Jpp.Projects
{
    public interface IProjectService
    {
        Task<IList<Project>> ListAsync(Company company);
    }
}
