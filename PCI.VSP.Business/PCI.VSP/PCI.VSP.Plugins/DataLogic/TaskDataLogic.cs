using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using PCI.VSP.Plugins.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Plugins.DataLogic
{
    class TaskDataLogic : ServiceObjectBase<Task>
    {
        private const string _entityName = "new_projectservice";

        public TaskDataLogic(ICrmService service) : base(service) { }

        public Task Retrieve(Guid taskID)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            query.Criteria.AddCondition("new_projectserviceid", ConditionOperator.Equal, taskID);

            var result = base.RetrieveMultiple(query);

            if (result != null && result.Count > 0)
                return result.First();
            else
                return null;
        }
    }
}
