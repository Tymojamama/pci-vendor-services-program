using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Data.CRM.Model;
using System.Diagnostics;

namespace PCI.VSP.Services.Filtering
{
    public partial class Filter
    {
        private void SetMatchesToPassed(int rank, int matchCount)
        {
            foreach (VendorProductFilterResult vendorProductFilterResult in _vendorProductFilterResults.Where(z => z.QuestionRank == rank && !z.Passed && z.MatchCount == matchCount))
            {
                vendorProductFilterResult.Passed = true; // Pass Items with matches
            }
        }
    }
}
