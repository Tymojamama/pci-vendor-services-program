using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;

namespace PCI.VSP.Data.CRM.Model
{
    public class ProjectVendor : EntityBase
    {
        private const String _entityName = "vsp_projectvendor";
        public ProjectVendor() : base(_entityName) { }
        public ProjectVendor(DynamicEntity e)
            : base(e)
        {
            base.Name = _entityName;
        }

        public new String Name
        {
            get { return base.GetPropertyValue<String>("vsp_name", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_name", PropertyType.String, value); }
        }
        
        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_projectvendorid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_projectvendorid", PropertyType.Key, value); }
        }

        public Guid VendorProductId
        {
            get { return base.GetPropertyValue<Guid>("vsp_vendorproductid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_vendorproductid", PropertyType.Lookup, value); }
        }

        public Guid ClientProjectId
        {
            get { return base.GetPropertyValue<Guid>("vsp_clientprojectid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_clientprojectid", PropertyType.Lookup, value); }
        }

        public Enums.ProjectVendorStatuses Status
        {
            get { return base.GetPropertyValue<Enums.ProjectVendorStatuses>("statuscode", PropertyType.Status, Enums.ProjectVendorStatuses.Unspecified); }
            set { base.SetPropertyValue<Enums.ProjectVendorStatuses>("statuscode", PropertyType.Status, value); }
        }

        public Boolean VendorAnswersConfirmed
        {
            get { return base.GetPropertyValue<Boolean>("vsp_vendoranswersconfirmed", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_vendoranswersconfirmed", PropertyType.Bit, value); }
        }

        public Boolean Excluded
        {
            get { return base.GetPropertyValue<Boolean>("vsp_excluded", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_excluded", PropertyType.Bit, value); }
        }

        public Boolean Phase1Benchmark
        {
            get { return base.GetPropertyValue<Boolean>("vsp_phase1benchmark", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_phase1benchmark", PropertyType.Bit, value); }
        }

        public Boolean Phase1Result
        {
            get { return base.GetPropertyValue<Boolean>("vsp_phase1result", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_phase1result", PropertyType.Bit, value); }
        }

        public Boolean Phase2Benchmark
        {
            get { return base.GetPropertyValue<Boolean>("vsp_phase2benchmark", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_phase2benchmark", PropertyType.Bit, value); }
        }

        public Boolean Phase2Result
        {
            get { return base.GetPropertyValue<Boolean>("vsp_phase2result", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_phase2result", PropertyType.Bit, value); }
        }
    }
}
