using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCI.VSP.Data.Classes;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class PlanAccountServiceProviderDataLogic : ServiceObjectBase<Model.PlanAccountServiceProvider, Guid>
    {
        public PlanAccountServiceProviderDataLogic(Model.IAuthenticationRequest authRequest) : base(authRequest, DataConstants.vsp_planaccountserviceprovider, null) { }

        public new Model.PlanAccountServiceProvider Retrieve(Guid planAccountServiceProviderId)
        {
            QueryExpression query = new QueryExpression(DataConstants.vsp_planaccountserviceprovider) { ColumnSet = new AllColumns() };
            query.Criteria.AddCondition(DataConstants.vsp_planaccountserviceproviderid, ConditionOperator.Equal, planAccountServiceProviderId);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }

            return new Model.PlanAccountServiceProvider(base.GetUniqueResult(des));
        }
    }
}
