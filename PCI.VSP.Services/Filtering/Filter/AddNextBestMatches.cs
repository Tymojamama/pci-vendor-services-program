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
        private void AddNextBestMatches()
        {
            int currentMatchCount = -1;
            foreach (int rank in ClientQuestions.Select(cq => cq.Rank).Distinct().OrderByDescending(rank => rank))
            {
                List<VendorProductFilterResult> nextBestMatches = _vendorProductFilterResults.Where(z => z.QuestionRank == rank && z.Passed == false).OrderByDescending(z => z.MatchCount).ToList();

                foreach (VendorProductFilterResult nextBestMatch in nextBestMatches)
                {
                    if (currentMatchCount == -1)
                    {
                        currentMatchCount = nextBestMatch.MatchCount;
                        nextBestMatch.Passed = true;
                    }
                    else if (currentMatchCount == nextBestMatch.MatchCount)
                    {
                        nextBestMatch.Passed = true;
                    }
                    else if (currentMatchCount > nextBestMatch.MatchCount && _totalVendorCount < _maximumResults)
                    {
                        currentMatchCount = nextBestMatch.MatchCount;
                        nextBestMatch.Passed = true;
                    }
                    else
                    {
                        break;
                    }

                    _totalVendorCount = (_vendorProductFilterResults.GroupBy(z => new { z.VendorId, z.VendorProductId }).Select(y => y.OrderByDescending(z => z.QuestionRank).First())).Where(z => z.Passed).Select(z => z.VendorId).Distinct().Count();
                }

                if (_totalVendorCount >= _maximumResults)
                {
                    break;
                }
            }
        }
    }
}
