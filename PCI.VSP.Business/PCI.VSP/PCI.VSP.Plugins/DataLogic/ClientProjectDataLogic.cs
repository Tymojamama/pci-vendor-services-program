using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using PCI.VSP.Plugins.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Plugins.DataLogic
{
    class ClientProjectDataLogic : ServiceObjectBase<ClientProject>
    {
        private const string _entityName = "vsp_clientproject";

        public ClientProjectDataLogic(ICrmService service) : base(service) { }

        public ClientProject Retrieve(Guid clientProjectId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);

            var result = base.RetrieveMultiple(query);

            if (result != null && result.Count > 0)
                return result.First();
            else
                return null;
        }

        public void Update(ClientProject clientProject)
        {
            base.Update(clientProject);
        }
    }
}
