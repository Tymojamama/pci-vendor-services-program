using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCI.VSP.Web.CrmIFrames.VendorMonitoring
{
    public class PopulateQAControlEventArgs : EventArgs
    {
        public Guid serviceProviderId { get; set; }
    }
}