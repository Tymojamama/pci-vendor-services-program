using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk.Query;
using PCI.VSP.Data.Classes;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class PlanAccountDataLogic : ServiceObjectBase<Model.PlanAccount, Guid>
    {
        public PlanAccountDataLogic(Model.IAuthenticationRequest authRequest) : base(authRequest, DataConstants.new_plan, null) { }

        public new Model.PlanAccount Retrieve(Guid planAccountId)
        {
            QueryExpression query = new QueryExpression(DataConstants.new_plan) { ColumnSet = new AllColumns() };
            query.Criteria.AddCondition(DataConstants.new_planid, ConditionOperator.Equal, planAccountId);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }

            return new Model.PlanAccount(base.GetUniqueResult(des));
        }

        public List<Model.PlanAccountServiceProvider> GetServiceProvidersForPlan(Guid planAccountId)
        {
            List<Model.PlanAccountServiceProvider> result = new List<Model.PlanAccountServiceProvider>();

            QueryExpression query = new QueryExpression(DataConstants.vsp_planaccountserviceprovider) { ColumnSet = new AllColumns() };
            query.Criteria.AddCondition(DataConstants.vsp_planaccountid, ConditionOperator.Equal, planAccountId);
            query.Criteria.AddCondition(DataConstants.vsp_startdate, ConditionOperator.NotNull);
            //query.Criteria.AddCondition(DataConstants.vsp_enddate, ConditionOperator.Null);

            List<DynamicEntity> des = base.RetrieveMultiple(query);

            if (des != null)
                foreach (var de in des)
                    result.Add(new Model.PlanAccountServiceProvider(de));

            return result;
        }
    }
}
