using Microsoft.Crm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Plugins.Model
{
    public class ClientProject : EntityBase
    {
        public const string _entityName = "vsp_clientproject";

        public ClientProject() : base(_entityName) { }
        public ClientProject(DynamicEntity entity) : base(entity) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_clientprojectid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_clientprojectid", PropertyType.Key, value); }
        }

        public string Name
        {
            get { return base.GetPropertyValue<string>("vsp_name", PropertyType.String, null); }
            set { base.SetPropertyValue<string>("vsp_name", PropertyType.String, value); }
        }

        public Enums.ProjectTypes ClientProjectType
        {
            get { return base.GetPropertyValue<Enums.ProjectTypes>("vsp_clientprojectype", PropertyType.Picklist, Enums.ProjectTypes.Search); }
            set { base.SetPropertyValue<Enums.ProjectTypes>("vsp_clientprojectype", PropertyType.Picklist, value); }
        }

        public Guid ClientId
        {
            get { return base.GetPropertyValue<Guid>("vsp_clientaccountid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_clientaccountid", PropertyType.Lookup, value); }
        }

        public Status Status
        {
            get { return base.GetPropertyValue<Status>("statecode", PropertyType.Status, Status.Null); }
            set { base.SetPropertyValue<Status>("statecode", PropertyType.Status, value); }
        }

        public Status StatusReason
        {
            get { return base.GetPropertyValue<Status>("statuscode", PropertyType.Status, Status.Null); }
            set { base.GetPropertyValue<Status>("statuscode", PropertyType.Status, value); }
        }

        public Guid ContactId
        {
            get { return base.GetPropertyValue<Guid>("vsp_contactid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_contactid", PropertyType.Lookup, value); }
        }

        public Owner OwnerId
        {
            get { return base.GetPropertyValue<Owner>("ownerid", PropertyType.Owner, Owner.Null); }
            set { base.SetPropertyValue<Owner>("ownerid", PropertyType.Owner, value); }
        }

        public Guid ManagerId
        {
            get { return base.GetPropertyValue<Guid>("vsp_manageruserid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_manageruserid", PropertyType.Lookup, value); }
        }

        public string ClientComment
        {
            get { return base.GetPropertyValue<string>("vsp_clientcomment", PropertyType.String, null); }
            set { base.SetPropertyValue<string>("vsp_clientcomment", PropertyType.String, value); }
        }

        public string PciComment
        {
            get { return base.GetPropertyValue<string>("vsp_pcicomment", PropertyType.String, null); }
            set { base.SetPropertyValue<string>("vsp_pcicomment", PropertyType.String, value); }
        }

        public Guid PlanAccountId
        {
            get { return base.GetPropertyValue<Guid>("vsp_planaccountid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_planaccountid", PropertyType.Lookup, value); }
        }
    }
}
