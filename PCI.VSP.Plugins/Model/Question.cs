using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Plugins.Model
{
    public class Question : EntityBase
    {
        private const String _entityName = "vsp_question";
        public enum ComparisonTypes
        {
            Unspecified,
            EqualTo,
            GreaterThanEqualTo,
            LessThanEqualTo,
            WithinRange,
            CompleteMatch,
            AnyMatch
        };

        public Question() : base(_entityName) { }
        public Question(DynamicEntity e) : base(e) { }

        public Guid QuestionId
        {
            get { return base.GetPropertyValue<Guid>("vsp_questionid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_questionid", PropertyType.Key, value); }
        }

        public new string Name
        {
            get { return base.GetPropertyValue<string>("vsp_name", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_name", PropertyType.String, value); }
        }

        public string NonProductQuestion
        {
            get { return base.GetPropertyValue<string>("vsp_nonproductquestion", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_nonproductquestion", PropertyType.String, value); }
        }

        public string VendorWording
        {
            get { return base.GetPropertyValue<string>("vsp_vendorwording", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_vendorwording", PropertyType.String, value); }
        }

        public string ClientWording
        {
            get { return base.GetPropertyValue<string>("vsp_clientwording", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_clientwording", PropertyType.String, value); }
        }

        public string ReportLabel
        {
            get { return base.GetPropertyValue<string>("vsp_reportlabel", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_reportlabel", PropertyType.String, value); }
        }

        public Enums.AnswerTypes ClientAnswerType
        {
            get { return base.GetPropertyValue<Enums.AnswerTypes>("vsp_clientanswertype", PropertyType.Picklist, Enums.AnswerTypes.Unspecified); }
            set { base.SetPropertyValue<Enums.AnswerTypes>("vsp_clientanswertype", PropertyType.Picklist, value); }
        }

        public Enums.AnswerTypes VendorAnswerType
        {
            get { return base.GetPropertyValue<Enums.AnswerTypes>("vsp_vendoranswertype", PropertyType.Picklist, Enums.AnswerTypes.Unspecified); }
            set { base.SetPropertyValue<Enums.AnswerTypes>("vsp_vendoranswertype", PropertyType.Picklist, value); }
        }

        public Enums.DataTypes QuestionDataType
        {
            get { return base.GetPropertyValue<Enums.DataTypes>("vsp_questiondatatype", PropertyType.Picklist, Enums.DataTypes.Unspecified); }
            set { base.SetPropertyValue<Enums.DataTypes>("vsp_questiondatatype", PropertyType.Picklist, value); }
        }

        public Enums.VendorMonitoringAnswerTypes VendorMonitoringType
        {
            get { return base.GetPropertyValue<Enums.VendorMonitoringAnswerTypes>("vsp_vendormonitoringanswertype", PropertyType.Picklist, Enums.VendorMonitoringAnswerTypes.None); }
            set { base.SetPropertyValue<Enums.VendorMonitoringAnswerTypes>("vsp_vendormonitoringanswertype", PropertyType.Picklist, value); }
        }

        public string ChoiceAnswers
        {
            get { return base.GetPropertyValue<string>("vsp_choiceanswers", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_choiceanswers", PropertyType.String, value); }
        }

        public Guid ClientProjectId
        {
            get { return base.GetPropertyValue<Guid>("vsp_clientprojectid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_clientprojectid", PropertyType.Lookup, value); }
        }

        public ComparisonTypes ComparisonType
        {
            get { return base.GetPropertyValue<ComparisonTypes>("vsp_comparisontype", PropertyType.Picklist, 0); }
            set { base.SetPropertyValue<ComparisonTypes>("vsp_comparisontype", PropertyType.Picklist, value); }
        }

        public Enums.QuestionTypes QuestionType
        {
            get { return base.GetPropertyValue<Enums.QuestionTypes>("vsp_questiontype", PropertyType.Picklist, Enums.QuestionTypes.Unspecified); }
            set { base.SetPropertyValue<Enums.QuestionTypes>("vsp_questiontype", PropertyType.Picklist, value); }
        }

        public Guid PlanAssumptionId
        {
            get { return base.GetPropertyValue<Guid>("vsp_planassumptionid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_planassumptionid", PropertyType.Lookup, value); }
        }

        public String PCICommentToVendor
        {
            get { return base.GetPropertyValue<String>("vsp_pcicommenttovendor", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_pcicommenttovendor", PropertyType.String, value); }
        }

        public string PCICommentToClient
        {
            get { return base.GetPropertyValue<String>("vsp_pcicommenttoclient", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<String>("vsp_pcicommenttoclient", PropertyType.String, value); }
        }

        // convert to enum
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
    }
}
