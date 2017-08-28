using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tricension.Data.CRM4.Model
{
    public interface IAuthenticationRequest
    {
        String Username { get; set; }
        System.Security.SecureString Password { get; set; }
        String DomainName { get; set; }
        String CrmTicket { get; set; }
        String OrganizationName { get; set; }
        Boolean WasRefreshed { get; set; }
    }

}
