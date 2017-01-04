using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Data.CRM.Model
{
    public abstract class Contact : EntityBase
    {
        private const string _entityName = "contact";

        public Contact() : base(_entityName) { }
        public Contact(Microsoft.Crm.Sdk.DynamicEntity e) : base(e) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("contactid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("contactid", PropertyType.Key, value); }
        }

        public Guid AccountId
        {
            get { return base.GetPropertyValue<Guid>("parentcustomerid", PropertyType.Customer, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("parentcustomerid", PropertyType.Customer, value); }
        }

        public String Email
        {
            get { return base.GetPropertyValue<String>("emailaddress1", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("emailaddress1", PropertyType.String, value); }
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

        public String Username
        {
            get { return base.GetPropertyValue<String>("vsp_username", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_username", PropertyType.String, value); }
        }

        public String Password
        {
            get { return base.GetPropertyValue<String>("vsp_password", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_password", PropertyType.String, value); }
        }

        public String SecurityQuestion
        {
            get { return base.GetPropertyValue<String>("vsp_securityquestion", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_securityquestion", PropertyType.String, value); }
        }

        internal String SecurityAnswer
        {
            get { return base.GetPropertyValue<String>("vsp_securityanswer", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_securityanswer", PropertyType.String, value); }
        }

        public DateTime LastLogin
        {
            get { return base.GetPropertyValue<DateTime>("vsp_lastlogin", PropertyType.DateTime, DateTime.MinValue); }
            set { base.SetPropertyValue<DateTime>("vsp_lastlogin", PropertyType.DateTime, value); }
        }

        public Boolean MustChangePassword
        {
            get { return base.GetPropertyValue<Boolean>("vsp_mustchangepassword", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_mustchangepassword", PropertyType.Bit, value); }
        }

        public Boolean IsLocked
        {
            get { return base.GetPropertyValue<Boolean>("vsp_islocked", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_islocked", PropertyType.Bit, value); }
        }
        
        public Boolean IsAdmin
        {       
            get { return base.GetPropertyValue<Boolean>("vsp_isadmin", PropertyType.Bit, false); }
        }
        
    }
}
