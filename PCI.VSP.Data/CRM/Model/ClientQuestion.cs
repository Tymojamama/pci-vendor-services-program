using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using PCI.VSP.Data.Classes;
using System.Xml.Linq;

namespace PCI.VSP.Data.CRM.Model
{
    [Serializable]
    public class ClientQuestion : EntityBase, IEquatable<ClientQuestion>
    {
        public ClientQuestion() : base(DataConstants.vsp_clientquestion) { }
        public ClientQuestion(DynamicEntity e)
            : base(e)
        {
        }

        public ClientQuestion(EntityBase e) :
            base(e.Name)
        {
            this.Properties = e.Properties;
            this.Attributes = e.Attributes;
        }

        public ClientQuestion(XElement el)
            : base(DataConstants.vsp_clientquestion, el)
        {
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

        public Enums.VendorMonitoringAnswerTypes VendorMonitoringType
        {
            get { return base.GetPropertyValue<Enums.VendorMonitoringAnswerTypes>(DataConstants.vsp_vendormonitoringanswertype, PropertyType.Picklist, Enums.VendorMonitoringAnswerTypes.None); }
            set { base.SetPropertyValue<Enums.VendorMonitoringAnswerTypes>(DataConstants.vsp_vendormonitoringanswertype, PropertyType.Picklist, value); }
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

        public Guid PlanAssumptionId
        {
            get { return base.GetPropertyValue<Guid>("vsp_planassumptionid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_planassumptionid", PropertyType.Lookup, value); }
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
        
        public Int32 Rank
        {
            get { return base.GetPropertyValue<Int32>("vsp_rank", PropertyType.Number, 0); }
            set { base.SetPropertyValue<Int32>("vsp_rank", PropertyType.Number, value); }
        }

        public Enums.FilterCategory FilterCategory
        {
            get 
            { 
                bool filterCategory = base.GetPropertyValue<Boolean>("vsp_filtercategory", PropertyType.Bit, false);
                if (filterCategory)
                    return Enums.FilterCategory.Filter2;
                else
                    return Enums.FilterCategory.Filter1;
            }
            set
            {
                bool filterCategory = false;
                switch (value)
                {
                    case Enums.FilterCategory.Filter2:
                        filterCategory = true;
                        break;
                    default:
                        break;
                }
                base.SetPropertyValue<Boolean>("vsp_filtercategory", PropertyType.Bit, filterCategory);
            }
        }

        public Enums.QuestionTypes QuestionType
        {
            get { return base.GetPropertyValue<Enums.QuestionTypes>("vsp_questiontype", PropertyType.Picklist, Enums.QuestionTypes.SearchQuestion_Filter1); }
            set { base.SetPropertyValue<Enums.QuestionTypes>("vsp_questiontype", PropertyType.Picklist, value); }
        }

        public bool Equals(ClientQuestion other)
        {
            return this.Id == other.Id;
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
            get { return base.GetPropertyValue<Decimal>("vsp_annualcontribution", PropertyType.Decimal, 0m); }
            set { base.SetPropertyValue<Decimal>("vsp_annualcontribution", PropertyType.Decimal, value); }
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

        /// <summary>
        /// Status of the client question
        /// </summary>
        public Enums.AccountQuestionStatuses Status
        {
            get { return base.GetPropertyValue<Enums.AccountQuestionStatuses>("statuscode", PropertyType.Status, Enums.AccountQuestionStatuses.Unspecified); }
            set { base.SetPropertyValue<Enums.AccountQuestionStatuses>("statuscode", PropertyType.Status, value); }
        }

        public Enums.EntityName ClientEntityName
        {
            get { return base.GetPropertyValue<Enums.EntityName>("vsp_cliententityname", PropertyType.Picklist, Enums.EntityName.NotMapped); }
            set { base.SetPropertyValue<Enums.EntityName>("vsp_cliententityname", PropertyType.Picklist, value); }
        }

        public String AttributName
        {
            get { return base.GetPropertyValue<String>("vsp_attributname", PropertyType.String, ""); }
            set { base.SetPropertyValue<String>("vsp_attributname", PropertyType.String, value); }
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
    }
}
