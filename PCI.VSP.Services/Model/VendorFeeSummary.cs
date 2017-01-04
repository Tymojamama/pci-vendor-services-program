using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services.Model
{
    public class VendorFeeSummary
    {
        public Guid ProjectVendorId { get; set; }
        public Guid VendorProductId { get; set; }
        public String ProductName { get; set; }
        public DateTime? LastUpdated { get; set; }
        public Decimal PercentComplete { get; set; }
        internal Guid ClientProjectId { get; set; }
        public String Status { get; set; }
    }
}
