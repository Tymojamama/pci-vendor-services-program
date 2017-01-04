using PCI.VSP.Data.CRM.Model;
using PCI.VSP.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PCI.VSP.Web.CrmIFrames
{
    public partial class Filter : System.Web.UI.Page
    {
        private Guid _clientProjectId
        {
            get
            {
                Guid clientProjectId;

                if (!Request.QueryString.AllKeys.Contains("id"))
                {
                    clientProjectId = Guid.Empty;
                }
                else
                {
                    try
                    {
                        clientProjectId = Guid.Parse(Request.QueryString["id"]);
                    }
                    catch
                    {
                        clientProjectId = Guid.Empty;
                    }
                }

                return clientProjectId;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                filterFormError.Visible = false;

                if (_clientProjectId == Guid.Empty)
                {
                    filterForm.Visible = false;
                    filterFormError.Visible = true;
                    return;
                }

                VspService service = new VspService();

                List<ProjectVendor> projectVendors = service.GetProjectVendorsByClientProject(_clientProjectId);
                if (projectVendors != null && projectVendors.Count > 0)
                {
                    Filter1RunHiddenField.Value = "true";
                }
            }

        }

        protected void PerformFilterButton_Click(object sender, EventArgs e)
        {
            Guid clientProjectId = this._clientProjectId;
            if (clientProjectId == Guid.Empty) { Response.Write("Invalid clientProjectId " + clientProjectId.ToString()); return; }
            
            VspService service = new Services.VspService();
            FilterResultsTextBox.Text = service.PerformPhase1Filter(clientProjectId);
            FilterResultsTextBox.Visible = true;
        }
        
    }
}