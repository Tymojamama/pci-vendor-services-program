using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using PCI.VSP.Data.Classes;

namespace PCI.VSP.Data.CRM.Model
{
    public class PlanAccountServiceProvider : EntityBase
    {
        public PlanAccountServiceProvider() : base(DataConstants.vsp_planaccountserviceprovider) { }
        public PlanAccountServiceProvider(DynamicEntity e) : base(e) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>(DataConstants.vsp_planaccountserviceproviderid, PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>(DataConstants.vsp_planaccountserviceproviderid, PropertyType.Key, value); }
        }

        public Guid VendorAccountId
        {
            get { return base.GetPropertyValue<Guid>(DataConstants.vsp_accountid, PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>(DataConstants.vsp_accountid, PropertyType.Lookup, value); }
        }

        public Guid PlanAccountId
        {
            get { return base.GetPropertyValue<Guid>(DataConstants.vsp_planaccountid, PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>(DataConstants.vsp_planaccountid, PropertyType.Lookup, value); }
        }

        public Guid ServiceProviderTypeId
        {
            get { return base.GetPropertyValue<Guid>(DataConstants.vsp_planaccountserviceprovidertypeid, PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>(DataConstants.vsp_planaccountserviceprovidertypeid, PropertyType.Lookup, value); }
        }

        public DateTime? StartDate
        {
            get { return base.GetPropertyValue<DateTime?>(DataConstants.vsp_startdate, PropertyType.DateTime, null); }
            set { base.SetPropertyValue<DateTime?>(DataConstants.vsp_startdate, PropertyType.DateTime, value); }
        }

        public DateTime? EndDate
        {
            get { return base.GetPropertyValue<DateTime?>(DataConstants.vsp_enddate, PropertyType.DateTime, null); }
            set { base.SetPropertyValue<DateTime?>(DataConstants.vsp_enddate, PropertyType.DateTime, value); }
        }
    }
}
