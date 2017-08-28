using PCI.VSP.Data;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Data.CRM.Model;
using PCI.VSP.Data.Enums;
using PCI.VSP.Services.Filtering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PCI.VSP.Services.Filtering
{
    public partial class FilterSummary
    {
        private string GetSummary()
        {
            if (AllResultsFromFilter == null || AllResultsFromFilter.Count == 0 || SelectedProductsFromFilter == null || SelectedProductsFromFilter.Count == 0)
            {
                return "The filter located no matches.";
            }

            WriteBenchesMatches();
            WriteQuestionComparisons();

            return StringBuilder.ToString();
        }
    }
}
