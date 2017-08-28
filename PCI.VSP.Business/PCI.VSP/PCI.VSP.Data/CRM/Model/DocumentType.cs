using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Data.CRM.Model
{
    public class DocumentType : EntityBase
    {
        public DocumentType() : base("vsp_documenttype") { }
        protected DocumentType(String entityName) : base(entityName) { }
        public DocumentType(DynamicEntity e)
            : base(e)
        {
        }

        public Guid DocumentTypeId
        {
            get { return base.GetPropertyValue<Guid>("vsp_documenttypeid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_documenttypeid", PropertyType.Key, value); }
        }

        public new string Name
        {
            get { return base.GetPropertyValue<string>("vsp_name", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_name", PropertyType.String, value); }
        }
    }
}
