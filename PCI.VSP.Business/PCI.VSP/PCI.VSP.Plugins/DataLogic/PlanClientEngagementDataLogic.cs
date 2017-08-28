using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Plugins.DataLogic
{
    class PlanClientEngagementDataLogic : ServiceObjectBase<Model.PlanClientEngagement>
    {
        public PlanClientEngagementDataLogic(ICrmService service) : base(service) { }

        public void Create(Guid planid, Guid projectid)
        {
            base.Associate(Model.PlanClientEngagement._entityName, "new_plan", planid, "new_project", projectid);
        }
        public List<Model.PlanClientEngagement> RetrieveByClientEngagementId(Guid id)
        {
            var qe = new QueryExpression();
            qe.EntityName = Model.PlanClientEngagement._entityName;
            qe.ColumnSet = new AllColumns();
            qe.Criteria = new FilterExpression();
            qe.PageInfo = new PagingInfo();
            qe.PageInfo.Count = 5000;
            qe.PageInfo.PageNumber = 1;
            qe.Criteria.AddCondition("new_projectid", ConditionOperator.Equal, id);
            var result = base.RetrieveMultiple(qe);
            if (result != null)
                return result.ToList();
            else
                return new List<Model.PlanClientEngagement>();
        }
    }
}
