using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PCI.VSP.Services
{
    public interface IVspService
    {
        // Security
        Model.IUser ValidateUser(AuthenticationRequest ticketRequest);
        Boolean ResetUserPassword(ResetPasswordRequest rpr);
        Boolean ChangePassword(ChangePasswordRequest changePasswordRequest);
        Boolean ChangePasswordQuestionAndAnswer(ChangePasswordQuestionRequest cpqr);
        Model.IUser GetUser(String username);
        Model.IUser GetUser(Guid contactId);

        // Setup
        void Initialize(PCI.VSP.Data.CrmServiceSettings crmServiceSettings);

        // Vendor Agent Management
        List<Model.VendorAgent> GetAgentSummary(Guid accountId);
        Guid CreateAgent(Model.VendorAgent vendorAgent);
        void UpdateAgent(Model.VendorAgent vendorAgent);

        // Vendor Products
        List<PCI.VSP.Data.CRM.Model.VendorProduct> GetAgentProducts(Guid accountId, Guid contactId);
        //List<Model.IAccountQuestion> GetVendorProductQuestions(Guid vendorId, Guid contactId, Guid vendorProductId);
        void UpdateAgentProducts(Guid accountId, Guid contactId, IEnumerable<Guid> vendorProductIds);
        Data.CRM.Model.VendorProduct GetVendorProduct(Guid vendorProductId);
        List<PCI.VSP.Data.CRM.Model.VendorProduct> GetVendorProducts(Guid accountId);

        // Dashboard
        List<Model.VendorProjectInquirySummary> RetrieveVendorProjectInquirySummary(Guid accountId, Guid contactId);
        List<Model.VendorProductSummary> GetVendorProductDashboard(Guid accountId, Guid contactId);

        // Vendor Questions
        List<Model.IAccountQuestion> GetVendorProfileQuestions(Guid accountId);
        void UpdateVendorProfileQuestions(List<Model.VendorQuestion> vendorProfileQuestions, Guid contactId);
        List<Model.IAccountQuestion> GetVendorProjectInquiries(Guid accountId, Guid contactId, Guid? projectVendorId);
        void UpdateVendorQuestions(List<Model.IAccountQuestion> vendorQuestions, Guid contactId, Boolean answersConfirmed);
        //void SaveInvestmentAssumptions(Model.VendorQuestion vendorQuestion, Guid contactId);
        //void SaveRevenueSharing(Model.VendorQuestion vendorQuestion, Guid contactId);

        // Questions
        List<Data.CRM.Model.QuestionCategory> GetQuestionCategories();
        Data.CRM.Model.Question GetQuestion(Guid questionId);
        void SaveQuestion(Data.CRM.Model.Question question);
        //Data.CRM.Model.Question GetInvestmentAssumptions(Guid projectVendorId);

        // ProjectVendors
        List<Data.CRM.Model.ProjectVendor> GetProjectVendorsByClientProject(Guid clientProjectId);
        Data.CRM.Model.ProjectVendor GetProjectVendor(Guid projectVendorId);

        // Vendors
        Data.CRM.Model.Vendor GetVendor(Guid vendorId);

        // Filtering
        String PerformPhase1Filter(Guid clientProjectId);
        String PerformPhase2Filter(Guid clientProjectId);

        // Projects
        Data.CRM.Model.ClientProject GetClientProjectByProjectVendor(Guid projectVendorId);

    }

}
