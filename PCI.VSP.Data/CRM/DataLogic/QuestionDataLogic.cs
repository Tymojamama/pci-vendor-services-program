using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.Sdk;
using PCI.VSP.Data.CRM.Model;
using PCI.VSP.Data.Enums;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class QuestionDataLogic : ServiceObjectBase<Model.Question, Guid>
    {
        private const String _entityName = "vsp_question";
        private static String[] _columnSet = new String[] { "vsp_clientprojectid", "vsp_questionid" };
        public QuestionDataLogic(Model.IAuthenticationRequest authRequest)
            : base(authRequest, _entityName, _columnSet)
        {
        }

        public new Model.Question Retrieve(Guid questionId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            query.Criteria.AddCondition("vsp_questionid", ConditionOperator.Equal, questionId);
            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0) { return null; }
            return new Model.Question(base.GetUniqueResult(des));
        }

        public Model.Question RetrievebyPlanName(string questionName)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            query.Criteria.AddCondition("vsp_name", ConditionOperator.Equal, questionName);
            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0) { return null; }
            return new Model.Question(base.GetUniqueResult(des));
        }

        public List<Question> RetrieveAll()
        {
            QueryExpression qe = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns(), Criteria = new FilterExpression() };
            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest() { Query = qe, ReturnDynamicEntities = true };
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);
            return res.BusinessEntityCollection.BusinessEntities.Select(e => new Question((DynamicEntity)e)).ToList();
        }

        public List<Model.Question> RetrieveProjectInquiriesByVendorMin(Guid accountId, Guid contactId, Guid? projectVendorId)
        {
            return RetrieveProjectInquiriesByVendor(accountId, contactId, projectVendorId, new ColumnSet(new String[] { "vsp_clientprojectid", "vsp_questionid" }));
        }

        public List<Model.Question> RetrieveProjectInquiriesByVendor(Guid accountId, Guid contactId, Guid? projectVendorId)
        {
            return RetrieveProjectInquiriesByVendor(accountId, contactId, projectVendorId, new AllColumns());
        }

        public List<Model.Question> RetrieveByVendorProductMin(Guid vendorProductId)
        {
            String[] columns = new String[] {"vsp_clientprojectid", "vsp_questionid", "vsp_name"};
            return this.RetrieveByVendorProduct(vendorProductId, new ColumnSet(columns));
        }

        public List<Model.Question> RetrieveByVendorProduct(Guid vendorProductId)
        {
            return this.RetrieveByVendorProduct(vendorProductId, new AllColumns());
        }

        public List<Model.Question> RetrieveComparisonTypes(IEnumerable<Guid> questionIds)
        {
            if (questionIds == null) { return null; }
            Guid[] qIds = questionIds.ToArray();
            if (qIds.Length == 0) { return null; }

            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new ColumnSet(new String[] { "vsp_questionid", "vsp_comparisontype", "vsp_questiondatatype" });
            query.Criteria.AddCondition("vsp_questionid", ConditionOperator.In, qIds);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }

            return des.Select<DynamicEntity, Model.Question>(q => new Model.Question(q)).OrderBy(d => d.SortOrder).ToList();
        }

        private List<Model.Question> RetrieveProjectInquiriesByVendor(Guid accountId, Guid contactId, Guid? projectVendorId, ColumnSetBase columnSet)
        {
            try
            {
                QueryExpression query = GetProjectVendorExpression(accountId, contactId, projectVendorId, columnSet);
                List<DynamicEntity> des = base.RetrieveMultiple(query);
                if (des == null) { return null; }

                return des.Select<DynamicEntity, Model.Question>(q => new Model.Question(q)).OrderBy(d => d.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                ex.Data.Add("AccountId", accountId);
                ex.Data.Add("ContactId", contactId);
                throw;
            }
        }

        private List<Model.Question> RetrieveByVendorProduct(Guid vendorProductId, ColumnSetBase columnSet)
        {
            try
            {
                QueryExpression query = this.GetVendorProductExpression(vendorProductId, columnSet);
                List<DynamicEntity> des = base.RetrieveMultiple(query);
                if (des == null) { return null; }
                return des.Select<DynamicEntity, Model.Question>(q => new Model.Question(q)).OrderBy(d => d.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                ex.Data.Add("VendorProductId", vendorProductId);
                throw;
            }
        }

        private QueryExpression GetProjectVendorExpression(Guid accountId, Guid contactId, Guid? projectVendorId, ColumnSetBase columnSet)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = columnSet
            };

            // filter expressions added to work with plan information/assumptions
            //FilterExpression fe = new FilterExpression();
            //fe.AddCondition("vsp_questiontype", ConditionOperator.NotEqual, Convert.ToInt32(Enums.QuestionTypes.PlanAssumption));
            //fe.FilterOperator = LogicalOperator.Or;

            //FilterExpression fe2 = new FilterExpression();
            //fe.AddCondition("vsp_questiontype", ConditionOperator.Equal, Convert.ToInt32(Enums.QuestionTypes.PlanAssumption));
            //fe.FilterOperator = LogicalOperator.And;
            //fe.AddCondition("vsp_planassumptionvisibletovendor", ConditionOperator.Equal, true);

            //fe.AddFilter(fe2);
            //query.Criteria = fe;

            //query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.Equal, Convert.ToInt32(Enums.QuestionTypes.SearchQuestion));

            // link to client project
            LinkEntity clientProjectLink = new LinkEntity(_entityName, "vsp_clientproject", "vsp_clientprojectid", "vsp_clientprojectid", JoinOperator.Inner);
            clientProjectLink.LinkCriteria.AddCondition("statuscode", ConditionOperator.Equal, 1); // filter out closed client projects
            query.LinkEntities.Add(clientProjectLink);

            // link to project vendor
            LinkEntity projectVendorLink = new LinkEntity("vsp_clientproject", "vsp_projectvendor", "vsp_clientprojectid", "vsp_clientprojectid", JoinOperator.Inner);
            if (projectVendorId.HasValue)
                projectVendorLink.LinkCriteria.AddCondition("vsp_projectvendorid", ConditionOperator.Equal, projectVendorId.Value);
            clientProjectLink.LinkEntities.Add(projectVendorLink);

            //// link to vendor product
            LinkEntity vendorProductLink = new LinkEntity("vsp_projectvendor", "vsp_vendorproduct", "vsp_vendorproductid", "vsp_vendorproductid", JoinOperator.Inner);
            vendorProductLink.LinkCriteria.AddCondition("vsp_accountid", ConditionOperator.Equal, accountId);
            projectVendorLink.LinkEntities.Add(vendorProductLink);

            query.AddOrder("modifiedon", OrderType.Ascending);
            return query;
        }

        private QueryExpression GetVendorProductExpression(Guid vendorProductId, ColumnSetBase columnSet)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = columnSet,
                Distinct = true
            };
            //query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.Equal, Convert.ToInt32(Enums.QuestionTypes.SearchQuestion));

            // link to templateQuestion
            LinkEntity templateQuestionLink = new LinkEntity(_entityName, "vsp_templatequestion", "vsp_questionid", "vsp_questionid", JoinOperator.Inner);

            // link to template
            LinkEntity templateLink = new LinkEntity("vsp_templatequestion", "vsp_template", "vsp_templateid", "vsp_templateid", JoinOperator.Inner);

            // link to product
            LinkEntity productLink = new LinkEntity("vsp_template", "vsp_product", "vsp_vspproductid", "vsp_productid", JoinOperator.Inner);

            // link to vendorProduct where vendorProductId 
            LinkEntity vendorProductLink = new LinkEntity("vsp_product", "vsp_vendorproduct", "vsp_productid", "vsp_productid", JoinOperator.Inner);
            vendorProductLink.LinkCriteria.AddCondition("vsp_vendorproductid", ConditionOperator.Equal, vendorProductId);

            query.LinkEntities.Add(templateQuestionLink);
            templateQuestionLink.LinkEntities.Add(templateLink);
            templateLink.LinkEntities.Add(productLink);
            productLink.LinkEntities.Add(vendorProductLink);
            query.AddOrder("modifiedon", OrderType.Ascending);

            return query;
        }

        /// <summary>
        /// Retrieve Questions that are assigned to a Template with a Template Type of Vendor Profile
        /// </summary>
        /// <returns>List of Question</returns>
        public List<Question> GetVendorProfileTemplateQuestions()
        {
            QueryExpression qe = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns() };
            LinkEntity leTemplateQuestion = new LinkEntity("vsp_question", "vsp_templatequestion", "vsp_questionid", "vsp_questionid", JoinOperator.Inner);
            LinkEntity leTemplate = new LinkEntity("vsp_templatequestion", "vsp_template", "vsp_templateid", "vsp_templateid", JoinOperator.Inner);
            leTemplate.LinkCriteria.AddCondition("vsp_templatetype", ConditionOperator.Equal, TemplateType.VendorProfileTemplate);
            
            leTemplateQuestion.LinkEntities.Add(leTemplate);
            qe.LinkEntities.Add(leTemplateQuestion);

            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest() { Query = qe, ReturnDynamicEntities = true };
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);
            return res.BusinessEntityCollection.BusinessEntities.Select(e => new Question((DynamicEntity)e)).OrderBy(d => d.SortOrder).ToList();
        }

        /// <summary>
        /// Retrieve Questions that are assigned to a Template with a Template Type of Vendor Product for a specific Product Type
        /// </summary>
        /// <returns>List of Question</returns>
        public List<Question> GetVendorProductTemplateQuestions(Guid productId)
        {
            QueryExpression qe = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns(), Distinct = true };
            LinkEntity leTemplateQuestion = new LinkEntity("vsp_question", "vsp_templatequestion", "vsp_questionid", "vsp_questionid", JoinOperator.Inner);
            LinkEntity leTemplate = new LinkEntity("vsp_templatequestion", "vsp_template", "vsp_templateid", "vsp_templateid", JoinOperator.Inner);
            leTemplate.LinkCriteria.AddCondition("vsp_templatetype", ConditionOperator.Equal, TemplateType.VendorProductTemplate);
            leTemplate.LinkCriteria.AddCondition("vsp_vspproductid", ConditionOperator.Equal, productId);

            leTemplateQuestion.LinkEntities.Add(leTemplate);
            qe.LinkEntities.Add(leTemplateQuestion);

            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest() { Query = qe, ReturnDynamicEntities = true };
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);
            return res.BusinessEntityCollection.BusinessEntities.Select(e => new Question((DynamicEntity)e)).OrderBy(d => d.SortOrder).ToList();
        }

    }
}
