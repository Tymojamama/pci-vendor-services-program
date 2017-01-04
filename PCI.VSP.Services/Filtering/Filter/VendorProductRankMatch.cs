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
        public struct VendorProductRankMatch
        {
            public Guid VendorId { get; set; }
            public Guid VendorProductId { get; set; }
            public Int32 Rank { get; set; }
            public Int32 Matches { get; set; }
            public Boolean IsBenchmark { get; set; }
        }
    }
}
