using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCI.VSP.Business.Entities
{
    /// <summary>
    /// Represents a vendor question that can interact with the database.
    /// </summary>
    public class VendorQuestion
    {
        public Guid CreatedBy { get; private set; }
        public Guid ModifiedBy { get; private set; }
        public Guid StatusId { get; set; }
        public Guid StatusReasonId { get; set; }
        public Guid VendorQuestionId { get; private set; }

        public Guid? AnswerType { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ClientQuestionId { get; set; }
        public Guid? DocumentTemplateId { get; set; }
        public Guid? FeeType { get; set; }
        public Guid? FunctionId { get; set; }
        public Guid? InvalidAnswerReason { get; set; }
        public Guid? QuestionId { get; set; }
        public Guid? QuestionTypeId { get; set; }
        public Guid? QuestionDataTypeId { get; set; }
        public Guid? ServiceProviderId { get; set; }
        public Guid? SubTaCalculationMethod { get; set; }
        public Guid? TemplateId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid? VendorMonitoringAnswerTypeId { get; set; }
        public Guid? VendorProductId { get; set; }

        public string AlternativeAnswer { get; set; }
        public string Answer { get; set; }
        public string ChoiceAnswers { get; set; }
        public string CommentVendorToClient { get; set; }
        public string CommentClientToVendor { get; set; }
        public string CommentCompanyToVendor { get; set; }
        public string CommentVendorToCompany { get; set; }
        public string InvestmentAssetClass { get; set; }
        public string InvestmentId { get; set; }
        public string InvestmentName { get; set; }
        public string MinimumAnswerAllowed { get; set; }
        public string MaximumAnswerAllowed { get; set; }
        public string Name { get; set; }
        public string RejectedAnswerReason { get; set; }
        public string RevenueSharingDocumentation { get; set; }
        public string RevenueSharingCompanyComment { get; set; }
        public string RevenueSharingVendorComment { get; set; }
        public string VendorWording { get; set; }

        public int? InvestmentAnnualContributions { get; set; }
        public int? InvestmentAssets { get; set; }
        public int? InvestmentParticipants { get; set; }

        public decimal? CommissionDepositBased { get; set; }
        public decimal? RevenueSharingAssetBased { get; set; }
        public decimal? RevenueSharingBasisPoints { get; set; }
        public decimal? ServiceFeeBasisPoints { get; set; }
        public decimal? SubTaBasisPoints { get; set; }
        public decimal? SubTaPerHead { get; set; }
        public decimal? WrapFee { get; set; }

        public bool? InvestmentInformationConfirmed { get; set; }
        public bool? RevenueSharingIsAvailable { get; set; }

        public DateTime CreatedOn { get; private set; }
        public DateTime ModifiedOn { get; private set; }

        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="VendorQuestion"/> that does not exist as a database record.
        /// </summary>
        public VendorQuestion()
        {
            VendorQuestionId = Guid.NewGuid();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new instance of <see cref="VendorQuestion"/> that has a related database record.
        /// </summary>
        /// <param name="vendorQuestionId">Used to get the VendorQuestion database record.</param>
        public VendorQuestion(Guid vendorQuestionId)
        {
            VendorQuestionId = vendorQuestionId;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a datatable with relevant <see cref="VendorQuestion"/> information.
        /// </summary>
        /// <param name="vendorQuestionId">Used to get the VendorQuestion database record.</param>
        /// <returns>Returns a DataTable with VendorQuestion data regarding the given vendorQuestionId.</returns>
        private DataTable GetVendorQuestionDetails(Guid vendorQuestionId)
        {
            throw new NotImplementedException();
        }
    }
}
