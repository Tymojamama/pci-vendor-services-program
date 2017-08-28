using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace PCI.VSP.Workflows
{
    class QuestionDataLogic
    {
        private String[] _columnSet = { "vsp_questionid" };
        private ICrmService _service;

        internal QuestionDataLogic(ICrmService service)
        {
            _service = service;
        }

        internal IEnumerable<Question> RetrieveMultipleByVendorProduct(Guid vendorProductId)
        {
            QueryExpression query = GetVendorProductExpression(vendorProductId);
            return Convert(_service.RetrieveMultiple(query));
        }

        internal IEnumerable<Question> RetrieveMultipleByVendorProfile(Guid vendorId)
        {
            QueryExpression query = GetVendorProfileExpression(vendorId);
            return Convert(_service.RetrieveMultiple(query));
        }

        internal IEnumerable<Question> Convert(BusinessEntityCollection bec)
        {
            if (bec == null || bec.BusinessEntities.Count == 0) { return null; }

            IEnumerable<DynamicEntity> des = bec.BusinessEntities.Select<BusinessEntity, DynamicEntity>(q => (DynamicEntity)q);
            IEnumerable<Question> qs = des.Select<DynamicEntity, Question>(q => new Question(q));
            return qs;
        }

        private QueryExpression GetVendorProductExpression(Guid vendorProductId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = Question._entityName,
                ColumnSet = new ColumnSet(_columnSet)
            };

            LinkEntity templateQuestionLink = new LinkEntity(Question._entityName, "vsp_templatequestion", "vsp_questionid", "vsp_questionid", JoinOperator.Inner);
            LinkEntity templateLink = new LinkEntity("vsp_templatequestion", "vsp_template", "vsp_templateid", "vsp_templateid", JoinOperator.Inner);
            LinkEntity vendorProductLink = new LinkEntity("vsp_template", "vsp_vendorproduct", "vsp_vspproductid", "vsp_productid", JoinOperator.Inner);
            vendorProductLink.LinkCriteria.AddCondition("vsp_vendorproductid", ConditionOperator.Equal, vendorProductId);

            query.LinkEntities.Add(templateQuestionLink);
            templateQuestionLink.LinkEntities.Add(templateLink);
            templateLink.LinkEntities.Add(vendorProductLink);
            return query;
        }

        private QueryExpression GetVendorProfileExpression(Guid vendorId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = Question._entityName,
                ColumnSet = new AllColumns()
            };

            LinkEntity templateQuestionLink = new LinkEntity(Question._entityName, "vsp_templatequestion", "vsp_questionid", "vsp_questionid", JoinOperator.Inner);
            LinkEntity templateLink = new LinkEntity("vsp_templatequestion", "vsp_template", "vsp_templateid", "vsp_templateid", JoinOperator.Inner);
            templateLink.LinkCriteria.AddCondition("vsp_vspproductid", ConditionOperator.Null);

            templateQuestionLink.LinkEntities.Add(templateLink);
            query.LinkEntities.Add(templateQuestionLink);

            return query;
        }
    }
}
