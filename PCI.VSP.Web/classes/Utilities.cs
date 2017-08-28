using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCI.VSP.Web.classes
{
    public static class Utilities
    {
        /// <summary>
        /// REMOVE THIS METHOD ONCE USER CREDENTIALS ARE STORED
        /// </summary>
        /// <returns></returns>
        public static PCI.VSP.Services.AuthenticationRequest FakeAuth()
        {
            InitVspService();

            System.Security.SecureString ss = new System.Security.SecureString();
            foreach (Char c in "whatever".ToCharArray())
            {
                ss.AppendChar(c);
            }
            ss.MakeReadOnly();

            PCI.VSP.Services.VspService vspService = new PCI.VSP.Services.VspService();
            PCI.VSP.Services.AuthenticationRequest authRequest = new PCI.VSP.Services.AuthenticationRequest()
            {
                DomainName = String.Empty,
                Password = ss,
                Username = "rsmartin"
            };

            return authRequest;
        }

        private static void InitVspService()
        {
            System.Security.SecureString ss = new System.Security.SecureString();
            foreach (Char c in "P@ssword1".ToCharArray())
            {
                ss.AppendChar(c);
            }
            ss.MakeReadOnly();

            PCI.VSP.Services.VspService vspService = new PCI.VSP.Services.VspService();
            PCI.VSP.Data.CrmServiceSettings settings = new Data.CrmServiceSettings()
            {
                DeploymentType = 2,
                DomainName = "TRICENSION",
                OrganizationName = "PensionConsultingInc",
                Password = ss,
                ServiceUrl = "http://pensionconsultinginc.tricension.net/MSCRMServices/2007/SPLA/CrmDiscoveryService.asmx",
                Username = "house.account"
            };

            vspService.Initialize(settings);
        }
    }
}