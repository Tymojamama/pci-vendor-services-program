using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace PCI.VSP.Web
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            System.Security.SecureString ss = new System.Security.SecureString();
            foreach (Char c in Properties.Settings.Default.CRMPassword.ToCharArray())
            {
                ss.AppendChar(c);
            }
            ss.MakeReadOnly();

            Data.CrmServiceSettings crmServiceSettings = new Data.CrmServiceSettings()
            {
                DeploymentType = Properties.Settings.Default.CRMDeploymentType,
                DomainName = Properties.Settings.Default.CRMDomain,
                OrganizationName = Properties.Settings.Default.CRMOrganization,
                Password = ss,
                ServiceUrl = Properties.Settings.Default.CRMUrl,
                Username = Properties.Settings.Default.CRMUsername
            };

            PCI.VSP.Services.VspService vspService = new Services.VspService();
            vspService.Initialize(crmServiceSettings);
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
        }

        void Application_Error(object sender, EventArgs e)
        {
            Response.Redirect("");
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

            // check user is locked
            CheckUserLocked();
        }

        private void CheckUserLocked()
        {
            if (User == null || User.Identity == null || String.IsNullOrWhiteSpace(User.Identity.Name) ||
                !User.Identity.IsAuthenticated)
                return;
            
            Services.VspService service = new Services.VspService();
            Services.Model.IUser user = service.GetUser(User.Identity.Name);
            if (user.IsLocked)
            {
                System.Web.Security.FormsAuthentication.SignOut();
                System.Web.Security.FormsAuthentication.RedirectToLoginPage();
            }
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
