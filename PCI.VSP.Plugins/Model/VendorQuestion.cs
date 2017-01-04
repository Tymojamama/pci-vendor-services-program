using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Plugins.Model
{
    public class VendorQuestion : EntityBase
    {
        public VendorQuestion() : base("vsp_vendorquestion") { }
        public VendorQuestion(DynamicEntity e)
            : base(e)
        {
        }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_vendorquestionid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_vendorquestionid", PropertyType.Key, value); }
        }

        public new string Name
        {
            get { return base.GetPropertyValue<string>("vsp_name", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_name", PropertyType.String, value); }
        }

        public Guid VendorId
        {
            get { return base.GetPropertyValue<Guid>("vsp_vendorid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_vendorid", PropertyType.Lookup, value); }
        }

        public Guid VendorProductId
        {
            get { return base.GetPropertyValue<Guid>("vsp_vendorproductid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_vendorproductid", PropertyType.Lookup, value); }
        }

        public Guid TemplateId
        {
            get { return base.GetPropertyValue<Guid>("vsp_templateid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_templateid", PropertyType.Lookup, value); }
        }

        public Guid QuestionId
        {
            get { return base.GetPropertyValue<Guid>("vsp_questionid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_questionid", PropertyType.Lookup, value); }
        }

        public Guid CategoryId
        {
            get { return base.GetPropertyValue<Guid>("vsp_categoryid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_categoryid", PropertyType.Lookup, value); }
        }

        public Guid FunctionId
        {
            get { return base.GetPropertyValue<Guid>("vsp_questionfunctionid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_questionfunctionid", PropertyType.Lookup, value); }
        }

        public Enums.AnswerTypes AnswerType
        {
            get { return base.GetPropertyValue<Enums.AnswerTypes>("vsp_answertype", PropertyType.Picklist, Enums.AnswerTypes.Unspecified); }
            set { base.SetPropertyValue<Enums.AnswerTypes>("vsp_answertype", PropertyType.Picklist, value); }
        }

        public Enums.DataTypes QuestionDataType
        {
            get { return base.GetPropertyValue<Enums.DataTypes>("vsp_questiondatatype", PropertyType.Picklist, Enums.DataTypes.Unspecified); }
            set { base.SetPropertyValue<Enums.DataTypes>("vsp_questiondatatype", PropertyType.Picklist, value); }
        }

        public string Answer
        {
            get { return base.GetPropertyValue<string>("vsp_answer", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_answer", PropertyType.String, value); }
        }

        public string AlternateAnswer
        {
            get { return base.GetPropertyValue<string>("vsp_altanswer", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_altanswer", PropertyType.String, value); }
        }

        public string ChoiceAnswers
        {
            get { return base.GetPropertyValue<string>("vsp_choiceanswers", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_choiceanswers", PropertyType.String, value); }
        }

        public string VendorWording
        {
            get { return base.GetPropertyValue<string>("vsp_vendorwording", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_vendorwording", PropertyType.String, value); }
        }

        public Enums.InvalidAnswerReasons InvalidAnswerReason
        {
            get { return base.GetPropertyValue<Enums.InvalidAnswerReasons>("vsp_invalidanswerreason", PropertyType.Picklist, Enums.InvalidAnswerReasons.Unspecified); }
            set { base.SetPropertyValue<Enums.InvalidAnswerReasons>("vsp_invalidanswerreason", PropertyType.Picklist, value); }
        }

        public string AnswerRejectedReason
        {
            get { return base.GetPropertyValue<string>("vsp_answerrejectedreason", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_answerrejectedreason", PropertyType.String, value); }
        }

        public DateTime? LastUpdated
        {
            get
            {
                return base.DateToNullable(base.GetPropertyValue<DateTime>("vsp_lastupdated", PropertyType.DateTime, DateTime.MinValue));
            }
            set
            {
                base.SetPropertyValue<DateTime>("vsp_lastupdated", PropertyType.DateTime, base.NullableToDate(value));
            }
        }

        public Enums.AccountQuestionStatuses Status
        {
            get { return base.GetPropertyValue<Enums.AccountQuestionStatuses>("statuscode", PropertyType.Status, Enums.AccountQuestionStatuses.Unspecified); }
            set { base.SetPropertyValue<Enums.AccountQuestionStatuses>("statuscode", PropertyType.Status, value); }
        }

        public Guid PlanAssumptionId
        {
            get { return base.GetPropertyValue<Guid>("vsp_planassumptionid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_planassumptionid", PropertyType.Lookup, value); }
        }

        public Enums.QuestionTypes QuestionType
        {
            get { return base.GetPropertyValue<Enums.QuestionTypes>("vsp_questiontype", PropertyType.Picklist, Enums.QuestionTypes.SearchQuestion_Filter1); }
            set { base.SetPropertyValue<Enums.QuestionTypes>("vsp_questiontype", PropertyType.Picklist, value); }
        }

        public String PCICommentToVendor
        {
            get { return base.GetPropertyValue<String>("vsp_pcicommenttovendor", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_pcicommenttovendor", PropertyType.String, value); }
        }

        public String VendorCommentToPCI
        {
            get { return base.GetPropertyValue<String>("vsp_vendorcommenttopci", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_vendorcommenttopci", PropertyType.String, value); }
        }

        public String VendorCommentToClient
        {
            get { return base.GetPropertyValue<String>("vsp_vendorcommenttoclient", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_vendorcommenttoclient", PropertyType.String, value); }
        }

        public String ClientCommentToVendor
        {
            get { return base.GetPropertyValue<String>("vsp_clientcommenttovendor", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_clientcommenttovendor", PropertyType.String, value); }
        }

        public Guid AssetClassId
        {
            get { return base.GetPropertyValue<Guid>("vsp_investmentassetclassid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_investmentassetclassid", PropertyType.Lookup, value); }
        }

        public String AssetFund
        {
            get { return base.GetPropertyValue<String>("vsp_assetfund", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_assetfund", PropertyType.String, value); }
        }

        public String AssetSymbol
        {
            get { return base.GetPropertyValue<String>("vsp_assetsymbol", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_assetsymbol", PropertyType.String, value); }
        }

        public Int32 Participants
        {
            get { return base.GetPropertyValue<Int32>("vsp_participants", PropertyType.Number, 0); }
            set { base.SetPropertyValue<Int32>("vsp_participants", PropertyType.Number, value); }
        }

        public Decimal Assets
        {
            get { return base.GetPropertyValue<Decimal>("vsp_assets", PropertyType.Decimal, 0m); }
            set { base.SetPropertyValue<Decimal>("vsp_assets", PropertyType.Decimal, value); }
        }

        public Decimal AnnualContributions
        {
            get { return base.GetPropertyValue<Decimal>("vsp_annualcontributions", PropertyType.Decimal, 0m); }
            set { base.SetPropertyValue<Decimal>("vsp_annualcontributions", PropertyType.Decimal, value); }
        }

        public Decimal CurrentBps
        {
            get { return base.GetPropertyValue<Decimal>("vsp_currentbps", PropertyType.Decimal, 0m); }
            set { base.SetPropertyValue<Decimal>("vsp_currentbps", PropertyType.Decimal, value); }
        }

        public Decimal CurrentFindersFee
        {
            get { return base.GetPropertyValue<Decimal>("vsp_currentfindersfee", PropertyType.Decimal, 0m); }
            set { base.SetPropertyValue<Decimal>("vsp_currentfindersfee", PropertyType.Decimal, value); }
        }

        public Decimal CurrentServiceFee
        {
            get { return base.GetPropertyValue<Decimal>("vsp_currentservicefee", PropertyType.Decimal, 0m); }
            set { base.SetPropertyValue<Decimal>("vsp_currentservicefee", PropertyType.Decimal, value); }
        }

        public Decimal OtherRevenueSharing
        {
            get { return base.GetPropertyValue<Decimal>("vsp_otherrevenuesharing", PropertyType.Decimal, 0m); }
            set { base.SetPropertyValue<Decimal>("vsp_otherrevenuesharing", PropertyType.Decimal, value); }
        }

        public Boolean RevenueSharingAlternativeAvailable
        {
            get { return base.GetPropertyValue<Boolean>("vsp_revenuesharingalternativeavailable", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_revenuesharingalternativeavailable", PropertyType.Bit, value); }
        }

        public Enums.RevenueSharingCalculationTypes RevenueSharingCalculationType
        {
            get { return base.GetPropertyValue<Enums.RevenueSharingCalculationTypes>("vsp_revenuesharingcalculationtype", PropertyType.Picklist, Enums.RevenueSharingCalculationTypes.Unspecified); }
            set { base.SetPropertyValue<Enums.RevenueSharingCalculationTypes>("vsp_revenuesharingcalculationtype", PropertyType.Picklist, value); }
        }

        public String RevenueSharingDocumentation
        {
            get { return base.GetPropertyValue<String>("vsp_revenuesharingdocumentation", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_revenuesharingdocumentation", PropertyType.String, value); }
        }

        public Boolean RevenueSharingIsAvailable
        {
            get { return base.GetPropertyValue<Boolean>("vsp_revenuesharingisavailable", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_revenuesharingisavailable", PropertyType.Bit, value); }
        }

        public String RevenueSharingPciComment
        {
            get { return base.GetPropertyValue<String>("vsp_revenuesharingpcicomment", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_revenuesharingpcicomment", PropertyType.String, value); }
        }

        public String RevenueSharingVendorComment
        {
            get { return base.GetPropertyValue<String>("vsp_revenuesharingvendorcomment", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_revenuesharingvendorcomment", PropertyType.String, value); }
        }

        public Decimal TotalRevenueSharing
        {
            get { return base.GetPropertyValue<Decimal>("vsp_totalrevenuesharing", PropertyType.Decimal, 0m); }
            set { base.SetPropertyValue<Decimal>("vsp_totalrevenuesharing", PropertyType.Decimal, value); }
        }

        public Boolean InvestmentAssumptionsConfirmed
        {
            get { return base.GetPropertyValue<Boolean>("vsp_investmentassumptionsconfirmed", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_investmentassumptionsconfirmed", PropertyType.Bit, value); }
        }


        /// <summary>
        /// The minimum value the answer (single or range) the answer can be.  Only works for integer or currency/money answer types
        /// </summary>
        public String MinimumAnswerAllowed
        {
            get { return base.GetPropertyValue<String>("vsp_minimumanswerallowed", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_minimumanswerallowed", PropertyType.String, value); }
        }

        /// <summary>
        /// The maximum value the answer (single or range) the answer can be.  Only works for integer or currency/money answer types
        /// </summary>
        public String MaximumAnswerAllowed
        {
            get { return base.GetPropertyValue<String>("vsp_maximumanswerallowed", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_maximumanswerallowed", PropertyType.String, value); }
        }

        public Enums.EntityName VendorEntityName
        {
            get { return base.GetPropertyValue<Enums.EntityName>("vsp_vendorentityname", PropertyType.Picklist, Enums.EntityName.NotMapped); }
            set { base.SetPropertyValue<Enums.EntityName>("vsp_vendorentityname", PropertyType.Picklist, value); }
        }

        public String AttributeName
        {
            get { return base.GetPropertyValue<String>("vsp_attributename", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_attributename", PropertyType.String, value); }
        }

        public Enums.AttributeDataType AttributeDataType
        {
            get { return base.GetPropertyValue<Enums.AttributeDataType>("vsp_attributedatatype", PropertyType.Picklist, Enums.AttributeDataType.Unspecified); }
            set { base.SetPropertyValue<Enums.AttributeDataType>("vsp_attributedatatype", PropertyType.Picklist, value); }
        }

        public Int32 SortOrder
        {
            get { return base.GetPropertyValue<Int32>("vsp_sortorder", PropertyType.Number, 0); }
            set { base.SetPropertyValue<Int32>("vsp_sortorder", PropertyType.Number, value); }
        }

        public Guid DocumentTemplateId
        {
            get { return base.GetPropertyValue<Guid>("vsp_documenttemplateid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_documenttemplateid", PropertyType.Lookup, value); }
        }

        public Enums.FeeType FeeType
        {
            get { return base.GetPropertyValue<Enums.FeeType>("vsp_feetype", PropertyType.Picklist, Enums.FeeType.Unspecified); }
            set { base.SetPropertyValue<Enums.FeeType>("vsp_feetype", PropertyType.Picklist, value); }
        }

        public Guid ClientQuestionId
        {
            get { return base.GetPropertyValue<Guid>("vsp_clientquestionid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_clientquestionid", PropertyType.Lookup, value); }
        }
    }
}
