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
        private void SetMatchesWithRankToPassed(int totalVendorCount, int rank)
        {
            IEnumerable<int> matchCountList = _vendorProductFilterResults.Where(z => z.QuestionRank == rank && !z.CompleteMatch && !z.Passed && z.MatchCount > 0).OrderByDescending(z => z.MatchCount).Select(z => z.MatchCount).Distinct();
            
            foreach (int matchCount in matchCountList)
            {
                SetMatchesToPassed(rank, matchCount);

                totalVendorCount = (_vendorProductFilterResults.GroupBy(z => new { z.VendorId, z.VendorProductId }).Select(y => y.OrderByDescending(z => z.QuestionRank).First())).Where(z => z.Passed).Select(z => z.VendorId).Distinct().Count();

                if (totalVendorCount == _maximumResults)
                {
                    break;
                }
                else if (totalVendorCount < _maximumResults)
                {
                    RemoveProductsThatPassed(rank);
                }
                else if (totalVendorCount > _maximumResults)
                {
                    RemoveProductsThatDidNotPass(rank);
                    break;
                }
            }
        }
    }
}
