using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCI.VSP.Data.Classes;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class PlanAccountServiceProviderTypeDataLogic : ServiceObjectBase<Model.PlanAccountServiceProviderType, Guid>
    {
        public PlanAccountServiceProviderTypeDataLogic(Model.IAuthenticationRequest authRequest) : base(authRequest, DataConstants.vsp_planaccountserviceprovidertype, null) { }

        public new Model.PlanAccountServiceProviderType Retrieve(Guid planAccountServiceProviderTypeId)
        {
            QueryExpression query = new QueryExpression(DataConstants.vsp_planaccountserviceprovidertype) { ColumnSet = new AllColumns() };
            query.Criteria.AddCondition(DataConstants.vsp_planaccountserviceprovidertypeid, ConditionOperator.Equal, planAccountServiceProviderTypeId);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }

            return new Model.PlanAccountServiceProviderType(base.GetUniqueResult(des));
        }

        public List<Model.PlanAccountServiceProviderType> RetrieveAll()
        {
            List<Model.PlanAccountServiceProviderType> result = new List<Model.PlanAccountServiceProviderType>();
            QueryExpression query = new QueryExpression(DataConstants.vsp_planaccountserviceprovidertype) { ColumnSet = new AllColumns() };
            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des != null)
                foreach (var de in des)
                    result.Add(new Model.PlanAccountServiceProviderType(de));

            return result;
        }

        public Dictionary<Guid, Model.PlanAccountServiceProviderType> RetrieveAllIntoDictionary(List<Guid> serviceTypeIds = null)
        {
            Dictionary<Guid, Model.PlanAccountServiceProviderType> result = new Dictionary<Guid, Model.PlanAccountServiceProviderType>();
            var list = RetrieveAll();

            if (serviceTypeIds != null)
                list = list.Where(z => serviceTypeIds.Contains(z.Id)).ToList();

            foreach (var item in list)
                result.Add(item.Id, item);

            return result;
        }
    }
}
