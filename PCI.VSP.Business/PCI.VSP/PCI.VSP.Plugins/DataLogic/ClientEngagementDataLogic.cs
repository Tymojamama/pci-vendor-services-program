using System;
using System.Linq;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using PCI.VSP.Plugins.Model;

namespace PCI.VSP.Plugins.DataLogic
{
    class ClientEngagementDataLogic : ServiceObjectBase<ClientEngagement>
    {
        private const string _entityName = "new_project";

        public ClientEngagementDataLogic(ICrmService service) : base(service) { }

        public ClientEngagement Retrieve(Guid clientEngagementId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            query.Criteria.AddCondition("new_projectid", ConditionOperator.Equal, clientEngagementId);

            var result = base.RetrieveMultiple(query);

            if (result != null && result.Count > 0)
                return result.First();
            else
                return null;
        }
    }
}
