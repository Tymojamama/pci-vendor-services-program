using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using PCI.VSP.Data.Classes;

namespace PCI.VSP.Data.CRM.Model
{
    public class ClientProject : EntityBase
    {
        private const String _entityName = "vsp_clientproject";
        
        public ClientProject() : base(_entityName) { }
        public ClientProject(DynamicEntity e)
            : base(e)
        {
        }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_clientprojectid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_clientprojectid", PropertyType.Key, value); }
        }
        
        public Guid ClientAccountId
        {
            get { return base.GetPropertyValue<Guid>("vsp_clientaccountid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_clientaccountid", PropertyType.Lookup, value); }
        }

        public Guid PlanAccountId
        {
            get { return base.GetPropertyValue<Guid>(DataConstants.vsp_planaccountid, PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>(DataConstants.vsp_planaccountid, PropertyType.Lookup, value); }
        }
        
        public String ClientProjectName
        {
            get { return base.GetPropertyValue<String>("vsp_name", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_name", PropertyType.String, value); }
        }

        public Int32 MaxPhase1Results
        {
            get { return base.GetPropertyValue<Int32>("vsp_maxphase1results", PropertyType.Number, 0); }
            set { base.SetPropertyValue<Int32>("vsp_maxphase1results", PropertyType.Number, value); }
        }

        public Int32 MaxPhase2Results
        {
            get { return base.GetPropertyValue<Int32>("vsp_maxphase2results", PropertyType.Number, 0); }
            set { base.SetPropertyValue<Int32>("vsp_maxphase2results", PropertyType.Number, value); }
        }
        
        public Enums.VendorAssignmentTypes VendorAssignmentType
        {
            get { return base.GetPropertyValue<Enums.VendorAssignmentTypes>("vsp_vendorassignmenttype", PropertyType.Picklist, Enums.VendorAssignmentTypes.Unspecified); }
            set { base.SetPropertyValue<Enums.VendorAssignmentTypes>("vsp_vendorassignmenttype", PropertyType.Picklist, value); }
        }
        
        public String ClientComment
        {
            get { return base.GetPropertyValue<String>("vsp_clientcomment", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_clientcomment", PropertyType.String, value); }
        }

        public String PciComment
        {
            get { return base.GetPropertyValue<String>("vsp_pcicomment", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_pcicomment", PropertyType.String, value); }
        }

        public Enums.ProjectStatuses Status
        {
            get { return base.GetPropertyValue<Enums.ProjectStatuses>("statuscode", PropertyType.Status, Enums.ProjectStatuses.Unspecified); }
            set { base.SetPropertyValue<Enums.ProjectStatuses>("statuscode", PropertyType.Status, value); }
        }
        
    }
}
