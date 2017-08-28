using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Workflows
{
    class VendorProfile : EntityBase
    {
        internal const String _entityName = "account";

        internal VendorProfile() : base(_entityName) { }
        internal VendorProfile(DynamicEntity e) : base(e) { }
        
        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("accountid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("accountid", PropertyType.Key, value); }
        }

        public Int32 EmailThreshold
        {
            get { return base.GetPropertyValue<Int32>("vsp_emailthreshold", PropertyType.Number, 0); }
            set { base.SetPropertyValue<Int32>("vsp_emailthreshold", PropertyType.Number, value); }
        }

        public Boolean IsAboveThreshold
        {
            get { return base.GetPropertyValue<Boolean>("vsp_abovethreshold", PropertyType.Number, false); }
            set { base.SetPropertyValue<Boolean>("vsp_abovethreshold", PropertyType.Number, value); }
        }
    }
}
