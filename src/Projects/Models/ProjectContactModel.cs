using Jpp.Common.DataModels;
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
                    return Jpp.Common.DataModels.Role.CivilTechnician;

                case 14:
                    return Jpp.Common.DataModels.Role.ClientContact;

                case 21:
                    return Jpp.Common.DataModels.Role.DevelopmentPlanningTechnician;

                case 35:
                    return Jpp.Common.DataModels.Role.CivilProjectLead;

                case 52:
                    return Jpp.Common.DataModels.Role.ProjectDirector;

                case 58:
                    return Jpp.Common.DataModels.Role.DevelopmentPlanningProjectLead;

                case 54:
                    return Jpp.Common.DataModels.Role.CivilEngineer;

                case 57:
                    return Jpp.Common.DataModels.Role.DevelopmentPlanningEngineer;

                case 60:
                    return Jpp.Common.DataModels.Role.SpecialProjectsTechnician;

                case 62:
                    return Jpp.Common.DataModels.Role.SpecialProjectsEngineer;

                case 63:
                    return Jpp.Common.DataModels.Role.SpecialProjectsProjectLead;

                case 64:
                    return Jpp.Common.DataModels.Role.StructuralTechnician;

                case 65:
                    return Jpp.Common.DataModels.Role.StructuralEngineer;

                case 67:
                    return Jpp.Common.DataModels.Role.StructuralProjectLead;

                case 69:
                    return Jpp.Common.DataModels.Role.ProjectOwner;

                default:
                    return Jpp.Common.DataModels.Role.Unknown;
            }
        }
    }
}
