using Microsoft.Crm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Plugins.Model
{
    public class OverageApproval : EntityBase
    {
        public const string _entityName = "vsp_overageapproval";

        public OverageApproval() : base(_entityName) { }
        public OverageApproval(DynamicEntity entity) : base(entity) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_overageapprovalid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_overageapprovalid", PropertyType.Key, value); }
        }

        public Boolean IsOngoing
        {
            get { return base.GetPropertyValue<Boolean>("vsp_approvaltype", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_approvaltype", PropertyType.Bit, value); }
        }

        public Guid ClientEngagement
        {
            get { return base.GetPropertyValue<Guid>("vsp_clientengagmentid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_clientengagmentid", PropertyType.Lookup, value); }
        }

        public Guid ComponentTask
        {
            get { return base.GetPropertyValue<Guid>("vsp_componenttaskid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_componenttaskid", PropertyType.Lookup, value); }
        }

        public String ComponentTaskID
        {
            get { return base.GetPropertyValue<String>("vsp_name", PropertyType.String, null); }
            set { base.SetPropertyValue<String>("vsp_name", PropertyType.String, value); }
        }

        public DateTime ApprovalDate
        {
            get { return base.GetPropertyValue<DateTime>("vsp_dateapproved", PropertyType.DateTime, default(DateTime)); }
            set { base.SetPropertyValue<DateTime>("vsp_dateapproved", PropertyType.DateTime, value); }
        }

        public Boolean FromRenewal
        {
            get { return base.GetPropertyValue<Boolean>("vsp_fromrenewal", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_fromrenewal", PropertyType.Bit, value); }
        }

        public Decimal HoursApproved
        {
            get { return base.GetPropertyValue<Decimal>("vsp_hoursapproved", PropertyType.Decimal, 0.0M); }
            set { base.SetPropertyValue<Decimal>("vsp_hoursapproved", PropertyType.Decimal, value); }
        }

        public Int32 TaskInstance
        {
            get { return base.GetPropertyValue<Int32>("new_taskinstance", PropertyType.Number, 0); }
            set { base.SetPropertyValue<Int32>("new_taskinstance", PropertyType.Number, value); }
        }

        public Guid CreatedBy
        {
            get { return base.GetPropertyValue<Guid>("ownerid", PropertyType.Owner, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("ownerid", PropertyType.Owner, value); }
        }

        public Guid ApprovedBy
        {
            get { return base.GetPropertyValue<Guid>("new_approvedby", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("new_approvedby", PropertyType.Lookup, value); }
        }
    }
}
