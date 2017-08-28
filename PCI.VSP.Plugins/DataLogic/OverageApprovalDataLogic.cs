using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using PCI.VSP.Plugins.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Plugins.DataLogic
{
    class OverageApprovalDataLogic : ServiceObjectBase<OverageApproval>
    {
        public const string _entityName = "vsp_overageapproval";

        public OverageApprovalDataLogic(ICrmService service) : base(service) { }

        public new Guid Create(OverageApproval approval)
        {
            return base.Create(approval);
        }

        public List<OverageApproval> RetrieveByCustomerEngagement(Guid customerEngagementId)
        {
            QueryExpression query = new QueryExpression();
            query.ColumnSet = new AllColumns();
            query.EntityName = _entityName;
            query.Criteria.AddCondition("vsp_clientengagmentid", ConditionOperator.Equal, customerEngagementId);

            var result = base.RetrieveMultiple(query);

            if (result != null)
                return result.ToList();
            else
                return new List<OverageApproval>();
        }
    }
}
