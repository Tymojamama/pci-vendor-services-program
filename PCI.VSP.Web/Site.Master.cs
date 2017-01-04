using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;

namespace PCI.VSP.Web
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PDFFilePath"]) && HeadLoginView.FindControl("lbHelpPDF") != null && File.Exists(ConfigurationManager.AppSettings["PDFFilePath"]))
                //    HeadLoginView.FindControl("lbHelpPDF").Visible = true;
            }
            catch { }
        }

        protected void lbHelpPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PDFFilePath"]) && File.Exists(ConfigurationManager.AppSettings["PDFFilePath"]))
                {
                    Response.Clear();
                    Response.ContentType = "application/octet-stream";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + ConfigurationManager.AppSettings["PDFFilePath"]);
                    Response.WriteFile(ConfigurationManager.AppSettings["PDFFilePath"]);
                    Response.End();
                }
            }
            catch { }
        }

        protected void HeadLoginStatus_LoggedOut(object sender, EventArgs e)
        {
            Session.Abandon();
        }
    }
}
