using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.SdkTypeProxy.Metadata;

namespace Tricension.Data.CRM4.DataLogic
{
    public class MetaDataServiceBroker
    {

        public MetadataService GetServiceInstance()
        {
            Model.IAuthenticationRequest tokenRequest = GetDefaultTicketRequest();
            TokenBroker.Token token = new TokenBroker().RequestToken(tokenRequest);

            MetadataService metaService = new MetadataService();
            metaService.CrmAuthenticationTokenValue = token.CrmToken;
            metaService.UnsafeAuthenticatedConnectionSharing = true;
            metaService.Url = Globals.CrmServiceSettings.MetadataServiceUrl;
 
            return metaService;
        }

        private Model.IAuthenticationRequest GetDefaultTicketRequest()
        {
            return new TokenRequest()
            {
                DomainName = Globals.CrmServiceSettings.DomainName,
                Password = Globals.CrmServiceSettings.Password,
                Username = Globals.CrmServiceSettings.Username,
                OrganizationName = Globals.CrmServiceSettings.OrganizationName
            };
        }

        private class TokenRequest : Model.IAuthenticationRequest
        {
            public string Username { get; set; }
            public System.Security.SecureString Password { get; set; }
            public string DomainName { get; set; }
            public bool WasRefreshed { get; set; }
            public string CrmTicket { get; set; }
            public string OrganizationName { get; set; }
        }
    }
}
