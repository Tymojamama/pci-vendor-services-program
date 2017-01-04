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
        private void SetTotalVendorCount()
        {
            _totalVendorCount =
                (
                    _vendorProductFilterResults
                    .GroupBy(z => new { z.VendorId, z.VendorProductId })
                    .Select(y => y.OrderByDescending(z => z.QuestionRank).First())
                )
                .Where(z => z.Passed)
                .Select(z => z.VendorId)
                .Distinct()
                .Count();
        }
    }
}
