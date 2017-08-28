using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Workflows
{
    class Question : EntityBase
    {
        internal const String _entityName = "vsp_question";

        internal Question() : base(_entityName) { }
        internal Question(DynamicEntity e) : base(e) { }
        
        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_questionid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_questionid", PropertyType.Key, value); }
        }
        
    }
}
