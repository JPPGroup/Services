﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDataModels
{
    public class ProjectDetails
    {
        public string Code { get; set; }
        public string Name { get; set; }        
        
        public FinanceCompany Company { get; set; }

        public IList<Invoice> Invoices { get; set; }
    }
}
