using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Data.CRM.Model
{
    public class VendorAgent : Contact
    {
        public VendorAgent() : base() { }
        public VendorAgent(Microsoft.Crm.Sdk.DynamicEntity e) : base(e) { }
    }
}
