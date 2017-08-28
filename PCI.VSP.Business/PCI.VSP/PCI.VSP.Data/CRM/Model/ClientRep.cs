using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Data.CRM.Model
{
    public class ClientRep : Contact
    {
        public ClientRep() : base() { }
        public ClientRep(Microsoft.Crm.Sdk.DynamicEntity e) : base(e) { }
    }
}
