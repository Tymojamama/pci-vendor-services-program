using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using System.Diagnostics;

namespace PCI.VSP.Plugins.DataLogic
{
    internal class ClientQuestionDataLogic : ServiceObjectBase<Model.ClientQuestion>
    {
        public const String _entityName = "vsp_clientquestion";
        //private static String[] _columnSet = new String[] { "vsp_clientquestionid", "vsp_name", "vsp_accountid", "vsp_clientprojectid", "vsp_templateid", 
        //    "vsp_questionid", "vsp_questioncategoryid", "vsp_answertype", "vsp_questiondatatype", "vsp_answer", "vsp_altanswer", "vsp_choiceanswers",
        //    "vsp_clientwording", "vsp_answerexpirationdate"};

        public ClientQuestionDataLogic(ICrmService crmService) : base(crmService) { }

        public new Guid Create(Model.ClientQuestion question)
        {
            return base.Create(question);
        }

        public List<Model.ClientQuestion> RetrieveByClientProject(Guid clientProjectId)
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            var result = base.RetrieveMultiple(query);

            if (result != null)
                return result.OrderBy(d => d.SortOrder).ToList();
            else
                return new List<Model.ClientQuestion>();
        }

    }
}
