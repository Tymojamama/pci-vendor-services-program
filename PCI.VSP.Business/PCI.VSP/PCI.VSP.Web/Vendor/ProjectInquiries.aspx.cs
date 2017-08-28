using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PCI.VSP.Web.Vendor
{
    public partial class ProjectInquiries : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RefreshClientInquiries();
            BindClientInquiries();
        }

        private void RefreshClientInquiries()
        {
            Services.VspService service = new Services.VspService();
            Services.Model.IUser user = new Security.Utility().GetUser();
            List<Services.Model.VendorProjectInquirySummary> vcisl = service.RetrieveVendorProjectInquirySummary(user.AccountId, user.Id);
            Session[Constants.VendorClientInquirySummary] = vcisl;
        }

        private void BindClientInquiries()
        {
            List<Services.Model.VendorProjectInquirySummary> vcisl = (List<Services.Model.VendorProjectInquirySummary>)Session[Constants.VendorClientInquirySummary];
            ClientInquiriesGridView.DataSource = vcisl;
            ClientInquiriesGridView.DataBind();
        }

    }
}