using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Plugins.Model
{
    public class ClientQuestion : EntityBase
    {
        private const String _entityName = "vsp_clientquestion";

        public ClientQuestion() : base(_entityName) { }
        public ClientQuestion(DynamicEntity e)
            : base(e)
        {
        }

        public ClientQuestion(Question q) : base(_entityName)
        {
            this.AnswerType = q.ClientAnswerType;
            this.ClientWording = q.ClientWording;
            this.Name = q.Name;
            this.QuestionDataType = q.QuestionDataType;
            this.QuestionType = q.QuestionType;
            this.QuestionId = q.QuestionId;
            this.MaximumAnswerAllowed = q.MaximumAnswerAllowed;
            this.MinimumAnswerAllowed = q.MinimumAnswerAllowed;
            this.ClientEntityName = q.ClientEntityName;
            this.AttributeName = q.AttributeName;
            this.DocumentTemplateId = q.DocumentTemplateId;
            this.ChoiceAnswers = q.ChoiceAnswers;
            this.VendorMonitoringType = q.VendorMonitoringType;
        }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_clientquestionid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_clientquestionid", PropertyType.Key, value); }
        }

        public new string Name
        {
            get { return base.GetPropertyValue<string>("vsp_name", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_name", PropertyType.String, value); }
        }

        public Guid ClientId
        {
            get { return base.GetPropertyValue<Guid>("vsp_accountid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_accountid", PropertyType.Lookup, value); }
        }

        public Guid ClientProjectId
        {
            get { return base.GetPropertyValue<Guid>("vsp_clientprojectid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_clientprojectid", PropertyType.Lookup, value); }
        }

        public Guid TemplateId
        {
            get { return base.GetPropertyValue<Guid>("vsp_templateid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_templateid", PropertyType.Lookup, value); }
        }

        public Guid QuestionId
        {
            get { return base.GetPropertyValue<Guid>("vsp_vspquestionid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_vspquestionid", PropertyType.Lookup, value); }
        }

        public Guid CategoryId
        {
            get { return base.GetPropertyValue<Guid>("vsp_questioncategoryid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_questioncategoryid", PropertyType.Lookup, value); }
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

        public string ClientWording
        {
            get { return base.GetPropertyValue<string>("vsp_clientwording", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_clientwording", PropertyType.String, value); }
        }

        public DateTime? AnswerExpirationDate
        {
            get
            {
                String crmDateString = base.GetPropertyValue<String>("vsp_answerexpirationdate", PropertyType.String, String.Empty);
                return base.StringDateToNullable(crmDateString);
            }
            set
            {
                String crmDateString = String.Empty;
                if (value.HasValue) { crmDateString = value.Value.ToShortDateString(); }
                base.SetPropertyValue<String>("vsp_answerexpirationdate", PropertyType.String, crmDateString);
            }
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

        public Enums.FilterCategory FilterCategory
        {
            get
            {
                bool fcb = base.GetPropertyValue<Boolean>("vsp_filtercategory", PropertyType.Bit, false);
                if (fcb)
                    return Enums.FilterCategory.Filter2;
                else
                    return Enums.FilterCategory.Filter2;
            }
            set
            {
                bool fcb = (value == Enums.FilterCategory.Filter2);
                base.SetPropertyValue<Boolean>("vsp_filtercategory", PropertyType.Bit, fcb);
            }
        }

        public Enums.QuestionTypes QuestionType
        {
            get { return base.GetPropertyValue<Enums.QuestionTypes>("vsp_questiontype", PropertyType.Picklist, Enums.QuestionTypes.Unspecified); }
            set { base.SetPropertyValue<Enums.QuestionTypes>("vsp_questiontype", PropertyType.Picklist, value); }
        }

        public Enums.VendorMonitoringAnswerTypes VendorMonitoringType
        {
            get { return base.GetPropertyValue<Enums.VendorMonitoringAnswerTypes>("vsp_vendormonitoringanswertype", PropertyType.Picklist, Enums.VendorMonitoringAnswerTypes.None); }
            set { base.SetPropertyValue<Enums.VendorMonitoringAnswerTypes>("vsp_vendormonitoringanswertype", PropertyType.Picklist, value); }
        }

        public String AssetClass
        {
            get { return base.GetPropertyValue<String>("vsp_assetclass", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_assetclass", PropertyType.String, value); }
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

        public Boolean VendorMustOfferFund
        {
            get { return base.GetPropertyValue<Boolean>("vsp_vendormustofferfund", PropertyType.Bit, false); }
            set { base.SetPropertyValue<Boolean>("vsp_vendormustofferfund", PropertyType.Bit, value); }
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

        public Enums.EntityName ClientEntityName
        {
            get { return base.GetPropertyValue<Enums.EntityName>("vsp_cliententityname", PropertyType.Picklist, Enums.EntityName.NotMapped); }
            set { base.SetPropertyValue<Enums.EntityName>("vsp_cliententityname", PropertyType.Picklist, value); }
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

        public String PCICommentToVendor
        {
            get { return base.GetPropertyValue<String>("vsp_pcicommenttovendor", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_pcicommenttovendor", PropertyType.String, value); }
        }

        public String ClientCommentToPCI
        {
            get { return base.GetPropertyValue<String>("vsp_clientcommenttopci", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_clientcommenttopci", PropertyType.String, value); }
        }

        public String PCICommentToClient
        {
            get { return base.GetPropertyValue<String>("vsp_pcicommenttoclient", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_pcicommenttoclient", PropertyType.String, value); }
        }

        public String ClientCommentToVendor
        {
            get { return base.GetPropertyValue<String>("vsp_clientcommenttovendor", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_clientcommenttovendor", PropertyType.String, value); }
        }

        public Guid DocumentTemplateId
        {
            get { return base.GetPropertyValue<Guid>("vsp_documenttemplateid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_documenttemplateid", PropertyType.Lookup, value); }
        }

        public Int32 Rank
        {
            get { return base.GetPropertyValue<Int32>("vsp_rank", PropertyType.Number, 0); }
            set { base.SetPropertyValue<Int32>("vsp_rank", PropertyType.Number, value); }
        }
    }
}
