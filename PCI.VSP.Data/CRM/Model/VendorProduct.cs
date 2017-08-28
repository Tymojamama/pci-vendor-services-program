using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Data.CRM.Model
{
    public class VendorProduct : EntityBase
    {
        public VendorProduct() : base("vsp_vendorproduct") { }
        public VendorProduct(DynamicEntity e)
            : base(e) 
        {
        }

        public Guid VendorProductId
        {
            get { return base.GetPropertyValue<Guid>("vsp_vendorproductid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_vendorproductid", PropertyType.Key, value); }
        }

        public string VendorProductName
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

        public DateTime? LastUpdated
        {
            get 
            {
                String crmDateString = base.GetPropertyValue<String>("vsp_lastupdated", PropertyType.String, String.Empty);
                return base.StringDateToNullable(crmDateString);
            }
            set 
            {
                String crmDateString = String.Empty;
                if (value.HasValue) { crmDateString = value.Value.ToShortDateString(); }
                base.SetPropertyValue<String>("vsp_lastupdated", PropertyType.String, crmDateString); 
            }
        }

        public String LastUpdatedBy
        {
            get { return base.GetPropertyValue<String>("vsp_lastupdatedby", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_lastupdatedby", PropertyType.String, value); }
        }

        public Int32 LegacyProductID
        {
            get { return base.GetPropertyValue<Int32>("new_legacyproductid", PropertyType.Number, 0); }
            set { base.SetPropertyValue<Int32>("new_legacyproductid", PropertyType.Number, value); }
        }
    }
}
