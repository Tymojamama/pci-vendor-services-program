using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services.Model
{
    public class ClientRep : PCI.VSP.Data.CRM.Model.ClientRep, IUser
    {
        internal ClientRep(PCI.VSP.Data.CRM.Model.ClientRep c)
            : base(c)
        {
        }
        public String CrmTicket { get; set; }
    }
}
