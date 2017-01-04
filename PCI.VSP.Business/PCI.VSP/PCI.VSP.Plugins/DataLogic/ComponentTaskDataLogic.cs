using Microsoft.Crm.Sdk;
using PCI.VSP.Plugins.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Plugins.DataLogic
{
    class ComponentTaskDataLogic : ServiceObjectBase<ComponentTask>
    {
        public const string _entityName = "new_projecttask";

        public ComponentTaskDataLogic(ICrmService service) : base(service) { }


    }
}
