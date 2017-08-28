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
        private bool StopFilterExecution()
        {
            bool stopFilterExecution = false;
            stopFilterExecution = (_maximumResults == 0);
            stopFilterExecution = (_vendorProducts == null);
            stopFilterExecution = (_vendorProducts.Count == 0);
            return stopFilterExecution;
        }
    }
}
