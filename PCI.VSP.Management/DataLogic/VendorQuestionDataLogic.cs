using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tricension.Data.CRM4.DataLogic;
using Tricension.Data.CRM4.Model;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using PCI.VSP.Management.Model.Enums;

namespace PCI.VSP.Management.DataLogic
{
    internal class VendorQuestionDataLogic : ServiceObjectBase<Model.VendorQuestion, Guid>
    {
        private static String[] _columnSet = new String[] { "vsp_vendorquestionid", "vsp_lastupdated", "vsp_invalidanswerreason", "vsp_answerrejectedreason" };
        public const String _entityName = "vsp_vendorquestion";

        public VendorQuestionDataLogic(IAuthenticationRequest authRequest) : base(authRequest, _entityName, null) { }

        internal List<Model.VendorQuestion> RetrieveExpiredVendorQuestions()
        {
            QueryExpression query = new QueryExpression() { EntityName = _entityName, ColumnSet = new ColumnSet(_columnSet) };
            CrmDateTime expiredDate = new CrmDateTime(DateTime.Now.AddDays(-90.0).ToString("yyyy-MM-ddTHH:mm:ss Z"));
            query.Criteria.AddCondition("vsp_lastupdated", ConditionOperator.LessEqual, expiredDate);
            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.In, new int[] { Convert.ToInt32(QuestionTypes.SearchQuestion_Filter1), Convert.ToInt32(QuestionTypes.PlanAssumption) });

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0) { return null; }

            return des.Select<DynamicEntity, Model.VendorQuestion>(vq => new Model.VendorQuestion(vq)).ToList();
        }

        internal void Update(List<Model.VendorQuestion> vqs)
        {
            if (vqs == null) { return; }
            foreach (Model.VendorQuestion vq in vqs)
                base.Update(vq);
        }
    }
}
