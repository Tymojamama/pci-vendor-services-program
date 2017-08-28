using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Diagnostics;

// Microsoft Dynamics CRM namespaces
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.SdkTypeProxy.Metadata;

namespace PCI.VSP.Plugins
{
    public class HashEncoderPlugin : IPlugin
    {
        // Configuration information that can be passed to a plug-in at run-time.
        private string _secureInformation;
        private string _unsecureInformation;

        // Note: Due to caching, Microsoft Dynamics CRM does not invoke the plug-in 
        // contructor every time the plug-in is executed.

        // Related SDK topic: Writing the Plug-in Constructor
        public HashEncoderPlugin(string unsecureInfo, string secureInfo)
        {
            _secureInformation = secureInfo;
            _unsecureInformation = unsecureInfo;
        }

        // Related SDK topic: Writing a Plug-in
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                DynamicEntity entity = null;

#if DEBUG
                TraceListener tl = new TextWriterTraceListener(System.IO.File.CreateText(@"C:\Temp\PCI.VSP.Plugins.PasswordHash.log"));
                Trace.Listeners.Add(tl);    
#endif

                Trace.WriteLine("context.InputParameters.Properties:");
                foreach (PropertyBagEntry pbe in context.InputParameters.Properties)
                    Trace.WriteLine("Name: " + pbe.Name + "; Value: " + pbe.Value.ToString());
                Trace.WriteLine(String.Empty);
                Trace.Flush();

                // Check if the InputParameters property bag contains a target of the current operation and that target is of type DynamicEntity.
                if (context.InputParameters.Properties.Contains(ParameterName.Target) && context.InputParameters.Properties[ParameterName.Target] is DynamicEntity)
                {
                    // Obtain the target business entity from the input parmameters.
                    entity = (DynamicEntity)context.InputParameters.Properties[ParameterName.Target];

                    // Test for an entity type and message supported by the plug-in.
                    Trace.WriteLine("Message name: " + context.MessageName);
                    Trace.WriteLine("Entity name: " + entity.Name);
                    Trace.WriteLine("Entity properties:");
                    foreach (Property p in entity.Properties)
                        Trace.WriteLine(p.Name);
                    Trace.WriteLine(String.Empty);
                    Trace.Flush();

                    if (entity.Name != EntityName.contact.ToString()) { return; }
                    switch (context.MessageName)
                    {
                        case MessageName.Create:
                        case MessageName.Update:
                            break;
                        default:
                            return;
                    }
                }
                else
                {
                    return;
                }

                DynamicEntity image = null;
                if (context.PreEntityImages.Contains("ContactPasswordUpdate"))
                    image = (DynamicEntity)context.PreEntityImages["ContactPasswordUpdate"];

                if (image != null)
                {
                    Trace.WriteLine("Image name: " + image.Name);
                    Trace.WriteLine("Image properties:");

                    foreach (Property p in image.Properties)
                        Trace.WriteLine(p.Name);
                    Trace.WriteLine(String.Empty);
                    Trace.Flush();
                }

                DynamicEntity c = new DynamicEntity(EntityName.contact.ToString());
                String username = null;

                if (image != null && image.Properties.Contains("vsp_username") && image.Properties["vsp_username"] != null && !String.IsNullOrEmpty(image.Properties["vsp_username"].ToString()))
                {
                    Trace.WriteLine("Image != null");
                    if (!image.Properties.Contains("vsp_username") || image.Properties["vsp_username"] == null || String.IsNullOrEmpty(image.Properties["vsp_username"].ToString()))
                        return;
                    username = image.Properties["vsp_username"].ToString();
                }
                else
                {
                    Trace.WriteLine("Image == null OR vsp_username does not exist in the image");
                    if (!entity.Properties.Contains("vsp_username") || entity.Properties["vsp_username"] == null || String.IsNullOrEmpty(entity.Properties["vsp_username"].ToString()))
                        return;
                    username = entity.Properties["vsp_username"].ToString();
                }
                Trace.WriteLine("Username: " + username);

                if (entity.Properties.Contains("vsp_password") && !String.IsNullOrEmpty(entity.Properties["vsp_password"].ToString()))
                {
                    String password = entity.Properties["vsp_password"].ToString();
                    password = EncodeString(password, GenerateSalt(username));
                    entity.Properties["vsp_password"] = password;
                    Trace.WriteLine("Password successfully hashed.");
                }

                if (entity.Properties.Contains("vsp_securityanswer") && !String.IsNullOrEmpty(entity.Properties["vsp_securityanswer"].ToString()))
                {
                    String securityAnswer = entity.Properties["vsp_securityanswer"].ToString();
                    securityAnswer = EncodeString(securityAnswer, GenerateSalt(username));
                    entity.Properties["vsp_securityanswer"] = securityAnswer;
                    Trace.WriteLine("Security answer successfully hashed.");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception Message: " + ex.Message);
                Trace.WriteLine("Exception Stack Trace: " + ex.StackTrace);
                throw new InvalidPluginExecutionException(String.Format("An error occurred in the {0} plug-in.", this.GetType().ToString()), ex);
            }
            finally
            {
                Trace.Close();
            }
        }

        #region Private methods
        /// <summary>
        /// Creates a CrmService proxy for plug-ins that execute in the child pipeline.
        /// </summary>
        /// <param name="context">The execution context that was passed to the plug-ins Execute method.</param>
        /// <param name="flag">Set to True to use impersontation.</param>
        /// <returns>A CrmServce instance.</returns>
        //private CrmService CreateCrmService(IPluginExecutionContext context, Boolean flag)
        //{
        //    CrmAuthenticationToken authToken = new CrmAuthenticationToken();
        //    authToken.AuthenticationType = 0;
        //    authToken.OrganizationName = context.OrganizationName;

        //    // Include support for impersonation.
        //    if (flag)
        //        authToken.CallerId = context.UserId;
        //    else
        //        authToken.CallerId = context.InitiatingUserId;

        //    CrmService service = new CrmService();
        //    service.CrmAuthenticationTokenValue = authToken;
        //    service.UseDefaultCredentials = true;

        //    // Include support for infinite loop detection.
        //    CorrelationToken corToken = new CorrelationToken();
        //    corToken.CorrelationId = context.CorrelationId;
        //    corToken.CorrelationUpdatedTime = context.CorrelationUpdatedTime;
        //    corToken.Depth = context.Depth;

        //    RegistryKey regkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MSCRM");

        //    service.Url = String.Concat(regkey.GetValue("ServerUrl").ToString(), "/2007/crmservice.asmx");
        //    service.CorrelationTokenValue = corToken;

        //    return service;
        //}

        /// <summary>
        /// Creates a MetadataService proxy for plug-ins that execute in the child pipeline.
        /// </summary>
        /// <param name="context">The execution context that was passed to the plug-ins Execute method.</param>
        /// <param name="flag">Set to True to use impersontation.</param>
        /// <returns>A MetadataServce instance.</returns>
        //private MetadataService CreateMetadataService(IPluginExecutionContext context, Boolean flag)
        //{
        //    CrmAuthenticationToken authToken = new CrmAuthenticationToken();
        //    authToken.AuthenticationType = 0;
        //    authToken.OrganizationName = context.OrganizationName;

        //    // Include support for impersonation.
        //    if (flag)
        //        authToken.CallerId = context.UserId;
        //    else
        //        authToken.CallerId = context.InitiatingUserId;

        //    MetadataService service = new MetadataService();
        //    service.CrmAuthenticationTokenValue = authToken;
        //    service.UseDefaultCredentials = true;

        //    RegistryKey regkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MSCRM");

        //    service.Url = String.Concat(regkey.GetValue("ServerUrl").ToString(), "/2007/metadataservice.asmx");

        //    return service;
        //}

        private string EncodeString(string pass, Byte[] salt)
        {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(pass);
            byte[] dst = new byte[salt.Length + bytes.Length];
            Buffer.BlockCopy(salt, 0, dst, 0, salt.Length);
            Buffer.BlockCopy(bytes, 0, dst, salt.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA256");
            byte[] inArray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inArray);
        }

        private Byte[] GenerateSalt(string foundation)
        {
            Char[] inverse = foundation.ToCharArray();
            Array.Reverse(inverse);
            return System.Text.Encoding.ASCII.GetBytes("B4MEF!8Hbpa6@#" + new string(inverse) + "B4yhN$%HUY^1sd");
        }
        #endregion Private Methods
    }
}
