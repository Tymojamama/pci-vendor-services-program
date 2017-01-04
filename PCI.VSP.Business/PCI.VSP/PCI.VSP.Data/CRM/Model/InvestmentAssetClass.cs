using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Data.CRM.Model
{
    public class InvestmentAssetClass : EntityBase
    {
        public InvestmentAssetClass() : base("investmentassetclass") { }
        public InvestmentAssetClass(DynamicEntity e) : base(e) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_investmentassetclassid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_investmentassetclassid", PropertyType.Key, value); }
        }

        public string Name
        {
            get { return base.GetPropertyValue<String>("vsp_name", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_name", PropertyType.String, value); }
        }
    }
}
