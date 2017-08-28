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
        private void AddSuccessfulMatchesToResults(List<Filter.VendorProductFilterResult> _vendorProductList)
        {
            int maxCount = _vendorProductList.Max(z => z.MatchCount);

            while (_vendorProductList.Where(z => z.MatchCount == maxCount).FirstOrDefault() != null)
            {
                var item = _vendorProductList.Where(z => z.MatchCount == maxCount).FirstOrDefault();
                _filterResultsSelected.Add(item);
                _vendorProductList.Remove(item);
            }
        }
    }
}
