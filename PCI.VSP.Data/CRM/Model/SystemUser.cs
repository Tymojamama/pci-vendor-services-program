using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Data.CRM.Model
{
    public class SystemUser : EntityBase
    {
        public SystemUser() : base("systemuser") { }
        public SystemUser(Microsoft.Crm.Sdk.DynamicEntity e) : base(e) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("systemuserid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("systemuserid", PropertyType.Key, value); }
        }

        public String Username
        {
            get { return base.GetPropertyValue<String>("domainname", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("domainname", PropertyType.String, value); }
        }
        
        public String FirstName
        {
            get { return base.GetPropertyValue<String>("firstname", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("firstname", PropertyType.String, value); }
        }
        
        public String LastName
        {
            get { return base.GetPropertyValue<String>("lastname", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("lastname", PropertyType.String, value); }
        }
        
        public String Email
        {
            get { return base.GetPropertyValue<String>("internalemailaddress", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("internalemailaddress", PropertyType.String, value); }
        }

    }
}
