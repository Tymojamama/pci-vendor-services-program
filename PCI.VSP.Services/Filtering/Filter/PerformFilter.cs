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
        // Methods below with two underscores "__" at the beginning of their names are
        // suspected to not be used anymore other than by this method, which I am
        // confident is not used either.
        public FilterResults PerformFilter()
        {
            Trace.TraceInformation("Entering " + System.Reflection.MethodBase.GetCurrentMethod().Name);

            SetClientQuestionDefaultRanksTo99();

            List<VendorProduct> _vendorProducts = GetVendorProductsForComparison();

            if (_vendorProducts == null || _vendorProducts.Count == 0)
                return null;

            //ExcludeCurrentVendorProductsFromComparison(ClientProject.Id, _vendorProducts);

            Dictionary<Guid, List<VendorClientQuestion>> _filterComparisons = __RunFilterComparison(_vendorProducts, ClientQuestions, FilterPhase);
            List<VendorProductAnalysis> _vendorProductAnalyses = __GetVendorProductAnalyses(ClientProject.Id, _filterComparisons);
            Dictionary<Guid, VendorProductRankMatch> _vendorProductMatches = __GetVendorProductMatches(ClientQuestions, _vendorProductAnalyses);

            IOrderedEnumerable<VendorProductRankMatch> _vendorProductMatchesOrdered =
                from VendorProductRankMatch vpr in _vendorProductMatches.Values
                orderby vpr.Rank, vpr.Matches
                select vpr;

            Int32? _maxResults = new Int32?();
            GetMaximumResults(out _maxResults);

            List<VendorProductRankMatch> _selectedVendorProductMatches = __GetSelectedVendorProductMatches(ClientProject.Id, FilterPhase, _vendorProductMatchesOrdered, _maxResults);

            FilterResults _filterResults = GetComparisonResults(_filterComparisons);
            _filterResults.FilterCategory = FilterPhase;
            _filterResults.RankedMatches = _selectedVendorProductMatches;

            Trace.TraceInformation("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().Name);

            return _filterResults;
        }
    }
}
