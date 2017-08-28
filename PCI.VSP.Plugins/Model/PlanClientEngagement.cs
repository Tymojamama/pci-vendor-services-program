using Microsoft.Crm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Plugins.Model
{
    class PlanClientEngagement : EntityBase
    {
        public const string _entityName = "new_new_plan_new_project";

        public PlanClientEngagement() : base(_entityName) { }
        public PlanClientEngagement(DynamicEntity entity) : base(entity) { }

        public Guid PlanId
        {
            get
            {
                return GetId("new_planid");
            }
        }
        public Guid ProjectId
        {
            get
            {
                return GetId("new_projectid");
            }
        }
    }
}
