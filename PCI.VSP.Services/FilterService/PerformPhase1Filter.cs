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
        /// Executes a phase one filter and organizes the results into a string.
        /// </summary>
        /// <returns>Returns the result of the filter as a string.</returns>
        private String PerformPhase1Filter()
        {
            Trace.TraceInformation("Entering " + MethodBase.GetCurrentMethod().Name);

            try
            {
                ResetFilters();

                if (_clientQuestions == null || _clientQuestions.Count == 0)
                {
                    return null;
                }

                _filterResultsAll = _filter.ExecuteFilter();
                _filterResultsSelected = SaveFilterResults();
                SaveFilterHistory();

                if (!String.IsNullOrWhiteSpace(_filterSummary.Result) && _filterSummary.Result.CompareTo("The filter located no matches.") != 0)
                {
                    _clientProjectDataLogic.SaveFilterResults(ClientProject.Id, FilterCategory.Filter1, _filterSummary.Result);
                }

                Trace.TraceInformation("Exiting " + MethodBase.GetCurrentMethod().Name);

                return _filterSummary.Result;
            }
            finally
            {
                Trace.Flush();
            }
        }
    }
}
