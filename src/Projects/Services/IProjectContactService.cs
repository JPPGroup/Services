using System.Collections.Generic;
using System.Threading.Tasks;
using CommonDataModels;
using Jpp.Projects.Models;
using Projects.Models;

namespace Jpp.Projects
{
    public interface IProjectContactService
    {
        Task<IList<ProjectContactModel>> ListByProjectAsync(string ProjectId);        
    }
}
