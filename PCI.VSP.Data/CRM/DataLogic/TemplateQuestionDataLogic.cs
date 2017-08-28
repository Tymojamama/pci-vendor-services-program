using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.Sdk;
using PCI.VSP.Data.CRM.Model;
using Microsoft.Crm.SdkTypeProxy;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class TemplateQuestionDataLogic : ServiceObjectBase<TemplateQuestion, Guid>
    {
        private static String[] _columnSet = new String[] { "vsp_templateid", "vsp_questionid", "vsp_templatequestionid", "vsp_questioncategoryid", "vsp_questionfunctionid" };
        private const String _entityName = "vsp_templatequestion";

        public TemplateQuestionDataLogic(Model.IAuthenticationRequest authRequest) : base(authRequest, _entityName, _columnSet) { }

        /// <summary>
        /// Retrieves Template Questions By Template ID
        /// </summary>
        /// <param name="templateId">Template ID</param>
        /// <returns>List of Template Questions</returns>
        public List<TemplateQuestion> RetrieveByTemplateId(Guid templateId)
        {
            QueryExpression qe = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns(), Distinct = true };
            FilterExpression fe = new FilterExpression();
            fe.AddCondition("vsp_templateid", ConditionOperator.Equal, templateId);
            qe.Criteria = fe;

            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest() { Query = qe, ReturnDynamicEntities = true };
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);
            return res.BusinessEntityCollection.BusinessEntities.Select(e => new TemplateQuestion((DynamicEntity)e)).OrderBy(d => d.SortOrder).ToList();
        }

        public List<TemplateQuestion> RetrieveByQuestionIds(IEnumerable<Guid> questionIds)
        {
            if (questionIds == null || questionIds.Count() == 0)
                return null;

            FilterExpression fe = new FilterExpression();
            fe.AddCondition("vsp_questionid", ConditionOperator.In, questionIds.ToArray());

            List<DynamicEntity> des = base.RetrieveMultiple(fe);
            if (des == null) { return null; }
            return des.Select<DynamicEntity, TemplateQuestion>(tq => new TemplateQuestion(tq)).OrderBy(d => d.SortOrder).ToList();
        }
    }
}
