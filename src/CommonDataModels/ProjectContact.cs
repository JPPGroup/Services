using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDataModels
{
    public class ProjectContact
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public Role? Role { get; set; }
        public bool? Active { get; set; }
    }

    //TODO: Complete
    public enum Role
    {
        Unknown,
        ClientContact,
        ProjectOwner,
        CivilTechnician,
        DevelopmentPlanningTechnician,
        StructuralTechnician,
        SpecialProjectsTechnician,
        CivilEngineer,
        DevelopmentPlanningEngineer,        
        StructuralEngineer,
        SpecialProjectsEngineer,
        CivilProjectLead,
        DevelopmentPlanningProjectLead,
        StructuralProjectLead,
        SpecialProjectsProjectLead,
        ProjectDirector
    }
}

