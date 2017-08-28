using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Data.CRM.Model
{
    public class DocumentTemplate : EntityBase
    {
        private const string _entityName = "vsp_documenttemplate";

        public DocumentTemplate() : base(_entityName) { }
        public DocumentTemplate(DynamicEntity e) : base(e) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_documenttemplateid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_documenttemplateid", PropertyType.Key, value); }
        }

        public string Name
        {
            get { return base.GetPropertyValue<string>("vsp_name", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<string>("vsp_name", PropertyType.String, value); }
        }
    }
}
