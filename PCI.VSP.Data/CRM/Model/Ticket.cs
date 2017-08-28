using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Data.CRM.Model
{
    public class Ticket
    {
        public String CrmTicket { get; set; } // TODO: 'crypt this dude
        public DateTime ExpirationDate { get; set; }
        public String OrganizationName { get; set; }
        public String ServiceUrl { get; set; }
    }

    public interface IAuthenticationRequest
    {
        String Username { get; set; }
        String Password { get; set; }
        String DomainName { get; set; }
    }
}
