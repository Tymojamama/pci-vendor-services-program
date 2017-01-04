using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Workflows
{
    class VendorProduct : EntityBase
    {
        internal const String _entityName = "vsp_vendorproduct";

        internal VendorProduct() : base(_entityName) { }
        internal VendorProduct(DynamicEntity e) : base(e) { }

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
        
        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_vendorproductid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_vendorproductid", PropertyType.Key, value); }
        }

        public new string Name
        {
            get { return base.GetPropertyValue<string>("vsp_name", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_name", PropertyType.String, value); }
        }

        public Guid VendorId
        {
            get { return base.GetPropertyValue<Guid>("vsp_accountid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_accountid", PropertyType.Lookup, value); }
        }

        public Guid ProductId
        {
            get { return base.GetPropertyValue<Guid>("vsp_productid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_productid", PropertyType.Lookup, value); }
        }

    }
}
