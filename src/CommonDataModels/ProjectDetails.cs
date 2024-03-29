﻿namespace CommonDataModels
{
    public class ProjectDetails
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public string DeltekId { get; set; }
        public string AnticipatedFolderPath { get; set; }

        public ProjectStatus Status { get; set; }

        public FinanceCompany Company { get; set; }

        public IList<ProjectWorkstage> Workstages { get; set; }

        public IList<ProjectContact> ProjectOwners { get; set; }
        public IList<ProjectContact> ProjectContacts { get; set; }

        public IList<Invoice> Invoices { get; set; }
        public IList<PurchaseOrder> PurchaseOrders { get; set; }
    }

    public enum ProjectStatus
    {
        Live,
        Enquiry,
        Completed,
        Abandoned
    }
}
