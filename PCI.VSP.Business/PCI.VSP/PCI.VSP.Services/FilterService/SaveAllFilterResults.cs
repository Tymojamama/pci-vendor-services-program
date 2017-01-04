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
        private void SaveAllFilterResults()
        {
            List<Filter.VendorProductFilterResult> _vendorProductFilterResult = _filterResultsAll.GroupBy(z => new { z.VendorId, z.VendorProductId }).Select(y => y.OrderByDescending(z => z.QuestionRank).First()).Where(z => z.Passed && !z.IsBenchmark && !_filterResultsSelected.Contains(z)).OrderBy(z => z.QuestionRank).ToList();

            int maxResults = GetMaximumResults();

            while (maxResults > _filterResultsSelected.Select(z => z.VendorId).Distinct().Count() && _vendorProductFilterResult.Count() > 0)
            {
                AddSuccessfulMatchesToResults(_vendorProductFilterResult);
            }

            foreach (Filter.VendorProductFilterResult _selectedFilterResult in _filterResultsSelected)
            {
                if (_filter.FilterPhase == Filter.FilterPhases.Phase1)
                {
                    SavePhase1ProjectVendorResult(_selectedFilterResult);
                }
                else if (_filter.FilterPhase == Filter.FilterPhases.Phase2)
                {
                    SavePhase2ProjectVendorResult(_selectedFilterResult);
                }
            }
        }
    }
}
