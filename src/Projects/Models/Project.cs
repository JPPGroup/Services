using CommonDataModels;
using System;
using System.Data;

namespace Jpp.Projects.Models
{
    public class Project : BaseModel
    {
        public Project(DataRow row) : base(row)
        {
        }

        public string Code => this.GetCode();
        public string Name => this.GetName();
        public int Category => this.GetCategory();
        public Company Company => this.GetCompany();

        public string DeltekId => this.GetId();

        public ProjectStatus Status => this.GetStatus();

        private string GetCode()
        {
            return this.TryGetRowValue("Project_Code", out var rowValue) ? (string)rowValue : string.Empty;
        }

        private string GetName()
        {
            return this.TryGetRowValue("Name", out var rowValue) ? (string)rowValue : string.Empty;
        }

        private ProjectStatus GetStatus()
        {
            int enumvalue = this.TryGetRowValue("Project_Status_ID", out var rowValue) ? (int)rowValue : throw new ArgumentOutOfRangeException($"Unknonw value {rowValue} returned for status");
            switch (enumvalue)
            {
                case 2:
                    return ProjectStatus.Enquiry;
                case 4:
                    return ProjectStatus.Live;
                case 5:
                    return ProjectStatus.Completed;
                case 7:
                    return ProjectStatus.Abandoned;

                default:
                    throw new ArgumentOutOfRangeException($"Unknown value {rowValue} returned for status");
            }
        }

        private string GetId()
        {
            return this.TryGetRowValue("Project_ID", out var rowValue) ? ((int)rowValue).ToString() : string.Empty;
        }

        private int GetCategory()
        {
            string catName = this.TryGetRowValue("Project_Category", out var rowValue) ? (string)rowValue : string.Empty;

            switch (catName)
            {
                case "Civil Engineering":
                    return (int)Projects.Category.CivilEngineering;

                case "Structural Engineering":
                    return (int)Projects.Category.StructuralEngineering;

                case "Structural Surveys":
                    return (int)Projects.Category.StructuralSurveys;

                case "Development Planning":
                    return (int)Projects.Category.DevelopmentPlanning;

                case "Surveying":
                    return (int)Projects.Category.Surveying;

                case "Soil Engineering":
                    return (int)Projects.Category.SoilEngineering;

                case "Architecture (New Build)":
                    return (int)Projects.Category.ArchitectureNewBuild;

                case "Architecture ( Refurbishments)":
                    return (int)Projects.Category.ArchitectureRefurbishments;

                default:
                    return (int)Projects.Category.Other;
            }
        }

        private Company GetCompany()
        {
            int catName = this.TryGetRowValue("Finance_Company_ID", out var rowValue) ? (int)rowValue : 0;

            switch (catName)
            {
                case 1:
                    return Company.Consulting;

                case 2:
                    return Company.Geo;

                case 3:
                    return Company.Surveying;

                case 4:
                    return Company.WeArchitects;

                case 5:
                    return Company.SmithFoster;

                default:
                    return Company.All;

            }
        }
    }
}
