using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PCI.VSP.Web.CrmIFrames
{
    public partial class FilterPhase2 : System.Web.UI.Page
    {
        private Guid ClientProjectId
        {
            get
            {
                if (!Request.QueryString.AllKeys.Contains("id")) { return Guid.Empty; }
                return Guid.Parse(Request.QueryString["id"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void PerformFilterButton_Click(object sender, EventArgs e)
        {
            Guid clientProjectId = this.ClientProjectId;
            //clientProjectId = Guid.Parse("C14421D7-CB38-E211-AF41-00155D016411");
            if (clientProjectId == Guid.Empty) { Response.Write("Invalid clientProjectId " + clientProjectId.ToString()); return; }

            Services.VspService service = new Services.VspService();
            FilterResultsTextBox.Text = service.PerformPhase2Filter(clientProjectId);
            FilterResultsTextBox.Visible = true;
        }

    }
}