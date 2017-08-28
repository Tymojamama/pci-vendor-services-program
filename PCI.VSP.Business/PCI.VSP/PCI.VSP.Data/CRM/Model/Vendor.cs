using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Data.CRM.Model
{
    public class Vendor : Account
    {
        public Vendor() : base() { }
        public Vendor(Microsoft.Crm.Sdk.DynamicEntity e) : base(e) { }
    }
}
