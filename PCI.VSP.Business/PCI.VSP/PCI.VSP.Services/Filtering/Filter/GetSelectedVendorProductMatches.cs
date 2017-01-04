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
        private List<VendorProductRankMatch> __GetSelectedVendorProductMatches(Guid _clientProjectId, FilterPhases _filterPhase, IOrderedEnumerable<VendorProductRankMatch> _vendorProductMatchesOrdered, Int32? _maxResults)
        {
            List<VendorProductRankMatch> _list = new List<VendorProductRankMatch>();

            // Benchmarking:
            // 1. get projectvendors based on clientprojectid
            List<ProjectVendor> _projectVendors = new ProjectVendorDataLogic(_getDefaultAuthRequest()).RetrieveMultipleByClientProject(_clientProjectId);

            // 2. check benchmarks based on the filtercategory & add those to the selection list no matter what
            foreach (ProjectVendor _projectVendor in _projectVendors)
            {
                bool _isPhaseOneBenchmark = (_filterPhase == FilterPhases.Phase1 && _projectVendor.Phase1Benchmark);
                bool _isPhaseTwoBenchmark = (_filterPhase == FilterPhases.Phase2 && _projectVendor.Phase2Benchmark);

                if (_isPhaseOneBenchmark || _isPhaseTwoBenchmark)
                {
                    VendorProductRankMatch _vendorProductMatch = new VendorProductRankMatch()
                    {
                        IsBenchmark = true,
                        Rank = 1,
                        VendorProductId = _projectVendor.VendorProductId
                    };

                    _list.Add(_vendorProductMatch);
                }
            }

            foreach (VendorProductRankMatch _vendorProductMatch in _vendorProductMatchesOrdered.OrderByDescending(a => a.Matches))
            {
                if (_maxResults.HasValue && _list.Count >= _maxResults)
                {
                    break;
                }

                _list.Add(_vendorProductMatch);
            }

            return _list;
        }
    }
}
