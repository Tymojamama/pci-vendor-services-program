using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCI.VSP.Data.Enums;

namespace PCI.VSP.Services.Model
{
    public class QuestionCategorizationItem
    {
        // Category Specific Properties
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CategorySortOrder { get; set; }
        
        // Function Specific Properties
        public Guid FunctionId { get; set; }
        public string FunctionName { get; set; }
        public int FunctionSortOrder { get; set; }

        public CategorizationQuestionTypes Type { get; set; }
        public string DisplayName { get; set; }
        public int SubItemLevel { get; set; }
        public string JSON { get; set; }

        public enum CategorizationQuestionTypes
        {
            SearchQuestion = 1, // Filter 1 or Filter 2
            PlanAssumption = 2, // Filter 1
            Fee = 3,
            InvestmentAssumption = 4, // Filter 2
            ProjectSpecificQuestion = 5, // Filter 2
            RevenueSharing = 6,
            VendorMonitoring = 7
        }
    }
}
