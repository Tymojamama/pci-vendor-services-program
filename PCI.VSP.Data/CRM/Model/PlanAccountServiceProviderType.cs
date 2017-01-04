using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using PCI.VSP.Data.Classes;
using PCI.VSP.Data.Enums;

namespace PCI.VSP.Data.CRM.Model
{
    public class PlanAccountServiceProviderType : EntityBase
    {
        public PlanAccountServiceProviderType() : base(DataConstants.vsp_planaccountserviceprovidertype) { }
        public PlanAccountServiceProviderType(DynamicEntity e) : base(e) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>(DataConstants.vsp_planaccountserviceprovidertypeid, PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>(DataConstants.vsp_planaccountserviceprovidertypeid, PropertyType.Key, value); }
        }

        public string Name
        {
            get { return base.GetPropertyValue<string>(DataConstants.vsp_name, PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<string>(DataConstants.vsp_name, PropertyType.String, value); }
        }

        public Guid CategoryId
        {
            get { return base.GetPropertyValue<Guid>(DataConstants.vsp_questioncategoryid, PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>(DataConstants.vsp_questioncategoryid, PropertyType.Lookup, value); }
        }

        public Guid FunctionId
        {
            get { return base.GetPropertyValue<Guid>(DataConstants.vsp_questionfunctionid, PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>(DataConstants.vsp_questionfunctionid, PropertyType.Lookup, value); }
        }

        public TemplateType TemplateType
        {
            get { return base.GetPropertyValue<TemplateType>("vsp_templatetype", PropertyType.Picklist, TemplateType.GenericQuestionTemplate); }
            set { base.SetPropertyValue<TemplateType>("vsp_templatetype", PropertyType.Picklist, value); }
        }
    }
}
