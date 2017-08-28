using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace PCI.VSP.Plugins.DataLogic
{
    class TemplateQuestionDataLogic : ServiceObjectBase<Model.TemplateQuestion>
    {
        //private static String[] _columnSet = new String[] { "vsp_templateid", "vsp_questionid", "vsp_templatequestionid", "vsp_questioncategoryid" };
        private const String _entityName = "vsp_templatequestion";

        public TemplateQuestionDataLogic(ICrmService crmService) : base(crmService) { }

        /// <summary>
        /// Retrieves Template Questions By Template ID
        /// </summary>
        /// <param name="templateId">Template ID</param>
        /// <returns>List of Template Questions</returns>
        public List<Model.TemplateQuestion> RetrieveByTemplateId(Guid templateId)
        {
            try
            {
                QueryExpression query = new QueryExpression()
                {
                    EntityName = _entityName,
                    ColumnSet = new AllColumns()
                };
                query.Criteria.AddCondition("vsp_templateid", ConditionOperator.Equal, templateId);

            
                List<Model.TemplateQuestion> tql = base.RetrieveMultiple(query);
                return tql.OrderBy(tq => tq.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                ex.Data.Add("TemplateId", templateId.ToString());
                throw;
            }
        }
    }
}
