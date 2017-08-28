using System;

namespace PCI.VSP.Test
{
    internal class Globals
    {
        internal static void InitVspService()
        {
            PCI.VSP.Services.VspService vspService = new PCI.VSP.Services.VspService();
            PCI.VSP.Data.CrmServiceSettings settings = new Data.CrmServiceSettings()
            {
                DeploymentType = 2,
                DomainName = "TRICENSION",
                OrganizationName = "PensionConsultingInc",
                Password = MakeSecureString("P@ssword1"),
                ServiceUrl = "http://pensionconsultinginc.tricension.net/MSCRMServices/2007/SPLA/CrmDiscoveryService.asmx",
                Username = "house.account"
            };

            vspService.Initialize(settings);
        }

        internal static System.Security.SecureString MakeSecureString(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) { return null; }
            System.Security.SecureString secureString = new System.Security.SecureString();
            foreach (Char c in value.Trim().ToCharArray())
                secureString.AppendChar(c);
            secureString.MakeReadOnly();
            return secureString;
        }

        internal static PCI.VSP.Services.AuthenticationRequest GetGenericAuthRequest()
        {
            PCI.VSP.Services.AuthenticationRequest authRequest = new PCI.VSP.Services.AuthenticationRequest()
            {
                DomainName = String.Empty,
                Password = Globals.MakeSecureString("whatever"),
                Username = "rsmartin"
            };
            return authRequest;
        }
    }
}

