using Microsoft.Crm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Plugins.Model
{
    public class ClientEngagement : EntityBase
    {
        public const string _entityName = "new_project";

        public ClientEngagement() : base(_entityName) { }
        public ClientEngagement(DynamicEntity entity) : base(entity) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("new_projectid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("new_projectid", PropertyType.Key, value); }
        }

        public Guid RenewedEngagement
        {
            get { return base.GetPropertyValue<Guid>("new_renewedid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("new_renewedid", PropertyType.Lookup, value); }
        }
    }
}
