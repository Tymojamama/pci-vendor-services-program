using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCI.VSP.Data.CRM.Model;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;
using System.Web.Services.Protocols;
using PCI.VSP.Data.Enums;
using PCI.VSP.Data.Classes;
using System.Xml.Linq;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class VendorQuestionDataLogic : ServiceObjectBase<VendorQuestion, Guid>
    {
        private static String[] _columnSet = new String[] { "vsp_categoryid", "vsp_name", "vsp_questiondatatypeid", "vsp_questionid", "vsp_templateid", "vsp_vendorid", 
            "vsp_vendorquestionid", "vsp_vendorwording", "vsp_vendorproductid, vsp_clientquestionid" };
        public const String _entityName = "vsp_vendorquestion";

        public VendorQuestionDataLogic(IAuthenticationRequest authRequest) : base(authRequest, _entityName, null) { }

        #region Vendor Profile

        /// <summary>
        /// Retrieve Vendor Profile Questions WITHOUT creating them
        /// </summary>
        /// <param name="vendorId">Account ID of the Vendor</param>
        /// <param name="categoryId">Category ID</param>
        /// <param name="functionId">Function ID</param>
        /// <returns>List of Vendor Questions</returns>
        public List<VendorQuestion> RetrieveVendorProfileQuestionsNoCreate(Guid vendorId, Guid? categoryId = null, Guid? functionId = null)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.Equal, Convert.ToInt32(Enums.QuestionTypes.SearchQuestion_Filter1));
            query.Criteria.AddCondition("vsp_vendorid", ConditionOperator.Equal, vendorId);
            query.Criteria.AddCondition("vsp_vendorproductid", ConditionOperator.Null);

            if (categoryId.HasValue && categoryId != Guid.Empty)
                query.Criteria.AddCondition("vsp_categoryid", ConditionOperator.Equal, categoryId);

            if (functionId.HasValue && functionId != Guid.Empty)
                query.Criteria.AddCondition("vsp_questionfunctionid", ConditionOperator.Equal, functionId);

            try
            {
                List<DynamicEntity> del = base.RetrieveMultiple(query);
                if (del == null) { return null; }
                return del.Select<DynamicEntity, VendorQuestion>(vq => new Model.VendorQuestion(vq)).OrderBy(d => d.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                ex.Data.Add("VendorId", vendorId.ToString());
                throw;
            }
        }

        /// <summary>
        /// Retrieve Vendor Profile Questions and creates vendor questions if needed
        /// </summary>
        /// <param name="id">Guid of Vendor</param>
        /// <returns>List of Vendor Profile Questions</returns>
        public List<VendorQuestion> RetrieveVendorProfileQuestions(Guid id)
        {
            List<VendorQuestion> vql = null;

            try
            {
                var ql = new QuestionDataLogic(_authRequest).GetVendorProfileTemplateQuestions();

                vql = RetrieveVendorQuestions(id, null, false, null, true, null);
                List<Model.Template> tl = new TemplateDataLogic(base._authRequest).RetrieveProfileTemplates();
                TemplateQuestionDataLogic tqdl = new TemplateQuestionDataLogic(base._authRequest);

                foreach (Model.Template t in tl)
                {
                    List<Model.TemplateQuestion> tql = tqdl.RetrieveByTemplateId(t.Id);
                    t.Questions = tql.OrderBy(d => d.SortOrder).ToList();
                }
                tqdl = null;

                #region Filter out vendor questions that do not have matches in the question list

                List<int> remove = new List<int>();

                for (int i = 0; i < vql.Count; i++)
                {
                    bool exists = false;

                    foreach (var q in ql)
                    {
                        exists = (vql[i].QuestionId == q.QuestionId);
                        if (exists)
                            break;
                    }

                    if (!exists)
                        remove.Add(i);
                }

                if (remove.Count > 0)
                    for (int i = remove.Count - 1; i >= 0; i--)
                        vql.RemoveAt(remove[i]);

                #endregion

                #region Create missing vendor questions in CRM

                foreach (var q in ql)
                {
                    bool exists = false;

                    foreach (var vq in vql)
                    {
                        exists = (vq.QuestionId == q.QuestionId);
                        if (exists)
                            break;
                    }

                    if (!exists)
                    {
                        Guid categoryId = Guid.Empty;
                        Guid functionId = Guid.Empty;
                        Guid templateId = Guid.Empty;
                        int sortOrder = 0;

                        foreach (Model.Template t in tl)
                        {
                            foreach (Model.TemplateQuestion tq in t.Questions)
                            {
                                if (tq.QuestionId == q.QuestionId)
                                {
                                    categoryId = tq.QuestionCategoryId;
                                    functionId = tq.QuestionFunctionId;
                                    templateId = tq.TemplateId;
                                    sortOrder = tq.SortOrder;
                                }
                            }
                        }

                        VendorQuestion new_vq = new VendorQuestion()
                        {
                            QuestionId = q.QuestionId,
                            VendorId = id,
                            Name = q.Name,
                            VendorWording = q.VendorWording,
                            QuestionDataType = q.QuestionDataType,
                            AnswerType = q.VendorAnswerType,
                            ChoiceAnswers = q.ChoiceAnswers,
                            QuestionType = q.QuestionType,
                            PCICommentToVendor = q.PCICommentToVendor,
                            MinimumAnswerAllowed = q.MinimumAnswerAllowed,
                            MaximumAnswerAllowed = q.MaximumAnswerAllowed,
                            VendorEntityName = q.VendorEntityName,
                            AttributeName = q.AttributeName,
                            DocumentTemplateId = q.DocumentTemplateId,
                            FeeType = q.FeeType,
                            TemplateId = templateId,
                            CategoryId = categoryId,
                            FunctionId = functionId,
                            SortOrder = sortOrder
                        };
                        //if (categoryId != Guid.Empty)
                        //    new_vq.CategoryId = categoryId;
                        //if (functionId != Guid.Empty)
                        //    new_vq.FunctionId = functionId;
                        //if (templateId != Guid.Empty)
                        //    new_vq.TemplateId = templateId;

                        new_vq.Id = Create(new_vq);
                        vql.Add(new_vq);
                    }
                }

                #endregion
            }
            catch (SoapException se)
            {
                se.Data.Add("Id", id.ToString());
                throw;
            }
            catch (Exception e)
            {
                e.Data.Add("Id", id.ToString());
                throw;
            }

            return vql.OrderBy(d => d.SortOrder).ToList();
        }

        /// <summary>
        /// Retrieve percentage of Vendor Profile questions completed
        /// </summary>
        /// <param name="id">Vendor ID GUID</param>
        /// <returns>percentage complete</returns>
        public float RetrieveVendorProfilePercentComplete(Guid id)
        {
            float percent = 0;

            try
            {
                var vql = RetrieveVendorProfileQuestions(id);
                int count = 0;

                foreach (var vq in vql)
                {
                    if (vq.InvalidAnswerReason <= 0)  // Answer is valid
                        if (!string.IsNullOrEmpty(vq.Answer) && (AnswerTypes)vq.AnswerType != AnswerTypes.Range)
                            count++;
                        else if (!string.IsNullOrEmpty(vq.Answer) && (AnswerTypes)vq.AnswerType == AnswerTypes.Range && !string.IsNullOrEmpty(vq.AlternateAnswer))
                            count++;
                }

                if (vql.Count > 0)
                    percent = (float)count / vql.Count;
            }
            catch (SoapException se)
            {
                if (!se.Data.Contains("Id"))
                    se.Data.Add("Id", id.ToString());
                throw;
            }
            catch (Exception e)
            {
                if (!e.Data.Contains("Id"))
                    e.Data.Add("Id", id.ToString());
                throw;
            }

            return percent;
        }

        #endregion

        /// <summary>
        /// Retrieves the Vendor Profile and Vendor Product Questions
        /// </summary>
        /// <param name="vendorId">Vendor ID</param>
        /// <param name="vendorProductId">Vendor Product ID</param>
        /// <returns>List of Vendor Question</returns>
        public List<VendorQuestion> RetrieveFilter1VendorQuestions(Guid vendorId, Guid vendorProductId)
        {
            List<VendorQuestion> result = new List<VendorQuestion>();

            QueryExpression query = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns() };

            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.In, new int[] { Convert.ToInt32(Enums.QuestionTypes.SearchQuestion_Filter1), Convert.ToInt32(Enums.QuestionTypes.PlanAssumption) });
            query.Criteria.AddCondition("vsp_vendorid", ConditionOperator.Equal, vendorId);
            query.Criteria.AddCondition("vsp_vendorproductid", ConditionOperator.Null);

            try
            {
                List<DynamicEntity> del = base.RetrieveMultiple(query);
                if (del != null)
                    result.AddRange(del.Select<DynamicEntity, VendorQuestion>(vq => new Model.VendorQuestion(vq)).OrderBy(d => d.SortOrder).ToList());
            }
            catch (Exception ex)
            {
                ex.Data.Add("VendorId", vendorId.ToString());
                throw;
            }

            query.Criteria.Conditions.Clear();
            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.In, new int[] { Convert.ToInt32(Enums.QuestionTypes.SearchQuestion_Filter1), Convert.ToInt32(Enums.QuestionTypes.PlanAssumption) });
            query.Criteria.AddCondition("vsp_vendorid", ConditionOperator.Equal, vendorId);
            query.Criteria.AddCondition("vsp_vendorproductid", ConditionOperator.Equal, vendorProductId);

            try
            {
                List<DynamicEntity> del = base.RetrieveMultiple(query);
                if (del != null)
                    result.AddRange(del.Select<DynamicEntity, VendorQuestion>(vq => new Model.VendorQuestion(vq)).OrderBy(d => d.SortOrder).ToList());
            }
            catch (Exception ex)
            {
                ex.Data.Add("VendorId", vendorId.ToString());
                ex.Data.Add("VendorProductId", vendorProductId.ToString());
                throw;
            }

            return result;
        }

        /// <summary>
        /// Retrieves the Vendor Questions for Filter 2
        /// </summary>
        /// <param name="vendorId">Vendor ID</param>
        /// <param name="vendorProductId">Vendor Product ID</param>
        /// <returns>List of Vendor Question</returns>
        public List<VendorQuestion> RetrieveFilter2VendorQuestions(Guid vendorId, Guid vendorProductId)
        {
            List<VendorQuestion> result = new List<VendorQuestion>();

            QueryExpression query = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns() };
            query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.In, new int[] { Convert.ToInt32(Enums.QuestionTypes.SearchQuestion_Filter2), Convert.ToInt32(Enums.QuestionTypes.ProjectSpecificQuestion), Convert.ToInt32(Enums.QuestionTypes.InvestmentAssumption) });
            query.Criteria.AddCondition("vsp_vendorid", ConditionOperator.Equal, vendorId);
            query.Criteria.AddCondition("vsp_vendorproductid", ConditionOperator.Equal, vendorProductId);

            try
            {
                List<DynamicEntity> del = base.RetrieveMultiple(query);
                if (del != null)
                    result.AddRange(del.Select<DynamicEntity, VendorQuestion>(vq => new Model.VendorQuestion(vq)).OrderBy(d => d.SortOrder).ToList());
            }
            catch (Exception ex)
            {
                ex.Data.Add("VendorId", vendorId.ToString());
                ex.Data.Add("VendorProductId", vendorProductId.ToString());
                throw;
            }

            return result;
        }

        #region Vendor Product

        public List<VendorProductSummary> GetVendorProductDashboard(Guid accountId, Guid contactId)
        {
            // get vendor products
            PCI.VSP.Data.CRM.DataLogic.VendorProductDataLogic vpdl = new Data.CRM.DataLogic.VendorProductDataLogic(base._authRequest);
            List<Data.CRM.Model.VendorProduct> vpl = vpdl.RetrieveAgentProducts(accountId, contactId);
            if (vpl == null || vpl.Count == 0) { return null; }

            List<VendorProductSummary> vpsl = new List<VendorProductSummary>();
            PCI.VSP.Data.CRM.DataLogic.QuestionDataLogic qdl = new Data.CRM.DataLogic.QuestionDataLogic(base._authRequest);
            PCI.VSP.Data.CRM.DataLogic.VendorQuestionDataLogic vqdl = new Data.CRM.DataLogic.VendorQuestionDataLogic(base._authRequest);

            foreach (Data.CRM.Model.VendorProduct vp in vpl)
            {
                List<Data.CRM.Model.Question> qs = qdl.RetrieveByVendorProductMin(vp.VendorProductId);
                if (qs == null || qs.Count == 0) { continue; }

                List<Data.CRM.Model.VendorQuestion> vqs = vqdl.RetrieveVendorProductQuestions(accountId, vp.VendorProductId, vp.ProductId, true);
                VendorProductSummary vps = new VendorProductSummary()
                {
                    VendorProductId = vp.VendorProductId,
                    ProductName = vp.VendorProductName,
                    LastUpdatedBy = vp.LastUpdatedBy,
                    LastUpdated = vp.LastUpdated
                };

                Decimal percentComplete = 0;
                if (vqs != null && vqs.Count > 0)
                {
                    int count = (from Data.CRM.Model.VendorQuestion vq in vqs
                                 where !String.IsNullOrWhiteSpace(vq.Answer)
                                    && (vq.AnswerType == Data.Enums.AnswerTypes.Range ? !string.IsNullOrEmpty(vq.AlternateAnswer) : true)
                                    && vq.LastUpdated > DateTime.Now.AddDays(-90.0)
                                    && vq.Status != AccountQuestionStatuses.Rejected
                                 select vq).Count();

                    percentComplete = Math.Round(Convert.ToDecimal(count / (float)qs.Count), 2, MidpointRounding.AwayFromZero);
                }
                vps.PercentComplete = percentComplete;
                vpsl.Add(vps);
            }
            return vpsl;
        }

        /// <summary>
        /// Retrieves Vendor Product Questions
        /// </summary>
        /// <param name="vendorId">Vendor Id</param>
        /// <param name="vendorProductId">Vendor Product Id</param>
        /// <param name="productId">Product Type Id</param>
        /// <param name="createMissingQuestions">Whether or not to create missing Vendor Questions</param>
        /// <returns>List of Vendor Questions</returns>
        public List<VendorQuestion> RetrieveVendorProductQuestions(Guid vendorId, Guid vendorProductId, Guid productId, bool createMissingQuestions = false)
        {
            List<VendorQuestion> vql = new List<VendorQuestion>();

            try
            {
                var ql = new QuestionDataLogic(_authRequest).GetVendorProductTemplateQuestions(productId);  // Retrieve VSP Questions

                vql = RetrieveVendorQuestions(vendorId, null, false, null, true, vendorProductId);          // Retrieve Vendor Questions for the Product

                #region Filter out vendor questions that do not have matches in the question list

                List<int> remove = new List<int>();

                for (int i = 0; i < vql.Count; i++)
                {
                    bool exists = false;

                    foreach (var q in ql)
                    {
                        exists = (vql[i].QuestionId == q.QuestionId);
                        if (exists)
                            break;
                    }

                    if (!exists)
                        remove.Add(i);
                }

                if (remove.Count > 0)
                    for (int i = remove.Count - 1; i >= 0; i--)
                        vql.RemoveAt(remove[i]);

                #endregion

                if (createMissingQuestions)
                {
                    #region Create missing vendor questions in CRM

                    List<Model.Template> tl = new TemplateDataLogic(base._authRequest).RetrieveTemplatesByProductId(productId);
                    TemplateQuestionDataLogic tqdl = new TemplateQuestionDataLogic(base._authRequest);

                    foreach (Model.Template t in tl)
                    {
                        List<Model.TemplateQuestion> tql = tqdl.RetrieveByTemplateId(t.Id);
                        t.Questions = tql.OrderBy(d => d.SortOrder).ToList();
                    }
                    tqdl = null;

                    foreach (var q in ql)
                    {
                        bool exists = false;

                        foreach (var vq in vql)
                        {
                            exists = (vq.QuestionId == q.QuestionId);
                            if (exists)
                                break;
                        }

                        if (!exists)
                        {
                            Guid categoryId = Guid.Empty;
                            Guid functionId = Guid.Empty;
                            Guid templateId = Guid.Empty;
                            int sortOrder = 0;

                            foreach (Model.Template t in tl)
                            {
                                foreach (Model.TemplateQuestion tq in t.Questions)
                                {
                                    if (tq.QuestionId == q.QuestionId)
                                    {
                                        categoryId = tq.QuestionCategoryId;
                                        functionId = tq.QuestionFunctionId;
                                        templateId = tq.TemplateId;
                                        sortOrder = tq.SortOrder;
                                    }
                                }
                            }

                            VendorQuestion new_vq = new VendorQuestion()
                            {
                                QuestionId = q.QuestionId,
                                VendorId = vendorId,
                                Name = q.Name,
                                VendorWording = q.VendorWording,
                                QuestionDataType = q.QuestionDataType,
                                AnswerType = q.VendorAnswerType,
                                ChoiceAnswers = q.ChoiceAnswers,
                                QuestionType = q.QuestionType,
                                PCICommentToVendor = q.PCICommentToVendor,
                                MinimumAnswerAllowed = q.MinimumAnswerAllowed,
                                MaximumAnswerAllowed = q.MaximumAnswerAllowed,
                                VendorEntityName = q.VendorEntityName,
                                AttributeName = q.AttributeName,
                                DocumentTemplateId = q.DocumentTemplateId,
                                FeeType = q.FeeType,
                                TemplateId = templateId,
                                CategoryId = categoryId,
                                FunctionId = functionId,
                                VendorProductId = vendorProductId,
                                SortOrder = sortOrder
                            };

                            new_vq.Id = Create(new_vq);
                            vql.Add(new_vq);
                        }
                    }

                    #endregion
                }
            }
            catch (SoapException se)
            {
                se.Data.Add("Id", vendorId.ToString());
                throw;
            }
            catch (Exception e)
            {
                e.Data.Add("Id", vendorId.ToString());
                throw;
            }

            return vql.OrderBy(d => d.SortOrder).ToList();
        }

        public List<VendorQuestion> RetrieveVendorProductQuestionsForFilter(Guid vendorId, Guid vendorProductId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            query.Criteria.AddCondition("vsp_vendorid", ConditionOperator.Equal, vendorId);
            query.Criteria.AddCondition("vsp_vendorproductid", ConditionOperator.Equal, vendorProductId);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }

            return des.Select<DynamicEntity, VendorQuestion>(vq => new VendorQuestion(vq)).OrderBy(d => d.SortOrder).ToList();
        }

        #endregion

        #region Project Inquiry

        /// <summary>
        /// Retrieve all of the Vendor Question Records that match up to the Client Questions (Search Question Filter 2, Investment Assumption, Project Specific Question, Fee) for a specific Client Project
        /// </summary>
        /// <param name="vendorId">Vendor ID</param>
        /// <param name="projectVendorId">Project Vendor ID</param>
        /// <param name="createMissingVendorQuestions">Whether or not to create the missing Vendor Questions</param>
        /// <returns>List of Vendor Question</returns>
        public List<VendorQuestion> RetrieveProjectInquiryQuestions(Guid vendorId, Guid projectVendorId, bool createMissingVendorQuestions)
        {
            var pv = new ProjectVendorDataLogic(_authRequest).Retrieve(projectVendorId);
            var cql = new ClientQuestionDataLogic(_authRequest).RetrieveClientInquiryQuestions(pv.ClientProjectId);
            var questionIds = cql.Select(z => z.QuestionId).Distinct();

            QueryExpression query = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns() };
            query.Criteria.AddCondition("vsp_questionid", ConditionOperator.In, questionIds.ToArray());
            query.Criteria.AddCondition("vsp_vendorproductid", ConditionOperator.Equal, pv.VendorProductId);
            List<DynamicEntity> del = base.RetrieveMultiple(query);
            var vql = del.Select<DynamicEntity, VendorQuestion>(vq => new Model.VendorQuestion(vq)).ToList();

            List<VendorQuestion> results = new List<VendorQuestion>();

            if (createMissingVendorQuestions)
            {
                #region Create Missing Vendor Question Records

                //foreach (var questionId in questionIds)
                //{
                foreach (var cq in cql)
                {
                    try
                    {
                        //var cq = cql.Where(z => z.QuestionId == questionId).FirstOrDefault();
                        //if (cq == null) continue; // If no client question found, move on

                        VendorQuestion vq;

                        if (cq.QuestionType == QuestionTypes.InvestmentAssumption || cq.QuestionType == QuestionTypes.Fee)
                            vq = vql.Where(z => z.QuestionId == cq.QuestionId && z.ClientQuestionId == cq.Id).FirstOrDefault();
                        else
                            vq = vql.Where(z => z.QuestionId == cq.QuestionId).FirstOrDefault();

                        //var vq = vql.Where(z => z.QuestionId == questionId).FirstOrDefault();
                        if (vq != null)
                        {
                            results.Add(vq);
                            continue; // If Vendor Question already exists, move on
                        }

                        var q = new QuestionDataLogic(_authRequest).Retrieve(cq.QuestionId);

                        VendorQuestion new_vq = new VendorQuestion()
                        {
                            QuestionId = q.QuestionId,
                            VendorId = vendorId,
                            VendorProductId = pv.VendorProductId,
                            Name = q.Name,
                            VendorWording = q.VendorWording,
                            QuestionDataType = q.QuestionDataType,
                            AnswerType = q.VendorAnswerType,
                            ChoiceAnswers = q.ChoiceAnswers,
                            QuestionType = q.QuestionType,
                            PCICommentToVendor = q.PCICommentToVendor,
                            MinimumAnswerAllowed = q.MinimumAnswerAllowed,
                            MaximumAnswerAllowed = q.MaximumAnswerAllowed,
                            VendorEntityName = q.VendorEntityName,
                            AttributeName = q.AttributeName,
                            DocumentTemplateId = q.DocumentTemplateId,
                            TemplateId = cq.TemplateId,
                            CategoryId = cq.CategoryId,
                            FunctionId = cq.FunctionId,
                            AssetFund = cq.AssetFund,
                            AssetClassId = cq.AssetClassId,
                            AssetSymbol = cq.AssetSymbol,
                            AnnualContributions = cq.AnnualContributions,
                            Assets = cq.Assets,
                            Participants = cq.Participants,
                            FeeType = q.FeeType,
                            SortOrder = (q.SortOrder == 0) ? 1 : q.SortOrder
                        };

                        if (new_vq.QuestionType == QuestionTypes.InvestmentAssumption || new_vq.QuestionType == QuestionTypes.Fee)
                            new_vq.ClientQuestionId = cq.Id;

                        new_vq.Id = Create(new_vq);
                        results.Add(new_vq);
                    }
                    catch (Exception)
                    {

                        //throw;
                    }
                }

                #endregion
            }

            return results;
        }

        #endregion

        public List<VendorQuestion> RetrieveByProjectInquiriesMin(Guid vendorId, Guid contactId, Guid? projectVendorId)
        {
            return RetrieveByProjectInquiries(vendorId, contactId, projectVendorId, new ColumnSet(new String[] { "vsp_vendorquestionid", "vsp_questionid", "vsp_vendorproductid" }));
        }

        public List<VendorQuestion> RetrieveByProjectInquiries(Guid vendorId, Guid contactId, Guid? projectVendorId)
        {
            return RetrieveByProjectInquiries(vendorId, contactId, projectVendorId, new AllColumns());
        }

        public List<VendorQuestion> RetrieveByProjectVendor(Guid projectVendorId)
        {
            QueryExpression query = GetProjectVendorExpression(projectVendorId);
            List<DynamicEntity> del = base.RetrieveMultiple(query);
            if (del == null) { return null; }
            return del.Select<DynamicEntity, VendorQuestion>(vq => new VendorQuestion(vq)).OrderBy(d => d.SortOrder).ToList();
        }

        private QueryExpression GetProjectVendorExpression(Guid projectVendorId)
        {
            // ClientProjectVendor --> VendorProduct --> VendorQuestion
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            //query.Criteria.AddCondition("vsp_questiontype", ConditionOperator.Equal, Convert.ToInt32(Enums.QuestionTypes.SearchQuestion));

            LinkEntity vendorProductLink = new LinkEntity(_entityName, "vsp_vendorproduct", "vsp_vendorproductid", "vsp_vendorproductid", JoinOperator.Inner);
            LinkEntity clientProjectVendorLink = new LinkEntity("vsp_vendorproduct", "vsp_projectvendor", "vsp_vendorproductid", "vsp_vendorproductid", JoinOperator.Inner);
            clientProjectVendorLink.LinkCriteria.AddCondition("vsp_projectvendorid", ConditionOperator.Equal, projectVendorId);

            query.LinkEntities.Add(vendorProductLink);
            vendorProductLink.LinkEntities.Add(clientProjectVendorLink);
            return query;
        }

        public List<VendorQuestion> RetrieveByProjectInquiries(Guid vendorId, Guid projectVendorId)
        {
            try
            {
                QueryExpression query = GetProjectInquiryExpressionFixed(vendorId, projectVendorId);
                List<DynamicEntity> des = base.RetrieveMultiple(query);
                if (des == null) { return null; }

                return des.Select<DynamicEntity, Model.VendorQuestion>(vq => new Model.VendorQuestion(vq)).OrderBy(d => d.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                ex.Data.Add("ProjectVendorId", projectVendorId.ToString());
                throw;
            }
        }

        private List<VendorQuestion> RetrieveByProjectInquiries(Guid vendorId, Guid contactId, Guid? projectVendorId, ColumnSetBase columnSet)
        {
            try
            {
                QueryExpression query = GetProjectInquiryExpressionFixed(vendorId, projectVendorId);
                List<DynamicEntity> des = base.RetrieveMultiple(query);
                if (des == null) { return null; }

                return des.Select<DynamicEntity, Model.VendorQuestion>(vq => new Model.VendorQuestion(vq)).OrderBy(d => d.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                ex.Data.Add("VendorId", vendorId);
                ex.Data.Add("ContactId", contactId);
                throw;
            }

        }

        private QueryExpression GetProjectInquiryExpressionFixed(Guid vendorId, Guid? projectVendorId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };

            FilterExpression fe = new FilterExpression();
            fe.AddCondition("vsp_vendorid", ConditionOperator.Equal, vendorId);
            fe.FilterOperator = LogicalOperator.And;
            query.Criteria.AddFilter(fe);

            FilterExpression cfe = new FilterExpression();
            cfe.FilterOperator = LogicalOperator.Or;
            fe.AddFilter(cfe);

            String[] notNullables = new String[] { "vsp_altanswer", "vsp_answer" };
            foreach (String notNullable in notNullables)
            {
                cfe.AddCondition(notNullable, ConditionOperator.NotNull);
                cfe.AddCondition(notNullable, ConditionOperator.NotEqual, String.Empty);
            }

            LinkEntity questionLink = new LinkEntity(_entityName, "vsp_question", "vsp_questionid", "vsp_questionid", JoinOperator.Inner);
            query.LinkEntities.Add(questionLink);

            LinkEntity clientProjectLink = new LinkEntity("vsp_question", "vsp_clientproject", "vsp_clientprojectid", "vsp_clientprojectid", JoinOperator.Inner);
            questionLink.LinkEntities.Add(clientProjectLink);

            LinkEntity projectVendorLink = new LinkEntity("vsp_clientproject", "vsp_projectvendor", "vsp_clientprojectid", "vsp_clientprojectid", JoinOperator.Inner);
            if (projectVendorId.HasValue)
                projectVendorLink.LinkCriteria.AddCondition("vsp_projectvendorid", ConditionOperator.Equal, projectVendorId.Value);
            clientProjectLink.LinkEntities.Add(projectVendorLink);

            return query;
        }

        private QueryExpression GetProjectInquiryExpression(Guid vendorId, Guid contactId, Guid? projectVendorId, ColumnSetBase columnSet)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = columnSet
            };

            FilterExpression fe = new FilterExpression();
            fe.FilterOperator = LogicalOperator.And;
            query.Criteria.AddFilter(fe);

            FilterExpression cfe = new FilterExpression();
            cfe.FilterOperator = LogicalOperator.Or;
            fe.AddFilter(cfe);

            String[] notNullables = new String[] { "vsp_altanswer", "vsp_answer" };
            foreach (String notNullable in notNullables)
            {
                cfe.AddCondition(notNullable, ConditionOperator.NotNull);
                cfe.AddCondition(notNullable, ConditionOperator.NotEqual, String.Empty);
            }

            // link to vendorProduct
            LinkEntity vendorProductLink = new LinkEntity(_entityName, "vsp_vendorproduct", "vsp_vendorproductid", "vsp_vendorproductid", JoinOperator.Inner);
            vendorProductLink.LinkCriteria.AddCondition("vsp_accountid", ConditionOperator.Equal, vendorId);
            query.LinkEntities.Add(vendorProductLink);

            // link to projectVendor
            LinkEntity projectVendorLink = new LinkEntity("vsp_vendorproduct", "vsp_projectvendor", "vsp_vendorproductid", "vsp_vendorproductid", JoinOperator.Inner);
            if (projectVendorId.HasValue && projectVendorId != Guid.Empty)
                projectVendorLink.LinkCriteria.AddCondition("vsp_projectvendorid", ConditionOperator.Equal, projectVendorId);
            vendorProductLink.LinkEntities.Add(projectVendorLink);

            // link to clientProject
            LinkEntity clientProjectLink = new LinkEntity("vsp_projectvendor", "vsp_clientproject", "vsp_clientprojectid", "vsp_clientprojectid", JoinOperator.Inner);
            projectVendorLink.LinkEntities.Add(clientProjectLink);

            // link to question
            LinkEntity questionLink = new LinkEntity("vsp_question", "vsp_clientproject", "vsp_clientprojectid", "vsp_clientprojectid", JoinOperator.Inner);
            clientProjectLink.LinkEntities.Add(questionLink);

            return query;
        }

        /// <summary>
        /// Retrieve Vendor Questions by VendorID, TemplateID and ProductID
        /// </summary>
        /// <param name="VendorID">GUID Vendor ID</param>
        /// <param name="TemplateID">Nullable GUID Template ID</param>
        /// <param name="ProductID">Nullable GUID Product ID</param>
        /// <returns>List of VendorQuestion</returns>
        public List<VendorQuestion> RetrieveVendorQuestions(Guid VendorID, Guid? vendorAgentId, Guid? TemplateID, Guid? ProductID)
        {
            QueryExpression qe = new QueryExpression();
            qe.EntityName = "vsp_vendorquestion";
            qe.ColumnSet = new AllColumns();
            qe.Criteria = new FilterExpression();
            qe.Criteria.Conditions.Add(new ConditionExpression("vsp_vendorid", ConditionOperator.Equal, new object[] { VendorID }));
            //qe.Criteria.AddCondition("vsp_questiontype", ConditionOperator.Equal, Convert.ToInt32(Enums.QuestionTypes.SearchQuestion));

            if (TemplateID.HasValue)
                qe.Criteria.Conditions.Add(new ConditionExpression("vsp_templateid", ConditionOperator.Equal, new object[] { TemplateID.Value }));
            else
                qe.Criteria.Conditions.Add(new ConditionExpression("vsp_templateid", ConditionOperator.Null));

            if (ProductID.HasValue)
                qe.Criteria.Conditions.Add(new ConditionExpression("vsp_vendorproductid", ConditionOperator.Equal, new object[] { ProductID.Value }));
            else
                qe.Criteria.Conditions.Add(new ConditionExpression("vsp_vendorproductid", ConditionOperator.Null));

            if (vendorAgentId.HasValue)
                qe.LinkEntities.Add(GetSecurityExpression(vendorAgentId.Value));

            LinkEntity questionLink = new LinkEntity(_entityName, "vsp_question", "vsp_questionid", "vsp_questionid", JoinOperator.Inner);
            questionLink.LinkCriteria.AddCondition("vsp_clientprojectid", ConditionOperator.Null);

            qe.LinkEntities.Add(questionLink);

            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest();
            rmr.Query = qe;
            rmr.ReturnDynamicEntities = true;
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);
            return res.BusinessEntityCollection.BusinessEntities.Select(e => new VendorQuestion((DynamicEntity)e)).OrderBy(d => d.SortOrder).ToList();
        }

        /// <summary>
        /// Retrieve Vendor Questions by VendorID, TemplateID and ProductID
        /// </summary>
        /// <param name="VendorID">Vendor ID</param>
        /// <param name="vendorAgentId">Contact ID</param>
        /// <param name="filterByTemplateId">Whether or not to filter by template</param>
        /// <param name="TemplateID">Template ID</param>
        /// <param name="filterByProductId">Whether or not to filter by Product</param>
        /// <param name="ProductID">Product ID</param>
        /// <returns></returns>
        public List<VendorQuestion> RetrieveVendorQuestions(Guid VendorID, Guid? vendorAgentId, bool filterByTemplateId, Guid? TemplateID, bool filterByProductId, Guid? ProductID)
        {
            QueryExpression qe = new QueryExpression();
            qe.EntityName = "vsp_vendorquestion";
            qe.ColumnSet = new AllColumns();
            qe.Criteria = new FilterExpression();
            qe.Criteria.Conditions.Add(new ConditionExpression("vsp_vendorid", ConditionOperator.Equal, new object[] { VendorID }));
            //qe.Criteria.AddCondition("vsp_questiontype", ConditionOperator.Equal, Convert.ToInt32(Enums.QuestionTypes.SearchQuestion));

            if (filterByTemplateId)
                if (TemplateID.HasValue)
                    qe.Criteria.Conditions.Add(new ConditionExpression("vsp_templateid", ConditionOperator.Equal, new object[] { TemplateID.Value }));
                else
                    qe.Criteria.Conditions.Add(new ConditionExpression("vsp_templateid", ConditionOperator.Null));

            if (filterByProductId)
                if (ProductID.HasValue)
                    qe.Criteria.Conditions.Add(new ConditionExpression("vsp_vendorproductid", ConditionOperator.Equal, new object[] { ProductID.Value }));
                else
                    qe.Criteria.Conditions.Add(new ConditionExpression("vsp_vendorproductid", ConditionOperator.Null));

            if (vendorAgentId.HasValue)
                qe.LinkEntities.Add(GetSecurityExpression(vendorAgentId.Value));

            LinkEntity questionLink = new LinkEntity(_entityName, "vsp_question", "vsp_questionid", "vsp_questionid", JoinOperator.Inner);
            questionLink.LinkCriteria.AddCondition("vsp_clientprojectid", ConditionOperator.Null);

            qe.LinkEntities.Add(questionLink);

            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest();
            rmr.Query = qe;
            rmr.ReturnDynamicEntities = true;
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);
            return res.BusinessEntityCollection.BusinessEntities.Select(e => new VendorQuestion((DynamicEntity)e)).OrderBy(d => d.SortOrder).ToList();
        }

        private LinkEntity GetSecurityExpression(Guid vendorAgentId)
        {
            LinkEntity vendorQuestionLink = new LinkEntity("vsp_vendorquestion", "vsp_vendorproduct", "vsp_vendorproductid", "vsp_vendorproductid", JoinOperator.Inner);
            LinkEntity vendorProductLink = new LinkEntity("vsp_vendorproduct", "vsp_vendorproduct_contact", "vsp_vendorproductid", "vsp_vendorproductid", JoinOperator.Inner);
            vendorQuestionLink.LinkEntities.Add(vendorProductLink);

            LinkEntity contactLink = new LinkEntity("vsp_vendorproduct_contact", "contact", "contactid", "contactid", JoinOperator.Inner);
            contactLink.LinkCriteria.AddCondition("contactid", ConditionOperator.Equal, vendorAgentId);
            vendorProductLink.LinkEntities.Add(contactLink);
            return vendorQuestionLink;
        }

        public void Save(List<VendorQuestion> vql, Guid contactId)
        {
            if (vql == null || vql.Count == 0) { return; }
            foreach (VendorQuestion vq in vql)
                this.Save(vq, contactId, null);
        }

        public void Save(VendorQuestion vq, Guid? contactId, annotation note)
        {
            if (vq == null) { return; }
            try
            {
                if (vq.Id != Guid.Empty)
                {
                    vq.LastUpdated = DateTime.UtcNow;
                    vq.Id = vq.Id; // make sure the underlying DynamicEntity has its ID...
                    base.Update(vq);
                }
                else
                    vq.Id = base.Create(vq);

                if (note != null)
                {
                    note.objectid = new Lookup("vsp_vendorquestionid", vq.Id);
                    note.objecttypecode = new EntityNameReference("vsp_vendorquestion");
                    ServiceBroker.GetServiceInstance(base._authRequest).Create(note);
                }

                #region Account/Vendor Product Attribute Update Functionality

                if (vq.VendorEntityName != Enums.EntityName.NotMapped && vq.VendorEntityName != Enums.EntityName.Unspecified && !string.IsNullOrEmpty(vq.AttributeName) && vq.AttributeDataType != AttributeDataType.Unspecified)
                {
                    DynamicEntity de = null;
                    switch (vq.VendorEntityName)
                    {
                        case Enums.EntityName.Account:
                            de = new DynamicEntity("account"); // MUST BE ALL LOWERCASE
                            de.Properties.Add(new KeyProperty("accountid", new Key(vq.VendorId)));
                            break;
                        case Enums.EntityName.VendorProduct:
                            de = new DynamicEntity("vendorproduct"); // MUST BE ALL LOWERCASE
                            de.Properties.Add(new KeyProperty("vendorproductid", new Key(vq.VendorProductId)));
                            break;
                    }
                    // MUST BE ALL LOWERCASE, the column name is entity name + "id" so accountid or vendorproductid, etc

                    switch (vq.AttributeDataType)
                    {
                        case Enums.AttributeDataType.nvarchar:
                            de.Properties.Add(new StringProperty("", vq.Answer));
                            break;
                        case Enums.AttributeDataType.datetime:
                            de.Properties.Add(new CrmDateTimeProperty(vq.AttributeName, new CrmDateTime(vq.Answer)));
                            break;
                        case Enums.AttributeDataType.bit:
                            de.Properties.Add(new CrmBooleanProperty(vq.AttributeName, new CrmBoolean(bool.Parse(vq.Answer))));
                            break;
                        case Enums.AttributeDataType.decimaltype:
                            de.Properties.Add(new CrmDecimalProperty(vq.AttributeName, new CrmDecimal(decimal.Parse(vq.Answer))));
                            break;
                        case Enums.AttributeDataType.integer:
                            de.Properties.Add(new CrmNumberProperty(vq.AttributeName, new CrmNumber(int.Parse(vq.Answer))));
                            break;
                        case Enums.AttributeDataType.money:
                            de.Properties.Add(new CrmMoneyProperty(vq.AttributeName, new CrmMoney(int.Parse(vq.Answer))));
                            break;
                    }

                    ServiceBroker.GetServiceInstance(_authRequest).Update(de);
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.Data.Add("QuestionId", vq.QuestionId);
                ex.Data.Add("VendorId", vq.VendorId);
                ex.Data.Add("VendorProductId", vq.VendorProductId);
                ex.Data.Add("ContactId", (contactId.HasValue ? contactId.Value : Guid.Empty));
                throw;
            }
        }

        public VendorQuestion Retrieve(Guid vendorQuestionId)
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            query.Criteria.AddCondition("vsp_vendorquestionid", ConditionOperator.Equal, vendorQuestionId);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null || des.Count == 0 || des.Count > 1) { return null; }

            return des.Select<DynamicEntity, Model.VendorQuestion>(vq => new Model.VendorQuestion(vq)).First();
        }

        public List<VendorQuestion> RetrieveVendorMonitoringQuestions(Guid VendorId, Guid categoryId, Guid functionId, VendorMonitoringAnswerTypes answerType, bool createIfMissing, TemplateType templateType, Guid clientProjectID, Guid serviceProviderId)
        {
            try
            {
                List<Template> usedTemplates = new TemplateDataLogic(_authRequest).RetrieveTemplatesUsedByClientProject(clientProjectID, templateType);

                var clientQuestions = new ClientQuestionDataLogic(_authRequest).Retrieve(clientProjectID, QuestionTypes.VendorMonitoring, usedTemplates.Select(z => z.Id).ToList());
                List<Guid> clientQuestionIds = clientQuestions.Select(z => z.Id).ToList();

                List<VendorQuestion> result = new List<VendorQuestion>();
                QueryExpression qe = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns(), Criteria = new FilterExpression() };
                qe.Criteria.Conditions.Add(new ConditionExpression(DataConstants.vsp_vendorid, ConditionOperator.Equal, new object[] { VendorId }));
                qe.Criteria.Conditions.Add(new ConditionExpression(DataConstants.vsp_clientquestionid, ConditionOperator.In, clientQuestionIds.ToArray()));
                qe.Criteria.Conditions.Add(new ConditionExpression(DataConstants.vsp_categoryid, ConditionOperator.Equal, categoryId));
                qe.Criteria.Conditions.Add(new ConditionExpression(DataConstants.vsp_questionfunctionid, ConditionOperator.Equal, functionId));
                qe.Criteria.Conditions.Add(new ConditionExpression(DataConstants.vsp_vendormonitoringanswertype, ConditionOperator.Equal, Convert.ToInt32(answerType)));
                qe.Criteria.Conditions.Add(new ConditionExpression(DataConstants.vsp_serviceproviderid, ConditionOperator.Equal, serviceProviderId));

                var des = base.RetrieveMultiple(qe);
                if (des != null && des.Count > 0)
                    foreach (var de in des)
                        result.Add(new Model.VendorQuestion(de));

                if (createIfMissing)
                {
                    var cqdl = new ClientQuestionDataLogic(_authRequest);
                    var qdl = new QuestionDataLogic(_authRequest);
                    var vqdl = new VendorQuestionDataLogic(_authRequest);

                    foreach (var clientQuestionId in clientQuestionIds)
                        if (!result.Any(z => z.ClientQuestionId == clientQuestionId))
                        {
                            var clientQuestion = cqdl.Retrieve(clientQuestionId);

                            if (answerType == VendorMonitoringAnswerTypes.None) continue;
                            if (answerType == VendorMonitoringAnswerTypes.Actual && clientQuestion.VendorMonitoringType == VendorMonitoringAnswerTypes.Estimate) continue;
                            if (answerType == VendorMonitoringAnswerTypes.Estimate && clientQuestion.VendorMonitoringType == VendorMonitoringAnswerTypes.Actual) continue;

                            var vspQuestion = qdl.Retrieve(clientQuestion.QuestionId);
                            var vendorQuestion = new VendorQuestion(vspQuestion);
                            vendorQuestion.VendorId = VendorId;
                            vendorQuestion.VendorMonitoringAnswerType = answerType;
                            vendorQuestion.CategoryId = categoryId;
                            vendorQuestion.FunctionId = functionId;
                            vendorQuestion.ClientQuestionId = clientQuestionId;
                            vendorQuestion.TemplateId = clientQuestion.TemplateId;
                            vendorQuestion.ServiceProviderId = serviceProviderId;

                            var id = vqdl.Create(vendorQuestion);
                            vendorQuestion.Id = id;
                            result.Add(vendorQuestion);
                        }
                }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public List<VendorQuestion> FetchVendorMonitoringQuestions(Guid VendorId, Guid categoryId, Guid functionId, VendorMonitoringAnswerTypes answerType, bool createIfMissing, TemplateType templateType, Guid clientProjectID, Guid serviceProviderId)
        {
            List<Template> usedTemplates = new TemplateDataLogic(_authRequest).RetrieveTemplatesUsedByClientProject(clientProjectID, templateType);

            var clientQuestions = new ClientQuestionDataLogic(_authRequest).Retrieve(clientProjectID, QuestionTypes.VendorMonitoring, usedTemplates.Select(z => z.Id).ToList());
            List<Guid> clientQuestionIds = clientQuestions.Select(z => z.Id).ToList();

            string fetchXml =
@"
<fetch version='1.0' output-format='xml-platform' mapping='logical'>
    <entity name='vsp_vendorquestion'>
        <all-attributes />
        <link-entity name='vsp_clientquestion' from='vsp_clientquestionid' to='vsp_clientquestionid' alias='cq' link-type='inner'>
            <attribute name='vsp_clientquestionid' />
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
        </link-entity>
        <filter type='and'>
            <condition attribute='vsp_vendorid' operator='eq' value='" + VendorId + @"' />
            <condition attribute='vsp_clientquestionid' operator='in'>"
              + string.Join("\n", clientQuestionIds.Select(q => "<value>" + q + "</value>")) +
            @"</condition>
            <condition attribute='vsp_categoryid' operator='eq' value='" + categoryId + @"' />
            <condition attribute='vsp_questionfunctionid' operator='eq' value='" + functionId + @"' />
            <condition attribute='vsp_vendormonitoringanswertype' operator='eq' value='" + Convert.ToInt32(answerType) + @"' />
            <condition attribute='vsp_serviceproviderid' operator='eq' value='" + serviceProviderId + @"' />
        </filter>
    </entity>
</fetch>";

            var results = base.Fetch(fetchXml).Descendants("result")
                .Select(el => new Model.VendorQuestion(el))
                .Where(vq => vq.Attributes["t1.vsp_templateid"].Equals(vq.Attributes["t2.vsp_templateid"]))
                .ToList();

            bool new_created = false;
            if (createIfMissing)
            {
                var cqdl = new ClientQuestionDataLogic(_authRequest);
                var qdl = new QuestionDataLogic(_authRequest);
                var vqdl = new VendorQuestionDataLogic(_authRequest);

                foreach (var clientQuestionId in clientQuestionIds)
                    if (!results.Any(z => z.ClientQuestionId == clientQuestionId))
                    {
                        var clientQuestion = cqdl.Retrieve(clientQuestionId);

                        if (answerType == VendorMonitoringAnswerTypes.None) continue;
                        if (answerType == VendorMonitoringAnswerTypes.Actual && clientQuestion.VendorMonitoringType == VendorMonitoringAnswerTypes.Estimate) continue;
                        if (answerType == VendorMonitoringAnswerTypes.Estimate && clientQuestion.VendorMonitoringType == VendorMonitoringAnswerTypes.Actual) continue;

                        var vspQuestion = qdl.Retrieve(clientQuestion.QuestionId);
                        var vendorQuestion = new VendorQuestion(vspQuestion);
                        vendorQuestion.VendorId = VendorId;
                        vendorQuestion.VendorMonitoringAnswerType = answerType;
                        vendorQuestion.CategoryId = categoryId;
                        vendorQuestion.FunctionId = functionId;
                        vendorQuestion.ClientQuestionId = clientQuestionId;
                        vendorQuestion.TemplateId = clientQuestion.TemplateId;
                        vendorQuestion.ServiceProviderId = serviceProviderId;

                        var id = vqdl.Create(vendorQuestion);
                        new_created = true;
                        vendorQuestion.Id = id;
                        results.Add(vendorQuestion);
                    }
            }

            if (new_created)
            {
                return base.Fetch(fetchXml).Descendants("result")
                    .Select(el => new Model.VendorQuestion(el))
                    .Where(vq => vq.Attributes["t1.vsp_templateid"].Equals(vq.Attributes["t2.vsp_templateid"]))
                    .ToList();
            }
            else
            {
                return results;
            }
        }
    }
}