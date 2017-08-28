using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using System.Diagnostics;

namespace PCI.VSP.Plugins.DataLogic
{
    class QuestionDataLogic : ServiceObjectBase<Model.Question>
    {
        private const String _entityName = "vsp_question";
        public QuestionDataLogic(ICrmService crmService) : base(crmService) { }

        public new Guid Create(Model.Question question)
        {
            return base.Create(question);
        }

        public Model.Question Retrieve(Guid questionId)
        {
            Trace.WriteLine("Inside Retrieve - entityName: " + _entityName + "    questionId: " + questionId.ToString());
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            query.Criteria.AddCondition("vsp_questionid", ConditionOperator.Equal, questionId);

            try
            {
                List<Model.Question> questions = base.RetrieveMultiple(query);
                if (questions != null && questions.Count > 0)
                    return questions.First();
                else
                    return null;
            }
            catch (Exception ex)
            {
                ex.Data.Add("QuestionId", questionId.ToString());
                throw;
            }
            
            //return base.Retrieve(_entityName, questionId);
        }

        public List<Model.Question> RetrieveByClientProject(Guid sourceClientProjectId)
        {
            Trace.WriteLine("Inside RetrieveByClientProjectId()");
            QueryExpression query = GetClientProjectExpression(sourceClientProjectId);
            return base.RetrieveMultiple(query);
        }

        private QueryExpression GetClientProjectExpression(Guid clientProjectId)
        {
            Trace.WriteLine("Inside GetClientProjectExpression()");
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            return query;
        }

        public List<Model.Question> RetrieveByTemplate(Guid templateId)
        {
            QueryExpression query = GetTemplateExpression(templateId);
            return base.RetrieveMultiple(query);
        }

        private QueryExpression GetTemplateExpression(Guid templateId)
        {
            Trace.WriteLine("Inside GetClientProjectExpression()");
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();

            LinkEntity templateQuestionLink = new LinkEntity(_entityName, "vsp_templatequestion", "vsp_questionid", "vsp_questionid", JoinOperator.Inner);
            templateQuestionLink.LinkCriteria.AddCondition("vsp_templateid", ConditionOperator.Equal, templateId);

            query.LinkEntities.Add(templateQuestionLink);
            return query;
        }

    }
}
