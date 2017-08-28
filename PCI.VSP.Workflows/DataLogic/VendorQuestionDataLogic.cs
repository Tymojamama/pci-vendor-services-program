using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace PCI.VSP.Workflows
{
    class VendorQuestionDataLogic
    {
        private ICrmService _service;

        internal VendorQuestionDataLogic(ICrmService service)
        {
            _service = service;
        }

        internal IEnumerable<VendorQuestion> RetrieveMultipleByVendorProduct(Guid vendorProductId)
        {
            QueryExpression query = GetVendorProductExpression(vendorProductId);
            return Convert(_service.RetrieveMultiple(query));
        }

        internal IEnumerable<VendorQuestion> RetrieveMultipleByVendorProfile(Guid vendorId)
        {
            QueryExpression query = GetVendorProfileExpression(vendorId);
            return Convert(_service.RetrieveMultiple(query));
        }

        internal IEnumerable<VendorQuestion> Convert(BusinessEntityCollection bec)
        {
            if (bec == null || bec.BusinessEntities.Count == 0) { return null; }

            IEnumerable<DynamicEntity> des = bec.BusinessEntities.Select<BusinessEntity, DynamicEntity>(vq => (DynamicEntity)vq);
            IEnumerable<VendorQuestion> vqs = des.Select<DynamicEntity, VendorQuestion>(vq => new VendorQuestion(vq));
            return vqs;
        }

        private QueryExpression GetVendorProductExpression(Guid vendorProductId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = VendorQuestion._entityName,
                ColumnSet = new AllColumns()
            };
            query.Criteria.AddCondition("vsp_vendorproductid", ConditionOperator.Equal, vendorProductId);

            FilterExpression cfe = new FilterExpression();
            cfe.FilterOperator = LogicalOperator.Or;
            query.Criteria.AddFilter(cfe);

            String[] notNullables = new String[] { "vsp_altanswer", "vsp_answer" };
            foreach (String notNullable in notNullables)
            {
                cfe.AddCondition(notNullable, ConditionOperator.NotNull);
                cfe.AddCondition(notNullable, ConditionOperator.NotEqual, String.Empty);
            }
            return query;
        }

        private QueryExpression GetVendorProfileExpression(Guid vendorId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = VendorQuestion._entityName,
                ColumnSet = new AllColumns()
            };
            query.Criteria.AddCondition("vsp_vendorid", ConditionOperator.Equal, vendorId);
            query.Criteria.AddCondition("vsp_vendorproductid", ConditionOperator.Null);

            FilterExpression cfe = new FilterExpression();
            cfe.FilterOperator = LogicalOperator.Or;
            query.Criteria.AddFilter(cfe);

            String[] notNullables = new String[] { "vsp_altanswer", "vsp_answer" };
            foreach (String notNullable in notNullables)
            {
                cfe.AddCondition(notNullable, ConditionOperator.NotNull);
                cfe.AddCondition(notNullable, ConditionOperator.NotEqual, String.Empty);
            }
            return query;
        }
    }
}
