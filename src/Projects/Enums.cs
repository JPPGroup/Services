using System.ComponentModel;

namespace Jpp.Projects
{
    public enum Company : byte
    {
        All = 0,
        [Description("JPP Consulting Ltd")] Consulting = 1,
        [Description("JPP Geotechnical & Environmental Ltd")] Geo = 2,
        [Description("JPP Surveying Ltd")] Surveying = 3,
        [Description("W E Architecture Ltd")] WeArchitects = 4,
        [Description("Smith Foster")] SmithFoster = 5,
        [Description("Flowerpot Holdings Limited")] Flowerpot = 6
    }

    public enum Category : byte
    {
        [Description("Architecture(New Build)")] ArchitectureNewBuild = 1,
        [Description("Civil Engineering")] CivilEngineering = 2,
        [Description("Structural Surveys")] StructuralSurveys = 3,
        [Description("Structural Engineering")] StructuralEngineering = 4,
        [Description("Soil Engineering")] SoilEngineering = 5,
        [Description("Project Co-ordination")] ProjectCoordination = 6,
        [Description("Project Management")] ProjectManagement = 9,
        [Description("Principal Designer")] PrincipalDesigner = 10,
        [Description("Other")] Other = 20,
        [Description("Surveying")] Surveying = 23,
        [Description("Architecture(Refurbishments)")] ArchitectureRefurbishments = 24,
        [Description("Architecture Consultancy")] ArchitectureConsultancy = 25,
        [Description("Development Planning")] DevelopmentPlanning = 26,
    }
}