using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tricension.Data.CRM4
{
    public class Globals
    {
        private static CrmServiceSettings _crmServiceSettings = null;

        public static CrmServiceSettings CrmServiceSettings
        {
            get
            {
                if (_crmServiceSettings == null)
                    throw new ApplicationException("Settings used to access the CRM services have not been initialized. Please call the 'Globals.Initialize()' method prior to using other methods within this library.");
                return _crmServiceSettings;
            }
            set
            {
                if (value == null)
                    throw new ApplicationException("A valid instance of CrmServiceSettings must be provided to this property.");
                _crmServiceSettings = value;
            }
        }

        public static void Initialize(CrmServiceSettings crmServiceSettings)
        {
            if (crmServiceSettings == null)
                throw new ApplicationException("Parameter 'crmServiceSettings' cannot be null.");
            CrmServiceSettings = crmServiceSettings;
        }

        internal static String UnwrapSecureString(System.Security.SecureString ss)
        {
            IntPtr i = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(ss);
            try
            {
                return System.Runtime.InteropServices.Marshal.PtrToStringBSTR(i);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(i);
            }
        }
    }

    public class CrmServiceSettings
    {
        public String OrganizationName { get; set; }
        public Int32 DeploymentType { get; set; }
        public String DomainName { get; set; }
        public System.Security.SecureString Password { get; set; }
        public String Username { get; set; }
        public String ServiceUrl { get; set; }
        public String MetadataServiceUrl { get; set; }
    }
}
