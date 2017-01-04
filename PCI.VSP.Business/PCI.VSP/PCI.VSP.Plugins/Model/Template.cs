using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Plugins.Model
{
    public class Template : EntityBase
    {
        public Template() : base("template") { }
        public Template(DynamicEntity de) : base(de) { }
        public List<Question> Questions { get; set; }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_templateid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_templateid", PropertyType.Key, value); }
        }

        public new String Name
        {
            get { return base.GetPropertyValue<String>("vsp_name", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_name", PropertyType.String, value); }
        }

        public Guid ProductId
        {
            get { return base.GetPropertyValue<Guid>("vsp_vspproductid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_vspproductid", PropertyType.Lookup, value); }
        }
        
        public Enums.FilterCategory FilterCategory
        {
            get { 
                bool fcb = base.GetPropertyValue<Boolean>("vsp_filtercategory", PropertyType.Bit, false);
                if (fcb)
                    return Enums.FilterCategory.Filter2;
                else
                    return Enums.FilterCategory.Filter2;
            }
            set {
                bool fcb = (value == Enums.FilterCategory.Filter2);
                base.SetPropertyValue<Boolean>("vsp_filtercategory", PropertyType.Bit, fcb);
            }
        }
        
    }
}
