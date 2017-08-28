using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PCI.VSP.Data.CRM.Model
{
    public class Product : EntityBase
    {
        public Product() : base("vsp_product") { }

        public new String Name
        {
            get { return base.GetPropertyValue<String>("vsp_name", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_name", PropertyType.String, value);  }
        }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_productid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_productid", PropertyType.Key, value); }
        }
        
    }
}
