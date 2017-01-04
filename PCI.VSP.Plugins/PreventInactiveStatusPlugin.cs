using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using System.Diagnostics;

namespace PCI.VSP.Plugins
{
    public class PreventInactiveStatusPlugin : IPlugin
    {
        private const string _status = "Status";
        private const string _inactiveStatusValue = "2";

        public void Execute(IPluginExecutionContext context)
        {
            try
            {
#if DEBUG
                TraceListener tl = new TextWriterTraceListener(System.IO.File.CreateText(@"C:\Temp\PCI.VSP.Plugins.PreventInactiveStatusPlugin.log"));
                Trace.Listeners.Add(tl);
#endif

                Trace.WriteLine("context.InputParameters.Properties:");
                foreach (PropertyBagEntry pbe in context.InputParameters.Properties)
                    Trace.WriteLine("Name: " + pbe.Name + "; Value: " + pbe.Value.ToString());
                Trace.WriteLine(String.Empty);
                
                if (context.InputParameters.Properties[_status].ToString() == _inactiveStatusValue)
                    throw new Exception("Deactivation Not Allowed");
            }
            catch (Exception ex)
            {
                #region Exception and Inner Exception Logging

                Trace.WriteLine("EXCEPTION: " + ex.Message);
                Trace.WriteLine("Stack: " + ex.StackTrace);

                if (ex.GetType() == typeof(System.Web.Services.Protocols.SoapException))
                {
                    Trace.WriteLine("Detail: ");
                    Trace.WriteLine(((System.Web.Services.Protocols.SoapException)ex).Detail.InnerText);
                }

                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    Trace.WriteLine(" ");
                    Trace.WriteLine("INNER EXCEPTION: " + innerException.Message);
                    Trace.WriteLine("Stack: " + innerException.StackTrace);
                    innerException = innerException.InnerException;
                }

                #endregion
                throw;
            }
            finally
            {
                Trace.Flush();
                Trace.Close();
            }
        }
    }
}
