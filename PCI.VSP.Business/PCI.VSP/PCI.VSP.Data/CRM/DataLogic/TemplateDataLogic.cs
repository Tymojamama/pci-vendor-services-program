using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.Sdk;
using PCI.VSP.Data.Enums;
using PCI.VSP.Data.CRM.Model;
using PCI.VSP.Data.Classes;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class TemplateDataLogic : ServiceObjectBase<Model.Template, Guid>
    {
        private static String[] _columnSet = new String[] { "vsp_templateid", "vsp_name", "vsp_vspproductid", "vsp_templatetype" };
        public const String _entityName = DataConstants.vsp_template;

        public TemplateDataLogic(Model.IAuthenticationRequest authRequest) : base(authRequest, _entityName, _columnSet) { }

        /// <summary>
        /// Retrieve Templates by Vendor Product ID
        /// </summary>
        /// <param name="productId">Vendor Product ID</param>
        /// <returns>List of Template</returns>
        public List<Template> RetrieveTemplatesByProductId(Guid productId)
        {
            FilterExpression fe = new FilterExpression();
            fe.AddCondition("vsp_vspproductid", ConditionOperator.Equal, productId);
            fe.AddCondition("vsp_templatetype", ConditionOperator.Equal, TemplateType.VendorProductTemplate);

            List<DynamicEntity> des = base.RetrieveMultiple(fe);
            List<Template> templates = new List<Template>();
            foreach (DynamicEntity de in des)
                templates.Add(new Template(de));
            return templates;
        }

        /// <summary>
        /// Retrieve Templates for Vendor Profile
        /// </summary>
        /// <returns>List of Template</returns>
        public List<Template> RetrieveProfileTemplates()
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            query.Criteria.AddCondition("vsp_templatetype", ConditionOperator.Equal, TemplateType.VendorProfileTemplate);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }
            return des.Select<DynamicEntity, Template>(t => new Template(t)).ToList();
        }

        public List<Template> RetrieveAllTemplates()
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            //query.Criteria.

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) return null;
            return des.Select<DynamicEntity, Template>(t => new Template(t)).ToList();
        }

        public List<Template> RetrieveTemplatesUsedByClientProject(Guid clientProjectId, TemplateType templateType)
        {
            List<Template> result = new List<Template>();
            ColumnSet queryColumn = new Microsoft.Crm.Sdk.Query.ColumnSet(DataConstants.vsp_clientquestion);
            queryColumn.AddColumn(DataConstants.vsp_templateid);

            QueryExpression query = new QueryExpression(DataConstants.vsp_clientquestion) { ColumnSet = queryColumn };
            query.Criteria.AddCondition(DataConstants.vsp_clientprojectid, ConditionOperator.Equal, clientProjectId);
            
            List<Guid> templateIds = new List<Guid>();

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des != null || des.Count > 0)
                foreach (var de in des)
                {
                    var clientQuestion = new Model.ClientQuestion(de);
                    if (clientQuestion.TemplateId != null && clientQuestion.TemplateId != Guid.Empty && !templateIds.Contains(clientQuestion.TemplateId))
                        templateIds.Add(clientQuestion.TemplateId);
                }

            query = new QueryExpression(_entityName) { ColumnSet = new AllColumns() };
            query.Criteria.AddCondition(DataConstants.vsp_templateid, ConditionOperator.In, templateIds.ToArray());

            if (templateType != TemplateType.Unspecified)
                query.Criteria.AddCondition(DataConstants.vsp_templatetype, ConditionOperator.Equal, (int)templateType);

            des = base.RetrieveMultiple(query);
            if (des != null || des.Count > 0)
                foreach (DynamicEntity de in des)
                    result.Add(new Template(de));

            return result;
        }
    }
}
