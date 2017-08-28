using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PCI.VSP.Web
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Services.Model.IUser user = new Security.Utility().GetUser();
            System.Type userType = user.GetType();

            if (userType == typeof(Services.Model.VendorAgent) ||
                userType == typeof(Services.Model.VendorAdmin))
                Response.Redirect("~/Vendor/Dashboard.aspx");
        }
    }
}
