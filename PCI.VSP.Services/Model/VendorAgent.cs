using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services.Model
{
    public class VendorAgent : PCI.VSP.Data.CRM.Model.VendorAgent, IUser
    {
        internal VendorAgent(PCI.VSP.Data.CRM.Model.VendorAgent v)
            : base(v)
        {
        }

        public VendorAgent()
            : base()
        {
        }
    }
}
