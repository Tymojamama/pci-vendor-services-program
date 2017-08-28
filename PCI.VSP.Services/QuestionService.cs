using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services
{
    class QuestionService
    {
        //// Vendor Questions
        //List<Model.IAccountQuestion> GetVendorProfileQuestions(Guid accountId);
        //void UpdateVendorProfileQuestions(List<Model.VendorQuestion> vendorProfileQuestions, Guid contactId);
        //List<Model.IAccountQuestion> GetVendorProjectInquiries(Guid accountId, Guid contactId, Guid? projectVendorId);
        //void UpdateVendorQuestions(List<Model.IAccountQuestion> vendorQuestions, Guid contactId, Boolean answersConfirmed);
        //List<Model.IAccountQuestion> GetVendorProductQuestions(Guid vendorId, Guid contactId, Guid vendorProductId);

        //// Questions
        //List<Data.CRM.Model.QuestionCategory> GetQuestionCategories();
        //Data.CRM.Model.Question GetQuestion(Guid questionId);
        //void SaveQuestion(Data.CRM.Model.Question question);
        //Data.CRM.Model.Question GetInvestmentAssumptions(Guid projectVendorId);

        private AuthenticationRequest GetDefaultAuthRequest()
        {
            return new AuthenticationRequest()
            {
                Username = Data.Globals.CrmServiceSettings.Username,
                Password = Data.Globals.CrmServiceSettings.Password
            };
        }

        public void SaveQuestion(Data.CRM.Model.Question question)
        {
            Data.CRM.DataLogic.QuestionDataLogic qdl = new Data.CRM.DataLogic.QuestionDataLogic(GetDefaultAuthRequest());
            qdl.Update(question);
        }

        public Data.CRM.Model.Question GetQuestion(Guid questionId)
        {
            Data.CRM.DataLogic.QuestionDataLogic qdl = new Data.CRM.DataLogic.QuestionDataLogic(GetDefaultAuthRequest());
            return qdl.Retrieve(questionId);
        }

        public List<Data.CRM.Model.QuestionCategory> GetQuestionCategories()
        {
            PCI.VSP.Data.CRM.DataLogic.QuestionCategoryDataLogic qcdl = new Data.CRM.DataLogic.QuestionCategoryDataLogic(GetDefaultAuthRequest());
            return qcdl.RetrieveMultiple();
        }

        public List<Model.IAccountQuestion> GetVendorProductQuestions(Guid vendorId, Guid contactId, Guid vendorProductId)
        {
            PCI.VSP.Data.CRM.DataLogic.QuestionDataLogic qdl = new Data.CRM.DataLogic.QuestionDataLogic(GetDefaultAuthRequest());
            List<Data.CRM.Model.Question> ql = qdl.RetrieveByVendorProduct(vendorProductId);

            PCI.VSP.Data.CRM.DataLogic.VendorQuestionDataLogic vqdl = new Data.CRM.DataLogic.VendorQuestionDataLogic(GetDefaultAuthRequest());
            List<Data.CRM.Model.VendorQuestion> vqs = vqdl.RetrieveVendorProductQuestions(vendorId, vendorProductId, new PCI.VSP.Data.CRM.DataLogic.VendorProductDataLogic(GetDefaultAuthRequest()).Retrieve(vendorProductId).ProductId , false);
            if (vqs == null) { vqs = new List<Data.CRM.Model.VendorQuestion>(); }
            List<Model.IAccountQuestion> aql = vqs.Select<Data.CRM.Model.VendorQuestion, Model.IAccountQuestion>(vq => new Model.VendorQuestion(vq)).ToList();
            return aql;
        }

        public List<Model.IAccountQuestion> GetVendorProfileQuestions(Guid accountId)
        {
            PCI.VSP.Data.CRM.DataLogic.VendorQuestionDataLogic vqdl = new Data.CRM.DataLogic.VendorQuestionDataLogic(GetDefaultAuthRequest());
            List<Data.CRM.Model.VendorQuestion> vqs = vqdl.RetrieveVendorProfileQuestions(accountId);
            if (vqs == null) { return null; }

            IEnumerable<Model.IAccountQuestion> aqs = vqs.Select<Data.CRM.Model.VendorQuestion, Model.IAccountQuestion>(vq => new Model.VendorQuestion(vq));
            if (aqs == null) { return null; }

            // need to get templates and template questions associated with this thing
            PopulateQuestionCategoryIds(aqs);

            return aqs.ToList();
        }

        private void PopulateQuestionCategoryIds(IEnumerable<Model.IAccountQuestion> aql)
        {
            // need to get templates and template questions associated with this thing
            IEnumerable<Guid> qids = from Model.IAccountQuestion aq in aql
                                     select aq.QuestionId;

            Data.CRM.DataLogic.TemplateQuestionDataLogic tqdl = new Data.CRM.DataLogic.TemplateQuestionDataLogic(GetDefaultAuthRequest());
            List<Data.CRM.Model.TemplateQuestion> tql = tqdl.RetrieveByQuestionIds(qids);
            if (tql == null) { return; }

            foreach (Data.CRM.Model.TemplateQuestion tq in tql)
            {
                foreach (Model.IAccountQuestion aq in aql)
                {
                    if (aq.QuestionId == tq.QuestionId && aq.CategoryId == Guid.Empty)
                    {
                        aq.CategoryId = tq.QuestionCategoryId;
                        break;
                    }
                }
            }
        }

        public List<Model.IAccountQuestion> GetVendorProjectInquiries(Guid accountId, Guid contactId, Guid? projectVendorId)
        {
            // get client inquiries for this vendor
            PCI.VSP.Data.CRM.DataLogic.QuestionDataLogic qdl = new Data.CRM.DataLogic.QuestionDataLogic(GetDefaultAuthRequest());
            List<Data.CRM.Model.Question> ql = qdl.RetrieveProjectInquiriesByVendor(accountId, contactId, projectVendorId);
            if (ql == null) { return null; }

            // get qualified vendor questions
            PCI.VSP.Data.CRM.DataLogic.VendorQuestionDataLogic vqdl = new Data.CRM.DataLogic.VendorQuestionDataLogic(GetDefaultAuthRequest());

            List<Data.CRM.Model.VendorQuestion> vqs = vqdl.RetrieveByProjectInquiries(accountId, projectVendorId.Value);

            List<Data.CRM.Model.Question> missingQuestions = new List<Data.CRM.Model.Question>();
            missingQuestions.AddRange(ql);

            // merge the two lists
            // find questions that haven't been answered by the vendor
            if (vqs != null)
            {
                foreach (Data.CRM.Model.Question q in ql)
                {
                    foreach (Data.CRM.Model.VendorQuestion vq in vqs)
                    {
                        if (vq.QuestionId == q.QuestionId)
                        {
                            missingQuestions.Remove(q);
                            break;
                        }
                    }
                }
            }

            IEnumerable<Model.IAccountQuestion> aqs = vqs.Select<Data.CRM.Model.VendorQuestion, Model.IAccountQuestion>(vq => new Model.VendorQuestion(vq));
            List<Model.IAccountQuestion> aql = new List<Model.IAccountQuestion>();

            if (aqs != null)
                aql.AddRange(aqs);

            Guid vendorProductId = Guid.Empty;
            if (projectVendorId.HasValue)
            {
                // get vendorProductId
                Data.CRM.DataLogic.ProjectVendorDataLogic pvdl = new Data.CRM.DataLogic.ProjectVendorDataLogic(GetDefaultAuthRequest());
                Data.CRM.Model.ProjectVendor pv = pvdl.Retrieve(projectVendorId.Value);
                if (pv != null) 
                    vendorProductId = pv.VendorProductId;
            }

            Boolean hasInvestmentAssumptions = false;
            Boolean hasPlanInformation = false;
            foreach (Data.CRM.Model.Question missingQuestion in missingQuestions)
            {
                aql.Add(new Model.VendorQuestion(accountId, vendorProductId, missingQuestion));
                switch (missingQuestion.QuestionType)
                {
                    case Data.Enums.QuestionTypes.InvestmentAssumption:
                        hasInvestmentAssumptions = true;
                        break;
                    case Data.Enums.QuestionTypes.PlanAssumption:
                        hasPlanInformation = true;
                        break;
                }
            }

            // populate new investment assumptions from client questions
            if (hasInvestmentAssumptions && projectVendorId.HasValue && projectVendorId != Guid.Empty)
                PopulateInvestmentAssumptions(projectVendorId.Value, aql);

            // populate new plan information from client questions
            if (hasPlanInformation)
                PopulatePlanInformation(projectVendorId.Value, aql);
            
            return aql;
        }

        private void PopulatePlanInformation(Guid projectVendorId, List<Model.IAccountQuestion> aql)
        {
            Data.CRM.DataLogic.ClientQuestionDataLogic cqdl = new Data.CRM.DataLogic.ClientQuestionDataLogic(GetDefaultAuthRequest());
            List<Data.CRM.Model.ClientQuestion> cql = cqdl.RetrievePlanInformationByProjectVendorId(projectVendorId);
            if (cql == null) { return; }

            foreach (Data.CRM.Model.ClientQuestion cq in cql)
            {
                foreach (Model.IAccountQuestion aq in aql)
                {
                    if (aq.QuestionId == cq.QuestionId)
                    {
                        if (aq.AnswerType != Data.Enums.AnswerTypes.None) { break; }

                        aq.PlanAssumptionId = cq.PlanAssumptionId;
                        aq.Answer = cq.Answer;
                        aq.AlternateAnswer = cq.AlternateAnswer;
                        aq.MaximumAnswerAllowed = cq.MaximumAnswerAllowed;
                        aq.MinimumAnswerAllowed = cq.MinimumAnswerAllowed;
                        aq.ChoiceAnswers = cq.ChoiceAnswers;
                        break;  
                    }
                }
            }
        }

        private void PopulateInvestmentAssumptions(Guid projectVendorId, List<Model.IAccountQuestion> aql)
        {
            Data.CRM.DataLogic.ClientQuestionDataLogic cqdl = new Data.CRM.DataLogic.ClientQuestionDataLogic(GetDefaultAuthRequest());
            List<Data.CRM.Model.ClientQuestion> cql = cqdl.RetrieveInvestmentAssumptionsByProjectVendorId(projectVendorId);
            if (cql == null) { return; }

            foreach (Data.CRM.Model.ClientQuestion cq in cql)
            {
                foreach (Model.IAccountQuestion aq in aql)
                {
                    if (aq.QuestionId == cq.QuestionId)
                    {
                        //aq.AssetClass = cq.AssetClass;
                        aq.AssetClassId = cq.AssetClassId;
                        aq.AssetFund = cq.AssetFund;
                        aq.Assets = cq.Assets;
                        aq.AssetSymbol = cq.AssetSymbol;
                        aq.Participants = cq.Participants;
                        aq.AnnualContributions = cq.AnnualContributions;
                        break;
                    }
                }
            }
        }

        //public void UpdateVendorQuestions(List<Model.IAccountQuestion> vendorQuestions, Guid contactId, Boolean answersConfirmed)
        //{
        //    if (vendorQuestions == null || vendorQuestions.Count == 0) { return; }

        //    // get projectVendors via vendorQuestions
        //    Guid[] pvids = vendorQuestions.Select(vq => vq.Id).ToArray();
        //    Data.CRM.DataLogic.ProjectVendorDataLogic pvdl = new Data.CRM.DataLogic.ProjectVendorDataLogic(GetDefaultAuthRequest());
        //    List<Data.CRM.Model.ProjectVendor> pvl = pvdl.RetrieveMultipleByVendorQuestions(pvids);

        //    // save vendorQuestions
        //    Data.CRM.DataLogic.VendorQuestionDataLogic vqdl = new Data.CRM.DataLogic.VendorQuestionDataLogic(GetDefaultAuthRequest());
        //    List<Data.CRM.Model.VendorQuestion> vql = vendorQuestions.Select(vq => (Data.CRM.Model.VendorQuestion)vq).ToList();
        //    List<Guid> vendorProductIds = new List<Guid>();
        //    List<Guid> vendorIds = new List<Guid>();

        //    foreach (Data.CRM.Model.VendorQuestion question in vql)
        //    {
        //        question.InvalidAnswerReason = Data.Enums.InvalidAnswerReasons.Unspecified;
        //        question.AnswerRejectedReason = String.Empty;
        //        question.LastUpdated = DateTime.Now;

        //        if (question.VendorProductId != Guid.Empty && !vendorProductIds.Contains(question.VendorProductId))
        //            vendorProductIds.Add(question.VendorProductId);
        //        else if (question.VendorProductId == Guid.Empty && !vendorIds.Contains(question.VendorId))
        //            vendorIds.Add(question.VendorId);
        //    }
        //    vqdl.Save(vql, contactId);

        //    // update timestamps for all vendorProducts affected by these vendorQuestions
        //    Data.CRM.DataLogic.VendorProductDataLogic vpdl = new Data.CRM.DataLogic.VendorProductDataLogic(GetDefaultAuthRequest());
        //    foreach (Guid vendorProductId in vendorProductIds)
        //    {
        //        vpdl.UpdateTimestamps(vendorProductId, contactId);
        //    }

        //    // update timestamps for all vendor profiles (accounts) affected by these vendorQuestions
        //    Data.CRM.DataLogic.AccountDataLogic adl = new Data.CRM.DataLogic.AccountDataLogic(GetDefaultAuthRequest());
        //    foreach (Guid vendorId in vendorIds)
        //    {
        //        adl.UpdateVendorTimestamps(vendorId);
        //    }

        //    // update flags on projectVendors
        //    foreach (Data.CRM.Model.ProjectVendor pv in pvl)
        //    {
        //        pv.Status = Data.Enums.ProjectVendorStatuses.VendorApproved;
        //        pvdl.Save(pv);
        //    }
        //}
    }
}
