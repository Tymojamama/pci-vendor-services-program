using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Web.classes;
using PCI.VSP.Services;

namespace PCI.VSP.Web.Vendor
{
    public partial class Products : System.Web.UI.Page
    {
        private AuthenticationRequest GetDefaultAuthRequest()
        {
            return new AuthenticationRequest()
            {
                Username = Data.Globals.CrmServiceSettings.Username,
                Password = Data.Globals.CrmServiceSettings.Password
            };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Services.Model.IUser user = new Security.Utility().GetUser();
            Repeater1.DataSource = new VendorQuestionDataLogic(GetDefaultAuthRequest()).GetVendorProductDashboard(user.AccountId, user.Id);
            Repeater1.DataBind();
        }
    }
}