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

namespace PCI.VSP.Services
{
    public partial class FilterService
    {
        /// <summary>
        /// Saves the results of the executed filter by updating database records.
        /// </summary>
        /// <returns>Returns a list of vendor product filter results</returns>
        private List<Filter.VendorProductFilterResult> SaveFilterResults()
        {
            Trace.TraceInformation("Entering " + MethodBase.GetCurrentMethod().Name);

            if (_filterResultsAll == null || _filterResultsAll.Count == 0)
            {
                return null;
            }

            List<Filter.VendorProductFilterResult> selectedFilterResults = new List<Filter.VendorProductFilterResult>();
            selectedFilterResults.AddRange(_filterResultsAll.GroupBy(z => new { z.VendorId, z.VendorProductId }).Select(y => y.OrderByDescending(z => z.QuestionRank).First()).Where(z => z.IsBenchmark)); // add benchmark vendor products

            SaveAllFilterResults();

            Trace.TraceInformation("Exiting " + MethodBase.GetCurrentMethod().Name);

            return selectedFilterResults;
        }
    }
}
