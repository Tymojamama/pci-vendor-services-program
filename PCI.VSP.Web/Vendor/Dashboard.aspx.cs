using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Web.classes;
using PCI.VSP.Web.Security;
using PCI.VSP.Services.Model;
using PCI.VSP.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace PCI.VSP.Web.Vendor
{
    public partial class Dashboard : System.Web.UI.Page
    {
        Utility sutility = new Utility();
        IUser user;

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
            user = sutility.GetUser();
            lblPercentProfileComplete.Text = new VendorQuestionDataLogic(sutility.GetAuthRequest()).RetrieveVendorProfilePercentComplete(user.AccountId).ToString("P");

            if (!IsPostBack)
            {
                RefreshVendorProducts();
                RefreshClientInquiries();
            }
            BindVendorProducts();
            BindClientInquiries();
        }

        private void RefreshVendorProducts()
        {
            Services.Model.IUser user = new Security.Utility().GetUser();
            List<Data.Classes.VendorProductSummary> vendorProductSummaries = new VendorQuestionDataLogic(GetDefaultAuthRequest()).GetVendorProductDashboard(user.AccountId, user.Id);
            Session[Constants.VendorProductDashboard] = vendorProductSummaries;
        }

        private void BindVendorProducts()
        {
            List<Data.Classes.VendorProductSummary> vendorProductSummaries = (List<Data.Classes.VendorProductSummary>)Session[Constants.VendorProductDashboard];
            VendorProductGridView.DataSource = vendorProductSummaries;
            VendorProductGridView.DataBind();
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
