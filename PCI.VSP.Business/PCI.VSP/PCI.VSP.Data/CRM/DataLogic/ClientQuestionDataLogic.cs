using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using PCI.VSP.Data.CRM.Model;
using Microsoft.Crm.SdkTypeProxy;
using PCI.VSP.Data.Classes;
using System.Xml.Linq;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class ClientQuestionDataLogic : ServiceObjectBase<Model.ClientQuestion, Guid>
    {
        public const String _entityName = "vsp_clientquestion";
        //private static String[] _columnSet = new String[] { "vsp_clientquestionid", "vsp_name", "vsp_accountid", "vsp_clientprojectid", "vsp_templateid", 
        //    "vsp_questionid", "vsp_questioncategoryid", "vsp_answertype", "vsp_questiondatatype", "vsp_answer", "vsp_altanswer", "vsp_choiceanswers",
        //    "vsp_clientwording", "vsp_answerexpirationdate"};

        public ClientQuestionDataLogic(Model.IAuthenticationRequest authRequest) : base(authRequest, _entityName, null) { }

        public List<Model.ClientQuestion> RetrieveForPhase1Filter(Guid clientProjectId)
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();

            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.In, new int[] { Convert.ToInt32(Enums.QuestionTypes.SearchQuestion_Filter1), Convert.ToInt32(Enums.QuestionTypes.PlanAssumption) });

            LinkEntity questionLink = new LinkEntity(_entityName, "vsp_question", "vsp_vspquestionid", "vsp_questionid", JoinOperator.Inner);
            questionLink.LinkCriteria = new FilterExpression();
            questionLink.LinkCriteria.AddCondition("vsp_vendoranswertype", ConditionOperator.NotEqual, 4);

            query.LinkEntities.Add(questionLink);
            try
            {
                List<DynamicEntity> des = base.RetrieveMultiple(query);
                if (des == null) { return null; }
                return des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).OrderBy(d => d.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                ex.Data.Add("clientProjectId", clientProjectId.ToString());
                throw;
            }
        }

        public List<Model.ClientQuestion> RetrieveForPhase2Filter(Guid clientProjectId)
        {
            List<ClientQuestion> cql = new List<ClientQuestion>();

            QueryExpression query = new QueryExpression(_entityName) { ColumnSet = new AllColumns() };
            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);

            LinkEntity questionLink = new LinkEntity(_entityName, "vsp_question", "vsp_vspquestionid", "vsp_questionid", JoinOperator.Inner);
            questionLink.LinkCriteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            query.LinkEntities.Add(questionLink);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des != null && des.Count > 0)
                cql.AddRange(des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).OrderBy(d => d.SortOrder).ToList());

            query = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns() };
            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.In, new int[] { Convert.ToInt32(Enums.QuestionTypes.SearchQuestion_Filter2), Convert.ToInt32(Enums.QuestionTypes.ProjectSpecificQuestion), Convert.ToInt32(Enums.QuestionTypes.InvestmentAssumption) });
            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            des = base.RetrieveMultiple(query);
            if (des != null && des.Count > 0)
                cql.AddRange(des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).OrderBy(d => d.SortOrder).ToList());

            return cql;
        }

        public List<ClientQuestion> RetrieveClientInquiryQuestions(Guid clientProjectId)
        {
            List<ClientQuestion> cql = new List<ClientQuestion>();
            List<Question> ql = new List<Question>();

            #region Retrieve all VSP Questions assigned to this specific project

            QueryExpression query = new QueryExpression("vsp_question") { ColumnSet = new AllColumns() };
            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des != null && des.Count > 0)
                ql.AddRange(des.Select<DynamicEntity, Model.Question>(q => new Model.Question(q)).OrderBy(d => d.SortOrder).ToList());

            #endregion

            #region Retrieve all of the Client Questions for this specific project

            query = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns() };
            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.In, new int[] { Convert.ToInt32(Enums.QuestionTypes.SearchQuestion_Filter2), Convert.ToInt32(Enums.QuestionTypes.ProjectSpecificQuestion), Convert.ToInt32(Enums.QuestionTypes.InvestmentAssumption), Convert.ToInt32(Enums.QuestionTypes.Fee) });
            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            des = base.RetrieveMultiple(query);
            if (des != null && des.Count > 0)
            {
                var cpQuestions = des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).OrderBy(d => d.SortOrder).ToList();

                #region Check if the Client Project VSP Questions are already a Client Question (it should be)

                for (int i = 0; i < ql.Count(); i++)
                {
                    var dupe = cpQuestions.Where(z => z.QuestionId == ql[i].QuestionId).FirstOrDefault();

                    if (dupe == null)
                        cql.Add(new ClientQuestion()
                        {
                            QuestionId = ql[i].QuestionId,
                            QuestionDataType = ql[i].QuestionDataType,
                            QuestionType = ql[i].QuestionType,
                            AnswerType = ql[i].ClientAnswerType,
                            ChoiceAnswers = ql[i].ChoiceAnswers,
                            ClientProjectId = ql[i].ClientProjectId,
                            DocumentTemplateId = ql[i].DocumentTemplateId,
                            Name = ql[i].Name,
                            MaximumAnswerAllowed = ql[i].MaximumAnswerAllowed,
                            MinimumAnswerAllowed = ql[i].MinimumAnswerAllowed,
                            PCICommentToClient = ql[i].PCICommentToClient,
                            PCICommentToVendor = ql[i].PCICommentToVendor,
                            PlanAssumptionId = ql[i].PlanAssumptionId,
                            Status = Enums.AccountQuestionStatuses.Answered,
                            AssetFund = ql[i].AssetFund,
                            AssetClassId = ql[i].AssetClassId,
                            AssetSymbol = ql[i].AssetSymbol,
                            AnnualContributions = ql[i].AnnualContributions,
                            Assets = ql[i].Assets,
                            Participants = ql[i].Participants,
                            SortOrder = (ql[i].SortOrder == 0) ? 1 : ql[i].SortOrder
                        });
                }

                #endregion

                cql.AddRange(cpQuestions.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).OrderBy(d => d.SortOrder).ToList());
            }

            #endregion

            return cql;
        }

        public List<Model.ClientQuestion> RetrieveClientInquiries(Guid clientProjectId)
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();

            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            LinkEntity questionLink = new LinkEntity(_entityName, "vsp_question", "vsp_vspquestionid", "vsp_questionid", JoinOperator.Inner);
            questionLink.LinkCriteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            query.LinkEntities.Add(questionLink);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0) { return null; }

            return des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).OrderBy(d => d.SortOrder).ToList();
        }

        public List<Model.ClientQuestion> RetrieveMultipleByClientProject(Guid clientProjectId, Boolean includeProjectSpecific)
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            if (!includeProjectSpecific)
            {
                LinkEntity questionLink = new LinkEntity(_entityName, "vsp_question", "vsp_vspquestionid", "vsp_questionid", JoinOperator.Inner);
                questionLink.LinkCriteria.AddCondition("vsp_clientprojectid", ConditionOperator.Null);
                query.LinkEntities.Add(questionLink);
            }

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0) { return null; }

            return des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).ToList();
        }

        public List<Model.ClientQuestion> FetchByClientProject(Guid clientProjectId, bool includeProjectSpecific)
        {
            string fetchXml =
@"<fetch version='1.0' output-format='xml-platform' mapping='logical'>
    <entity name='vsp_clientquestion'>
        <all-attributes />
        <link-entity name='vsp_template' from='vsp_templateid' to='vsp_templateid' alias='t2' link-type='inner'>
            <attribute name='vsp_templateid' />
            <attribute name='vsp_name' />
            <order attribute='vsp_name' descending='false' />
        </link-entity>
        <link-entity name='vsp_templatequestion' from='vsp_questionid' to='vsp_vspquestionid' alias='tq' link-type='inner'>
            <attribute name='vsp_sortorder' />
            <link-entity name='vsp_template' from='vsp_templateid' to='vsp_templateid' alias='t1' link-type='inner'>
                <attribute name='vsp_templateid' />
            </link-entity>
            <order attribute='vsp_sortorder' descending='false' />
        </link-entity>
        <filter type='and'>
            <condition attribute='vsp_clientprojectid' operator='eq' value='" + clientProjectId + @"' />
        </filter>
    </entity>
</fetch>";

            XDocument result = base.Fetch(fetchXml);
            return result.Descendants("result").Select(el => new Model.ClientQuestion(el)).Where(cq => cq.Attributes["t1.vsp_templateid"].Equals(cq.Attributes["t2.vsp_templateid"])).ToList();
        }

        public List<Model.ClientQuestion> RetrieveInvestmentAssumptions(Guid clientProjectId)
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.Equal, Convert.ToInt32(Enums.QuestionTypes.InvestmentAssumption));

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0) { return null; }

            return des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).OrderBy(d => d.SortOrder).ToList();
        }

        public List<Model.ClientQuestion> RetrieveInvestmentAssumptionsByProjectVendorId(Guid projectVendorId)
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.Equal, Convert.ToInt32(Enums.QuestionTypes.InvestmentAssumption));

            LinkEntity clientProjectLink = new LinkEntity(_entityName, "vsp_clientproject", "vsp_clientprojectid", "vsp_clientprojectid", JoinOperator.Inner);
            LinkEntity projectVendorLink = new LinkEntity("vsp_clientproject", "vsp_projectvendor", "vsp_clientprojectid", "vsp_clientprojectid", JoinOperator.Inner);
            projectVendorLink.LinkCriteria.AddCondition("vsp_projectvendorid", ConditionOperator.Equal, projectVendorId);
            clientProjectLink.LinkEntities.Add(projectVendorLink);

            query.LinkEntities.Add(clientProjectLink);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0) { return null; }

            return des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).OrderBy(d => d.SortOrder).ToList();
        }

        /// <summary>
        /// Retrieve Plan Information (Assumptions) by Client Project ID
        /// </summary>
        /// <param name="clientProjectId">Client Project ID</param>
        /// <returns>List of Client Questions (Plan Information/Assumptions)</returns>
        public List<Model.ClientQuestion> RetrievePlanInformation(Guid clientProjectId)
        {
            QueryExpression query = new QueryExpression(_entityName) { ColumnSet = new AllColumns() };
            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.Equal, Convert.ToInt32(Enums.QuestionTypes.PlanAssumption));
            query.LinkEntities.Add(new LinkEntity(_entityName, "vsp_clientproject", "vsp_clientprojectid", "vsp_clientprojectid", JoinOperator.Inner));

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0) { return null; }

            return des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).OrderBy(d => d.SortOrder).ToList();
        }

        public List<Model.ClientQuestion> RetrievePlanInformationByProjectVendorId(Guid projectVendorId)
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.Equal, Convert.ToInt32(Enums.QuestionTypes.PlanAssumption));

            LinkEntity clientProjectLink = new LinkEntity(_entityName, "vsp_clientproject", "vsp_clientprojectid", "vsp_clientprojectid", JoinOperator.Inner);
            LinkEntity projectVendorLink = new LinkEntity("vsp_clientproject", "vsp_projectvendor", "vsp_clientprojectid", "vsp_clientprojectid", JoinOperator.Inner);
            projectVendorLink.LinkCriteria.AddCondition("vsp_projectvendorid", ConditionOperator.Equal, projectVendorId);
            clientProjectLink.LinkEntities.Add(projectVendorLink);

            query.LinkEntities.Add(clientProjectLink);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0) { return null; }

            return des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).OrderBy(d => d.SortOrder).ToList();
        }

        public Model.ClientQuestion Retrieve(Guid clientQuestionId)
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            query.Criteria.AddCondition("vsp_clientquestionid", ConditionOperator.Equal, clientQuestionId);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0 || des.Count > 1) { return null; }

            return des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).First();
        }

        /// <summary>
        /// Retrieve Client Questions (not VSP Questions) by question type / template id
        /// </summary>
        /// <param name="clientProjectId">Client Project ID</param>
        /// <param name="type">Question Type</param>
        /// <param name="templateIds">Template IDs to use in query</param>
        /// <returns>List of Client Questions</returns>
        public List<Model.ClientQuestion> Retrieve(Guid clientProjectId, Enums.QuestionTypes type, List<Guid> templateIds = null)
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.Equal, Convert.ToInt32(type));

            if (templateIds != null)
                query.Criteria.AddCondition(DataConstants.vsp_templateid, ConditionOperator.In, templateIds.ToArray());

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0) { return null; }

            return des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).ToList();
        }

        public List<Model.ClientQuestion> Fetch(Guid clientProjectId, Enums.QuestionTypes type, List<Guid> templateIds = null)
        {
            string fetchXml =
@"<fetch version='1.0' output-format='xml-platform' mapping='logical'>
    <entity name='vsp_clientquestion'>
        <all-attributes />
        <link-entity name='vsp_template' from='vsp_templateid' to='vsp_templateid' alias='t2' link-type='inner'>
            <attribute name='vsp_templateid' />
            <attribute name='vsp_name' />
            <order attribute='vsp_name' descending='false' />
        </link-entity>
        <link-entity name='vsp_templatequestion' from='vsp_questionid' to='vsp_vspquestionid' alias='tq' link-type='inner'>
            <attribute name='vsp_sortorder' />
            <link-entity name='vsp_template' from='vsp_templateid' to='vsp_templateid' alias='t1' link-type='inner'>
                <attribute name='vsp_templateid' />
            </link-entity>
            <order attribute='vsp_sortorder' descending='false' />
        </link-entity>
        <filter type='and'>
            <condition attribute='vsp_clientprojectid' operator='eq' value='" + clientProjectId + @"' />
            <condition attribute='vsp_questiontype' operator='eq' value='" + Convert.ToInt32(type) + @"' />
        </filter>
    </entity>
</fetch>";

            XDocument result = base.Fetch(fetchXml);
            return result.Descendants("result").Select(el => new Model.ClientQuestion(el)).Where(cq => cq.Attributes["t1.vsp_templateid"].Equals(cq.Attributes["t2.vsp_templateid"])).ToList();
        }

        /// <summary>
        /// Get the Client Questions (not VSP Questions) for a client project, allowing a filter of Category and/or Function
        /// </summary>
        /// <param name="clientProjectId">Client Project ID</param>
        /// <param name="categoryId">Question Category ID</param>
        /// <param name="functionId">Question Function ID</param>
        /// <param name="questionTypes">Question Types INT Array</param>
        /// <returns>List of Client Questions</returns>
        public List<Model.ClientQuestion> Retrieve(Guid clientProjectId, Guid categoryId, Guid functionId, int[] questionTypes = null)
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);

            if (categoryId != Guid.Empty)
                query.Criteria.AddCondition("vsp_questioncategoryid", ConditionOperator.Equal, categoryId);
            else
                query.Criteria.AddCondition("vsp_questioncategoryid", ConditionOperator.Null);
            
            if (functionId != Guid.Empty)
                query.Criteria.AddCondition("vsp_questionfunctionid", ConditionOperator.Equal, functionId);

            if (questionTypes != null)
                query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.In, questionTypes);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0) { return null; }

            return des.Select<DynamicEntity, Model.ClientQuestion>(cq => new Model.ClientQuestion(cq)).OrderBy(d => d.SortOrder).ToList();
        }

        public void Save(ClientQuestion cq, annotation note)
        {
            if (cq == null) { return; }
            try
            {
                if (cq.Id != Guid.Empty)
                {
                    base.Update(cq);
                }
                else
                    cq.Id = base.Create(cq);

                if (note != null)
                {
                    note.objectid = new Lookup("vsp_clientquestionid", cq.Id);
                    note.objecttypecode = new EntityNameReference("vsp_clientquestion");
                    ServiceBroker.GetServiceInstance(base._authRequest).Create(note);
                }

                if ((cq.ClientEntityName != Enums.EntityName.NotMapped || cq.ClientEntityName == Enums.EntityName.Unspecified) && !string.IsNullOrEmpty(cq.AttributName) && cq.AttributeDataType != Enums.AttributeDataType.Unspecified)
                {
                    #region Data Point Update

                    DynamicEntity de = null;
                    switch (cq.ClientEntityName)
                    {
                        case Enums.EntityName.Account:
                            de = new DynamicEntity("account"); // MUST BE ALL LOWERCASE
                            de.Properties.Add(new KeyProperty("accountid", new Key(cq.ClientId))); // MUST BE ALL LOWERCASE, the column name is entity name + "id" so accountid or vendorproductid, etc
                            break;
                        case Enums.EntityName.ClientProject:
                            de = new DynamicEntity("clientproject"); // MUST BE ALL LOWERCASE
                            de.Properties.Add(new KeyProperty("clientprojectid", new Key(cq.ClientProjectId))); // MUST BE ALL LOWERCASE, the column name is entity name + "id" so accountid or vendorproductid, etc
                            break;
                    }
                    
                    switch (cq.AttributeDataType)
                    {
                        case Enums.AttributeDataType.nvarchar:
                            de.Properties.Add(new StringProperty(cq.AttributName, cq.Answer));
                            break;
                        case Enums.AttributeDataType.datetime:
                            de.Properties.Add(new CrmDateTimeProperty(cq.AttributName, new CrmDateTime(cq.Answer)));
                            break;
                        case Enums.AttributeDataType.bit:
                            de.Properties.Add(new CrmBooleanProperty(cq.AttributName, new CrmBoolean(bool.Parse(cq.Answer))));
                            break;
                        case Enums.AttributeDataType.decimaltype:
                            de.Properties.Add(new CrmDecimalProperty(cq.AttributName, new CrmDecimal(decimal.Parse(cq.Answer))));
                            break;
                        case Enums.AttributeDataType.integer:
                            de.Properties.Add(new CrmNumberProperty(cq.AttributName, new CrmNumber(int.Parse(cq.Answer))));
                            break;
                        case Enums.AttributeDataType.money:
                            de.Properties.Add(new CrmMoneyProperty(cq.AttributName, new CrmMoney(int.Parse(cq.Answer))));
                            break;
                    }

                    ServiceBroker.GetServiceInstance(_authRequest).Update(de);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("QuestionId", cq.QuestionId);
                ex.Data.Add("ClientId", cq.ClientId);
                ex.Data.Add("ClientProjectId", cq.ClientProjectId);
                ex.Data.Add("ClientEntityName", cq.ClientEntityName);
                ex.Data.Add("AttributName", cq.AttributName);
                ex.Data.Add("AttributeDataType", cq.AttributeDataType);
                throw;
            }
        }

        public List<Guid> RetrieveVendorMonitoringTemplateIDsUsedByClientProject(Guid clientProjectId)
        {
            List<Guid> result = new List<Guid>();
            QueryExpression query = new QueryExpression(_entityName) { ColumnSet = new AllColumns() };
            query.Criteria.AddCondition(DataConstants.vsp_clientprojectid, ConditionOperator.Equal, clientProjectId);
            query.Criteria.AddCondition(DataConstants.vsp_questiontype, ConditionOperator.Equal, Convert.ToInt32(Enums.QuestionTypes.VendorMonitoring));
 
            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des != null || des.Count > 0)
                foreach (var de in des)
                {
                    var clientQuestion = new Model.ClientQuestion(de);
                    if (clientQuestion.TemplateId != null && clientQuestion.TemplateId != Guid.Empty && !result.Contains(clientQuestion.TemplateId))
                        result.Add(clientQuestion.TemplateId);
                }

            return result;
        }
    }
}
