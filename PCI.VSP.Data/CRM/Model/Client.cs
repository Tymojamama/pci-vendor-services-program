using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Data.CRM.Model
{
    public class Client : Account
    {
        public Client() : base() { }
        public Client(Microsoft.Crm.Sdk.DynamicEntity e) : base(e) { }
    }
}
