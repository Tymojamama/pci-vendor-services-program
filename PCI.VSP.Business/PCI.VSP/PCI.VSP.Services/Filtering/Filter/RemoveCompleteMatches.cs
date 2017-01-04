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
        private void RemoveCompleteMatchesFromVendorProducts(int rank)
        {
            // Remove complete (100%) matches from further processing
            foreach (VendorProductFilterResult vendorProductFilterResult in _vendorProductFilterResults.Where(z => z.Passed && z.CompleteMatch && z.QuestionRank == rank))
            {
                _vendorProducts.Remove(_vendorProducts.Where(z => z.VendorId == vendorProductFilterResult.VendorId && z.VendorProductId == vendorProductFilterResult.VendorProductId).FirstOrDefault());
            }
        }
    }
}
