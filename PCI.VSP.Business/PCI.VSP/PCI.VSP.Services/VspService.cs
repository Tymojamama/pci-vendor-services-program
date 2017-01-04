using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;
using PCI.VSP.Data.Enums;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Services;
using PCI.VSP.Services.Filtering;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;

namespace PCI.VSP.Services
{
    public class VspService : IVspService
    {
        public VspService()
        {
#if DEBUG && TRACE_FILTER
            Trace.Listeners.Clear();
            TraceListener tl = new TextWriterTraceListener(@"C:\Logs\VspFilterLog.log");
            Trace.Listeners.Add(tl);
#endif
        }

        public void Initialize(PCI.VSP.Data.CrmServiceSettings crmServiceSettings)
        {
            PCI.VSP.Data.Globals.Initialize(crmServiceSettings);
        }

        private AuthenticationRequest GetDefaultAuthRequest()
        {
            return new AuthenticationRequest()
            {
                Username = Data.Globals.CrmServiceSettings.Username,
                Password = Data.Globals.CrmServiceSettings.Password
            };
        }

        //public void SaveRevenueSharing(Model.VendorQuestion vendorQuestion, Guid contactId)
        //{
        //    //new QuestionService().SaveInvestmentAssumptions(vendorQuestion, contactId);
        //}

        public Data.CRM.Model.ClientProject GetClientProjectByProjectVendor(Guid projectVendorId)
        {
            Data.CRM.DataLogic.ProjectVendorDataLogic pvdl = new Data.CRM.DataLogic.ProjectVendorDataLogic(GetDefaultAuthRequest());
            Data.CRM.Model.ProjectVendor pv = pvdl.Retrieve(projectVendorId);
            if (pv == null) { return null; }

            Data.CRM.DataLogic.ClientProjectDataLogic cpdl = new Data.CRM.DataLogic.ClientProjectDataLogic(GetDefaultAuthRequest());
            Data.CRM.Model.ClientProject cp = cpdl.Retrieve(pv.ClientProjectId);
            return cp;
        }

        #region User Management

        public Model.IUser ValidateUser(AuthenticationRequest authRequest)
        {
            return new SecurityService().ValidateUser(authRequest);
        }

        public Model.IUser GetUser(String username)
        {
            return new SecurityService().GetUser(username);
        }

        public Boolean ResetUserPassword(ResetPasswordRequest rpr)
        {
            return new SecurityService().ResetUserPassword(rpr);
        }

        public Boolean ChangePassword(ChangePasswordRequest cpr)
        {
            return new SecurityService().ChangePassword(cpr);
        }

        private Model.IUser ValidateContact(AuthenticationRequest authRequest)
        {
            Data.CRM.DataLogic.ContactDataLogic cdl = new Data.CRM.DataLogic.ContactDataLogic(authRequest);
            Data.CRM.Model.Contact c = cdl.Retrieve(authRequest.Username, authRequest.Password);
            if (c == null) { return null; }
            return ConvertContactToIUser(c);
        }

        private Model.IUser ConvertContactToIUser(Data.CRM.Model.Contact c)
        {
            Model.IUser user = null;
            if (c.GetType() == typeof(Data.CRM.Model.VendorAgent))
            {
                Data.CRM.Model.VendorAgent va = c as Data.CRM.Model.VendorAgent;
                if (va.IsAdmin)
                    user = new Model.VendorAdmin(va);
                else
                    user = new Model.VendorAgent(va);
            }
            else if (c.GetType() == typeof(Data.CRM.Model.ClientRep))
                user = new Model.ClientRep((Data.CRM.Model.ClientRep)c);
            return user;
        }

        public Boolean ChangePasswordQuestionAndAnswer(ChangePasswordQuestionRequest cpqr)
        {
            return new SecurityService().ChangePasswordQuestionAndAnswer(cpqr);
        }

        #endregion

        #region Vendor Contact/Agent Management

        /// <summary>
        /// Gets a list of Vendor Contacts "Agents"
        /// </summary>
        /// <param name="accountId">Vendor ID</param>
        /// <returns>List of Vendor Agent</returns>
        public List<Model.VendorAgent> GetAgentSummary(Guid accountId)
        {
            PCI.VSP.Data.CRM.Model.Contact[] cs = new Data.CRM.DataLogic.ContactDataLogic(GetDefaultAuthRequest()).RetrieveByAccount(accountId);
            if (cs == null) return null;

            List<Model.VendorAgent> vas = new List<Model.VendorAgent>();
            foreach (PCI.VSP.Data.CRM.Model.Contact c in cs)
                vas.Add(new Model.VendorAgent((PCI.VSP.Data.CRM.Model.VendorAgent)c));
            return vas;
        }

        /// <summary>
        /// Create new Vendor Agent, returns Vendor Agent ID
        /// </summary>
        /// <param name="vendorAgent">Vendor Agent</param>
        /// <returns>Vendor Agent ID</returns>
        public Guid CreateAgent(Model.VendorAgent vendorAgent)
        {
            return new Data.CRM.DataLogic.ContactDataLogic(GetDefaultAuthRequest()).Create(vendorAgent);
        }

        /// <summary>
        /// Update existing Vendor Agent
        /// </summary>
        /// <param name="vendorAgent">Vendor Agent</param>
        public void UpdateAgent(Model.VendorAgent vendorAgent)
        {
            new Data.CRM.DataLogic.ContactDataLogic(GetDefaultAuthRequest()).Update(vendorAgent);
        }

        /// <summary>
        /// Retrieves a list of Vendor Agent Products
        /// </summary>
        /// <param name="accountId">Vendor ID</param>
        /// <param name="contactId">Contact ID</param>
        /// <returns>List of Vendor Products</returns>
        public List<PCI.VSP.Data.CRM.Model.VendorProduct> GetAgentProducts(Guid accountId, Guid contactId)
        {
            return new Data.CRM.DataLogic.VendorProductDataLogic(GetDefaultAuthRequest()).RetrieveAgentProducts(accountId, contactId);
        }

        public void UpdateAgentProducts(Guid accountId, Guid contactId, IEnumerable<Guid> vendorProductIds)
        {
            new VendorService().UpdateAgentProducts(accountId, contactId, vendorProductIds);
        }

        #endregion

        //public List<Model.IAccountQuestion> GetVendorProductQuestions(Guid vendorId, Guid contactId, Guid vendorProductId)
        //{
        //    return new QuestionService().GetVendorProductQuestions(vendorId, contactId, vendorProductId);
        //}

        public List<Model.IAccountQuestion> GetVendorProfileQuestions(Guid accountId)
        {
            return new QuestionService().GetVendorProfileQuestions(accountId);
        }

        public void UpdateVendorProfileQuestions(List<Model.VendorQuestion> vendorProfileQuestions, Guid contactId)
        {
            PCI.VSP.Data.CRM.DataLogic.VendorQuestionDataLogic vqdl = new Data.CRM.DataLogic.VendorQuestionDataLogic(GetDefaultAuthRequest());
            List<Data.CRM.Model.VendorQuestion> vpql;

            vpql = vendorProfileQuestions.Select<Model.VendorQuestion, Data.CRM.Model.VendorQuestion>(vpq => (Data.CRM.Model.VendorQuestion)vpq).ToList();

            foreach (Data.CRM.Model.VendorQuestion question in vpql)
            {
                question.LastUpdated = DateTime.Now;
                question.InvalidAnswerReason = Data.Enums.InvalidAnswerReasons.Unspecified;
                question.AnswerRejectedReason = String.Empty;

                // TODO: Put Note Logic Here
                annotation note = null;
                var aq = question as Model.IAccountQuestion;
                if (!string.IsNullOrEmpty(aq.NoteFileName) && !string.IsNullOrEmpty(aq.NoteBase64Data))
                    note = new annotation { filename = aq.NoteFileName, documentbody = aq.NoteBase64Data };

                vqdl.Save(question, contactId, note);
            }
        }

        public void ConfirmProjectVendor(ConfirmProjectVendorRequest request)
        {
            if (request.ConfirmVendorQuestions)
                ConfirmVendorQuestions(request.ProjectVendorId, request.ConfirmationState);

            // update the projectVendor
            Data.CRM.Model.ProjectVendor pv = new Data.CRM.Model.ProjectVendor() { Id = request.ProjectVendorId };
            if (request.ConfirmVendorQuestions)
                pv.Status = Data.Enums.ProjectVendorStatuses.VendorApproved;
            else
                pv.Status = Data.Enums.ProjectVendorStatuses.ClientApproved;

            Data.CRM.DataLogic.ProjectVendorDataLogic pvdl = new Data.CRM.DataLogic.ProjectVendorDataLogic(GetDefaultAuthRequest());
            pvdl.Update(pv);
        }

        private void ConfirmVendorQuestions(Guid projectVendorId, Boolean confirmationState)
        {
            // if updateVendorQuestions, update the status on all related vendorQuestions
            Data.CRM.DataLogic.VendorQuestionDataLogic vqdl = new Data.CRM.DataLogic.VendorQuestionDataLogic(GetDefaultAuthRequest());
            List<Data.CRM.Model.VendorQuestion> vql = vqdl.RetrieveByProjectVendor(projectVendorId);
            if (vql == null || vql.Count == 0) { return; }

            foreach (Data.CRM.Model.VendorQuestion vq in vql)
            {
                if (confirmationState)
                    vq.Status = Data.Enums.AccountQuestionStatuses.AccountConfirmed;
                else
                    vq.Status = Data.Enums.AccountQuestionStatuses.Answered;
                vqdl.Update(vq);
            }
        }

        public List<Model.IAccountQuestion> GetVendorProjectInquiries(Guid accountId, Guid contactId, Guid? projectVendorId)
        {
            //return new QuestionService().GetVendorProjectInquiries(accountId, contactId, projectVendorId);
            List<Model.IAccountQuestion> aql = new List<Model.IAccountQuestion>();
            var vql = new VendorQuestionDataLogic(GetDefaultAuthRequest()).RetrieveProjectInquiryQuestions(accountId, projectVendorId.Value, true);
            aql.AddRange(vql.Select<Data.CRM.Model.VendorQuestion, Model.IAccountQuestion>(vq => new Model.VendorQuestion(vq)).ToList());
            if (projectVendorId.HasValue)
            {
                var cql = new ClientQuestionDataLogic(GetDefaultAuthRequest()).RetrievePlanInformationByProjectVendorId(projectVendorId.Value);
                if (cql != null)
                    aql.AddRange(cql.Select<Data.CRM.Model.ClientQuestion, Model.IAccountQuestion>(cq => new Model.ClientQuestion(cq)).ToList());
            }
            return aql;
        }

        public List<Model.VendorProductSummary> GetVendorProductDashboard(Guid accountId, Guid contactId)
        {
            //return new VendorService().GetVendorProductDashboard(accountId, contactId);
            return null;
        }

        public List<Data.CRM.Model.QuestionCategory> GetQuestionCategories()
        {
            return new QuestionService().GetQuestionCategories();
        }

        public Model.IUser GetUser(Guid contactId)
        {
            return new SecurityService().GetUser(contactId);
        }

        #region Vendor Methods

        public Data.CRM.Model.Vendor GetVendor(Guid vendorId)
        {
            return new Data.CRM.DataLogic.AccountDataLogic(GetDefaultAuthRequest()).Retrieve(vendorId) as Data.CRM.Model.Vendor;
        }

        #endregion

        #region Project Vendors

        public Data.CRM.Model.ProjectVendor GetProjectVendor(Guid projectVendorId)
        {
            return new Data.CRM.DataLogic.ProjectVendorDataLogic(GetDefaultAuthRequest()).Retrieve(projectVendorId);
        }

        public List<Data.CRM.Model.ProjectVendor> GetProjectVendorsByClientProject(Guid clientProjectId)
        {
            Data.CRM.DataLogic.ProjectVendorDataLogic pvdl = new Data.CRM.DataLogic.ProjectVendorDataLogic(GetDefaultAuthRequest());
            List<Data.CRM.Model.ProjectVendor> pvl = pvdl.RetrieveMultipleByClientProject(clientProjectId);
            return pvl;
        }

        #endregion

        #region Vendor Products

        /// <summary>
        /// Retrieve a specific Vendor Product by ID
        /// </summary>
        /// <param name="vendorProductId">Vendor Product ID</param>
        /// <returns>Vendor Product</returns>
        public Data.CRM.Model.VendorProduct GetVendorProduct(Guid vendorProductId)
        {
            return new Data.CRM.DataLogic.VendorProductDataLogic(GetDefaultAuthRequest()).Retrieve(vendorProductId);
        }

        /// <summary>
        /// Retrieve all Vendor Products by Vendor
        /// </summary>
        /// <param name="accountId">Vendor ID</param>
        /// <returns>List of Vendor Product</returns>
        public List<Data.CRM.Model.VendorProduct> GetVendorProducts(Guid accountId)
        {
            return new Data.CRM.DataLogic.VendorProductDataLogic(GetDefaultAuthRequest()).RetrieveVendorProductByVendor(accountId);
        }

        #endregion

        #region Question Methods

        /// <summary>
        /// Method to get an IAccountQuestion, only used for Vendor and Client Questions
        /// </summary>
        /// <param name="id">ID for the Vendor or Client Question</param>
        /// <param name="qe">Enumeration of the Question Entity</param>
        /// <returns>IAccountQuestion</returns>
        public PCI.VSP.Services.Model.IAccountQuestion GetIAccountQuestion(Guid id, QuestionEntity qe)
        {
            switch (qe)
            {
                case QuestionEntity.VendorQuestion:
                    return ((Model.IAccountQuestion)new Model.VendorQuestion(new VendorQuestionDataLogic(GetDefaultAuthRequest()).Retrieve(id)));
                    //return (Model.IAccountQuestion)new VendorQuestionDataLogic(GetDefaultAuthRequest()).Retrieve(id, null);
                case QuestionEntity.ClientQuestion:
                    return ((Model.IAccountQuestion)new Model.ClientQuestion(new ClientQuestionDataLogic(GetDefaultAuthRequest()).Retrieve(id)));
                    //return (Model.IAccountQuestion)new ClientQuestionDataLogic(GetDefaultAuthRequest()).Retrieve(id);
                default:
                    return null;
            }
        }

        public void SaveQuestion(Data.CRM.Model.Question question)
        {
            new QuestionService().SaveQuestion(question);
        }

        public Data.CRM.Model.Question GetQuestion(Guid questionId)
        {
            return new QuestionService().GetQuestion(questionId);
        }

        #endregion

        #region Vendor Question Methods

        public PCI.VSP.Data.CRM.Model.VendorQuestion GetVendorQuestion(Guid id)
        {
            return (PCI.VSP.Data.CRM.Model.VendorQuestion)new VendorQuestionDataLogic(GetDefaultAuthRequest()).Retrieve(id, null);
        }

        public void UpdateVendorQuestions(List<Model.IAccountQuestion> vendorQuestions, Guid contactId, Boolean answersConfirmed)
        {
            if (vendorQuestions == null || vendorQuestions.Count == 0 || contactId == Guid.Empty) { return; }
            List<Guid> vendorProductIds = new List<Guid>();
            List<Guid> vendorIds = new List<Guid>();
            
            foreach (Data.CRM.Model.VendorQuestion question in vendorQuestions.Select(vq => (Data.CRM.Model.VendorQuestion)vq).ToList())
            {
                #region Update Each Vendor Question

                question.InvalidAnswerReason = Data.Enums.InvalidAnswerReasons.Unspecified;
                question.AnswerRejectedReason = String.Empty;
                question.LastUpdated = DateTime.Now;
                annotation note = null;

                var aq = question as Model.IAccountQuestion;
                if (!string.IsNullOrEmpty(aq.NoteFileName) && !string.IsNullOrEmpty(aq.NoteBase64Data))
                    note = new annotation { filename = aq.NoteFileName, documentbody = aq.NoteBase64Data };

                if (question.VendorProductId != Guid.Empty && !vendorProductIds.Contains(question.VendorProductId))
                    vendorProductIds.Add(question.VendorProductId);
                else if (question.VendorProductId == Guid.Empty && !vendorIds.Contains(question.VendorId))
                    vendorIds.Add(question.VendorId);

                new Data.CRM.DataLogic.VendorQuestionDataLogic(GetDefaultAuthRequest()).Save(question, contactId, note);

                #endregion
            }
            
            // update timestamps for all vendorProducts affected by these vendorQuestions
            Data.CRM.DataLogic.VendorProductDataLogic vpdl = new Data.CRM.DataLogic.VendorProductDataLogic(GetDefaultAuthRequest());
            foreach (Guid vendorProductId in vendorProductIds)
                vpdl.UpdateTimestamps(vendorProductId, contactId);

            // update timestamps for all vendor profiles (accounts) affected by these vendorQuestions
            Data.CRM.DataLogic.AccountDataLogic adl = new Data.CRM.DataLogic.AccountDataLogic(GetDefaultAuthRequest());
            foreach (Guid vendorId in vendorIds)
                adl.UpdateVendorTimestamps(vendorId);

            //// get projectVendors via vendorQuestions
            //Guid[] pvids = vendorQuestions.Select(vq => vq.Id).ToArray();
            //List<Data.CRM.Model.ProjectVendor> pvl = new Data.CRM.DataLogic.ProjectVendorDataLogic(GetDefaultAuthRequest()).RetrieveMultipleByVendorQuestions(pvids);

            //// update flags on projectVendors
            //foreach (Data.CRM.Model.ProjectVendor pv in pvl)
            //{
            //    pv.Status = Data.Enums.ProjectVendorStatuses.VendorApproved;
            //    pvdl.Save(pv);
            //}
        }

        /// <summary>
        /// Save Vendor Question Comment Change
        /// </summary>
        /// <param name="vendorQuestion">IAccountQuestion (Vendor Question)</param>
        /// <param name="contactId">Contact/User ID</param>
        public void UpdateVendorQuestionComment(Model.IAccountQuestion vendorQuestion, Guid? contactId)
        {
            new VendorQuestionDataLogic(GetDefaultAuthRequest()).Save(vendorQuestion as Data.CRM.Model.VendorQuestion, contactId, null);
        }

        #endregion

        #region Client Question

        /// <summary>
        /// Method to save Client Question
        /// </summary>
        /// <param name="clientQuestions">IAccountQuestions (Client Question)</param>
        public void UpdateClientQuestion(Model.IAccountQuestion clientQuestion)
        {
            ClientQuestionDataLogic cqdl = new ClientQuestionDataLogic(GetDefaultAuthRequest());
            var q = (PCI.VSP.Data.CRM.Model.ClientQuestion)clientQuestion;
            q.InvalidAnswerReason = Data.Enums.InvalidAnswerReasons.Unspecified;
            q.AnswerRejectedReason = String.Empty;
            annotation note = null;

            if (!string.IsNullOrEmpty(clientQuestion.NoteFileName) && !string.IsNullOrEmpty(clientQuestion.NoteBase64Data))
                note = new annotation { filename = clientQuestion.NoteFileName, documentbody = clientQuestion.NoteBase64Data };

            cqdl.Save(q, note);
        }

        /// <summary>
        /// Method to save Client Questions
        /// </summary>
        /// <param name="clientQuestions">List of IAccountQuestions (Client Questions)</param>
        public void UpdateClientQuestions(List<Model.IAccountQuestion> clientQuestions)
        {
            ClientQuestionDataLogic cqdl = new ClientQuestionDataLogic(GetDefaultAuthRequest());

            foreach (var cq in clientQuestions)
            {
                var q = (PCI.VSP.Data.CRM.Model.ClientQuestion)cq;
                q.InvalidAnswerReason = Data.Enums.InvalidAnswerReasons.Unspecified;
                q.AnswerRejectedReason = String.Empty;
                annotation note = null;
                
                if (!string.IsNullOrEmpty(cq.NoteFileName) && !string.IsNullOrEmpty(cq.NoteBase64Data))
                    note = new annotation { filename = cq.NoteFileName, documentbody = cq.NoteBase64Data };

                cqdl.Save(q, note);
            }
        }

        #endregion

        #region Vendor Portal UI Data Sets

        /// <summary>
        /// Retrieve the Project Inquiry data set for use on the Dashboard
        /// </summary>
        /// <param name="accountId">Vendor ID</param>
        /// <param name="contactId">Vendor Contact ID</param>
        /// <returns>List of VendorProjectInquirySummary</returns>
        public List<Model.VendorProjectInquirySummary> RetrieveVendorProjectInquirySummary(Guid accountId, Guid contactId)
        {
            Dictionary<Guid, Data.CRM.Model.VendorProduct> vpd = new Data.CRM.DataLogic.VendorProductDataLogic(GetDefaultAuthRequest()).RetrieveAgentProducts(accountId, contactId).ToDictionary(z => z.VendorProductId);
            if (vpd == null || vpd.Count == 0) { return null; }

            List<Data.CRM.Model.ProjectVendor> pvl = new Data.CRM.DataLogic.ProjectVendorDataLogic(GetDefaultAuthRequest()).RetrieveMultiple(accountId, contactId);
            if (pvl == null || pvl.Count == 0) { return null; }

            List<Model.VendorProjectInquirySummary> vcis = new List<Model.VendorProjectInquirySummary>();

            foreach (Data.CRM.Model.ProjectVendor pv in pvl)
            {
                if (vpd.ContainsKey(pv.VendorProductId) && !pv.Excluded)
                {
                    Data.CRM.Model.VendorProduct vp = vpd[pv.VendorProductId];
                    List<Model.IAccountQuestion> questions = new QuestionService().GetVendorProjectInquiries(accountId, contactId, pv.Id);

                    Model.VendorProjectInquirySummary vci = new Model.VendorProjectInquirySummary()
                    {
                        ClientProjectId = pv.ClientProjectId,
                        ProductName = vp.VendorProductName,
                        ProjectVendorId = pv.Id,
                        Status = Data.Enums.ProjectVendorStatusesConvertor.ToString(pv.Status),
                        LastUpdated = vp.LastUpdated,
                        VendorProductId = vp.VendorProductId,
                        ClientProjectName = pv.Name
                    };

                    vcis.Add(vci);
                }
            }
            return vcis;
        }

        #endregion

        #region Filter Processes

        public String PerformPhase1Filter(Guid clientProjectId)
        {
            FilterService _filterService = new FilterService(clientProjectId, Filter.FilterPhases.Phase1);
            return _filterService.PerformFilter();
        }

        public String PerformPhase2Filter(Guid clientProjectId)
        {
            FilterService _filterService = new FilterService(clientProjectId, Filter.FilterPhases.Phase2);
            return _filterService.PerformFilter();
        }

        #endregion
    }

    public class AuthenticationRequest : PCI.VSP.Data.CRM.Model.IAuthenticationRequest
    {
        public string Username { get; set; }
        public System.Security.SecureString Password { get; set; }
        public string DomainName { get; set; }
        public string CrmTicket { get; set; }
        public string OrganizationName { get; set; }
        public bool WasRefreshed { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string Username { get; set; }
        public System.Security.SecureString OldPassword { get; set; }
        public System.Security.SecureString NewPassword { get; set; }
    }

    public class ChangePasswordQuestionRequest
    {
        public String Username { get; set; }
        public System.Security.SecureString Password { get; set; }
        public String PasswordQuestion { get; set; }
        public System.Security.SecureString PasswordAnswer { get; set; }
    }

    public class ResetPasswordRequest
    {
        public String Username { get; set; }
        public System.Security.SecureString SecurityAnswer { get; set; }
        public System.Security.SecureString NewPassword { get; set; }
    }

    public class ConfirmProjectVendorRequest
    {
        public Boolean ConfirmationState { get; set; }
        public Guid ProjectVendorId { get; set; }
        public Boolean ConfirmVendorQuestions { get; set; }
    }
}
