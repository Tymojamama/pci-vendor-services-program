using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Services;
using PCI.VSP.Data.CRM.Model;
using System.IO;

namespace PCI.VSP.Web.Vendor
{
    /// <summary>
    /// Summary description for DownloadNote
    /// </summary>
    public class DownloadNote : IHttpHandler
    {
        #region Private Properties

        /// <summary>
        /// CRM User Credentials so I don't have to keep hardcoding it everywhere
        /// </summary>
        private AuthenticationRequest _authRequest = new AuthenticationRequest() { Username = Data.Globals.CrmServiceSettings.Username, Password = Data.Globals.CrmServiceSettings.Password };

        #endregion

        public void ProcessRequest(HttpContext context)
        {
            Guid id;

            if (!string.IsNullOrEmpty(context.Request.QueryString["id"]) && Guid.TryParse(context.Request.QueryString["id"], out id))
            {
                Annotation note = new AnnotationDataLogic(_authRequest).Retrieve(id, new string[] { "filename", "documentbody" });
                if (note != null && !string.IsNullOrEmpty(note.DocumentBody))
                {
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.AddHeader("content-disposition", "attachment;filename=" + note.FileName);
                    var byteContent = Convert.FromBase64String(note.DocumentBody);
                    context.Response.OutputStream.Write(byteContent, 0, byteContent.Length);
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}