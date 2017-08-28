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
        private void FilterProductsByRanksAndMatches()
        {
            foreach (int rank in _ranks)
            {
                foreach (VendorProduct vendorProduct in _vendorProducts)
                {
                    CreateAndAddVendorProductFilterResultWithRank(vendorProduct, rank);
                }

                SetTotalVendorCount();

                if (_totalVendorCount == _maximumResults)
                {
                    break;
                }
                else if (_totalVendorCount < _maximumResults)
                {
                    RemoveCompleteMatchesFromVendorProducts(rank);
                    SetMatchesWithRankToPassed(_totalVendorCount, rank);

                    if (_totalVendorCount == _maximumResults)
                    {
                        break;
                    }
                }
                else if (_totalVendorCount > _maximumResults)
                {
                    RemoveProductsThatDidNotPass(rank);
                }
            }
        }
    }
}
