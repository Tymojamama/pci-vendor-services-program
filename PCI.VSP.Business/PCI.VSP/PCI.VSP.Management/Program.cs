using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Net.Mail;
using System.Configuration;

namespace PCI.VSP.Management
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            TraceListener tl = new TextWriterTraceListener(System.IO.File.CreateText(AppDomain.CurrentDomain.BaseDirectory + "PCI_VSP_Management.log"));
            Trace.Listeners.Add(tl);

            try
            {
                Trace.WriteLine("Begin Processing");
                InitializeDataLogic();

                if (Properties.Settings.Default.CheckVendorQuestionExpiration)
                {
                    VendorQuestionModule vqm = new VendorQuestionModule();
                    vqm.CheckExpiredVendorQuestions();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Empty);
                Trace.WriteLine("Exception Message: " + ex.Message);
                Trace.WriteLine("Exception Stack Trace: " + ex.StackTrace);

                SmtpClient smtp = new SmtpClient();
                MailMessage email = new MailMessage() { Subject = "VSP Manage Console Application Error", Body = "Exception Message: " + ex.Message + "</br></br>Stack Trace:" + ex.StackTrace, Priority = MailPriority.High, IsBodyHtml = true };
                email.To.Add(ConfigurationManager.AppSettings["ErrorEmailAddress"]);
                smtp.Send(email);
            }
            finally
            {
                Trace.WriteLine("End Processing");
                Trace.Flush();
                Trace.Close();
            }
        }

        internal static void InitializeDataLogic()
        {
            Trace.WriteLine("Entering InitializeDataLogic");
            System.Security.SecureString ss = new System.Security.SecureString();
            foreach (Char c in Properties.Settings.Default.CRMPassword.ToCharArray())
            {
                ss.AppendChar(c);
            }
            ss.MakeReadOnly();
            
            Trace.WriteLine("DeploymentType: " + Properties.Settings.Default.CRMDeploymentType);
            Trace.WriteLine("DomainName: " + Properties.Settings.Default.CRMDomain);
            Trace.WriteLine("OrganizationName: " + Properties.Settings.Default.CRMOrganization);
            Trace.WriteLine("ServiceUrl: " + Properties.Settings.Default.CRMUrl);
            Trace.WriteLine("Username: " + Properties.Settings.Default.CRMUsername);
            Tricension.Data.CRM4.CrmServiceSettings crmServiceSettings = new Tricension.Data.CRM4.CrmServiceSettings()
            {
                DeploymentType = Properties.Settings.Default.CRMDeploymentType,
                DomainName = Properties.Settings.Default.CRMDomain,
                OrganizationName = Properties.Settings.Default.CRMOrganization,
                Password = ss,
                ServiceUrl = Properties.Settings.Default.CRMUrl,
                Username = Properties.Settings.Default.CRMUsername
            };

            Tricension.Data.CRM4.Globals.Initialize(crmServiceSettings);
            Trace.WriteLine("Exiting InitializeDataLogic");
        }

        internal static Tricension.Data.CRM4.Model.IAuthenticationRequest GetDefaultAuthRequest()
        {
            return new AuthenticationRequest()
            {
                Username = Tricension.Data.CRM4.Globals.CrmServiceSettings.Username,
                Password = Tricension.Data.CRM4.Globals.CrmServiceSettings.Password
            };
        }

    }

    internal class AuthenticationRequest : Tricension.Data.CRM4.Model.IAuthenticationRequest
    {
        public string Username { get; set; }
        public System.Security.SecureString Password { get; set; }
        public string DomainName { get; set; }
        public string CrmTicket { get; set; }
        public string OrganizationName { get; set; }
        public bool WasRefreshed { get; set; }
    }
}
