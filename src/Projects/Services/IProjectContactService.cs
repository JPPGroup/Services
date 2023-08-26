using Jpp.Projects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jpp.Projects
{
    public interface IProjectContactService
    {
        Task<IList<ProjectContactModel>> ListByProjectAsync(string ProjectId);
    }
}
