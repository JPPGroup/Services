using CommonDataModels;
using System.Data;

namespace Jpp.Projects.Models
{
    public class ProjectContactModel : BaseModel
    {
        
        public string? FirstName { get { return TryGetRowValue("Forename", out var rowValue) ? (string)rowValue : null; } }
        public string? LastName { get { return TryGetRowValue("Surname", out var rowValue) ? (string)rowValue : null; } }

        public Role? Role { get { return GetRole(); } }
        public bool? Active { get { return TryGetRowValue("Active", out var rowValue) ? (bool)rowValue : null; } }

        public ProjectContactModel(DataRow row) : base(row)
        { }

        private Role GetRole()
        {
            int? roleId = TryGetRowValue("Contact_Role_ID", out var rowValue) ? (int)rowValue : null;

            switch (roleId)
            {
                case 12:
                    return CommonDataModels.Role.CivilTechnician;

                case 14:
                    return CommonDataModels.Role.ClientContact;

                case 21:
                    return CommonDataModels.Role.DevelopmentPlanningTechnician;

                case 35:
                    return CommonDataModels.Role.CivilProjectLead;

                case 52:
                    return CommonDataModels.Role.ProjectDirector;

                case 58:
                    return CommonDataModels.Role.DevelopmentPlanningProjectLead;

                case 54:
                    return CommonDataModels.Role.CivilEngineer;

                case 57:
                    return CommonDataModels.Role.DevelopmentPlanningEngineer;

                case 60:
                    return CommonDataModels.Role.SpecialProjectsTechnician;

                case 62:
                    return CommonDataModels.Role.SpecialProjectsEngineer;

                case 63:
                    return CommonDataModels.Role.SpecialProjectsProjectLead;

                case 64:
                    return CommonDataModels.Role.StructuralTechnician;

                case 65:
                    return CommonDataModels.Role.StructuralEngineer;

                case 67:
                    return CommonDataModels.Role.StructuralProjectLead;

                case 69:
                    return CommonDataModels.Role.ProjectOwner;

                default:
                    return CommonDataModels.Role.Unknown;
            }
        }
    }    
}
