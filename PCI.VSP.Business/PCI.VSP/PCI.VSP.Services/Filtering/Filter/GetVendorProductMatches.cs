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
        // This method may no longer be used.
        private Dictionary<Guid, VendorProductRankMatch> __GetVendorProductMatches(List<ClientQuestion> _clientQuestions, List<VendorProductAnalysis> _vendorProductAnalyses)
        {
            // get the client rankings; they're not necessarily logical
            IOrderedEnumerable<Int32> _rankings = _clientQuestions.Select(cq => cq.Rank).Distinct().OrderBy(rank => rank);

            Dictionary<Guid, VendorProductRankMatch> _vendorProductMatches = new Dictionary<Guid, VendorProductRankMatch>();
            foreach (Int32 _rank in _rankings)
            {
                foreach (VendorProductAnalysis _vendorProductAnalysis in _vendorProductAnalyses)
                {
                    if (_vendorProductAnalysis.Count == 0 || !_vendorProductAnalysis.ContainsKey(_rank) || _vendorProductAnalysis[_rank].Count == 0)
                    {
                        continue;
                    }

                    if (_vendorProductMatches.ContainsKey(_vendorProductAnalysis.VendorProductId))
                    {
                        continue;
                    }

                    VendorProductRankMatch _vendorProductMatch = new VendorProductRankMatch()
                    {
                        Matches = _vendorProductAnalysis[_rank].Count,
                        Rank = _rank,
                        VendorProductId = _vendorProductAnalysis.VendorProductId,
                        VendorId = _vendorProductAnalysis.VendorId
                    };

                    _vendorProductMatches.Add(_vendorProductAnalysis.VendorProductId, _vendorProductMatch);
                }
            }

            return _vendorProductMatches;
        }
    }
}
