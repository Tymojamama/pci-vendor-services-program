using Microsoft.Crm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Plugins.Model
{
    public class ComponentTask : EntityBase
    {
        private const string _entityName = "new_projecttask";

        public ComponentTask() : base(_entityName) { }
        public ComponentTask(DynamicEntity entity) : base(entity) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("new_projecttaskid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("new_projecttaskid", PropertyType.Key, value); }
        }

        public Guid ClientEngagement
        {
            get { return base.GetPropertyValue<Guid>("new_projectid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("new_projectid", PropertyType.Lookup, value);}
        }

        public int TaskInstance
        {
            get { return base.GetPropertyValue<int>("new_taskinstance", PropertyType.Number, 0); }
            set { base.SetPropertyValue<int>("new_taskinstance", PropertyType.Number, value); }
        }

        public Guid TaskID
        {
            get { return base.GetPropertyValue<Guid>("new_projectserviceid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("new_projectserviceid", PropertyType.Lookup, value); }
        }
    }
}
