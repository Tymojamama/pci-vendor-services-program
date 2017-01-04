using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Data.Classes
{
    public class VendorProductSummary
    {
        public Guid VendorProductId { get; set; }
        public String ProductName { get; set; }
        public DateTime? LastUpdated { get; set; }
        public String LastUpdatedBy { get; set; }
        public Decimal PercentComplete { get; set; }
    }
}
