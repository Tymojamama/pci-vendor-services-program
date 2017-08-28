using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using PCI.VSP.Data.Classes;

namespace PCI.VSP.Data.CRM.Model
{
    public class PlanAccount : EntityBase
    {
        public PlanAccount() : base(DataConstants.new_plan) { }
        public PlanAccount(DynamicEntity e) : base(e) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>(DataConstants.new_planid, PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>(DataConstants.new_planid, PropertyType.Key, value); }
        }

        public Guid ClientId
        {
            get { return base.GetPropertyValue<Guid>(DataConstants.new_planaccountid, PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>(DataConstants.new_planaccountid, PropertyType.Lookup, value); }
        }

        public String PlanName
        {
            get { return base.GetPropertyValue<String>(DataConstants.new_planname, PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>(DataConstants.new_planname, PropertyType.String, value); }
        }
    }
}
