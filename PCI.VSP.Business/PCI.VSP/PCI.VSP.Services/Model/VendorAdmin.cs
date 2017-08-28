using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services.Model
{
    public class VendorAdmin : PCI.VSP.Data.CRM.Model.VendorAgent, IUser
    {
        internal VendorAdmin(PCI.VSP.Data.CRM.Model.VendorAgent v)
            : base(v)
        {
        }

        public VendorAdmin()
            : base()
        {
        }
    }
}
