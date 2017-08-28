using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PCI.VSP.Data.Enums;

namespace PCI.VSP.Web.CrmIFrames.VendorMonitoring
{
    public partial class Actuals : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Master.VendorMonitoringAnswerType = VendorMonitoringAnswerTypes.Actual;
        }
    }
}