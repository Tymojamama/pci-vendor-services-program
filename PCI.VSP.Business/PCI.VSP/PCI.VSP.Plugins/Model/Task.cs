using Microsoft.Crm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Plugins.Model
{
    class Task : EntityBase
    {
        public const string _entityName = "new_projectservice";

        public Task() : base(_entityName) { }
        public Task(DynamicEntity entity) : base(entity) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("new_projectserviceid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("new_projectserviceid", PropertyType.Key, value); }
        }

        public string Code
        {
            get { return base.GetPropertyValue<string>("new_code", PropertyType.String, null); }
            set { base.SetPropertyValue<string>("new_code", PropertyType.String, value); }
        }
    }
}
