using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using PCI.VSP.Data.Enums;

namespace PCI.VSP.Data.CRM.Model
{
    public class Template : EntityBase
    {
        public Template() : base("template") { }
        public Template(DynamicEntity de) : base(de) { }
        public List<TemplateQuestion> Questions { get; set; }

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

        public TemplateType TemplateType
        {
            get { return base.GetPropertyValue<TemplateType>("vsp_templatetype", PropertyType.Picklist, TemplateType.GenericQuestionTemplate); }
            set { base.SetPropertyValue<TemplateType>("vsp_templatetype", PropertyType.Picklist, value); }
        }
    }
}
