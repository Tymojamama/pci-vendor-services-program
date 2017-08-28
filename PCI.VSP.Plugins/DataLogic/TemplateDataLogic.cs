using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace PCI.VSP.Plugins.DataLogic
{
    class TemplateDataLogic : ServiceObjectBase<Model.Template>
    {
        private const String _entityName = "vsp_template";
        public TemplateDataLogic(ICrmService crmService) : base(crmService) { }

        public new Model.Template Retrieve(Guid templateId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            query.Criteria.AddCondition("vsp_templateid", ConditionOperator.Equal, templateId);

            try
            {
                List<Model.Template> templates = base.RetrieveMultiple(query);
                return templates.First();
            }
            catch (Exception ex)
            {
                ex.Data.Add("TemplateId", templateId.ToString());
                throw;
            }
        }
    }
}
