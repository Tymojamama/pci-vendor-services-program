using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Data.CRM.Model
{
    public class VendorQuestionHistory : EntityBase
    {
        private const String _entityName = "vsp_vendorquestionhistory";

        public VendorQuestionHistory() : base(_entityName) {}
        public VendorQuestionHistory(DynamicEntity e)
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

        public DateTime? LastUpdated
        {
            get
            {
                return base.GetPropertyValue<DateTime?>("vsp_lastupdated", PropertyType.DateTime, null);
            }
            set
            {
                base.SetPropertyValue<DateTime?>("vsp_lastupdated", PropertyType.DateTime, value);
            }
        }

        public Enums.AccountQuestionStatuses Status
        {
            get { return base.GetPropertyValue<Enums.AccountQuestionStatuses>("statuscode", PropertyType.Status, Enums.AccountQuestionStatuses.Unspecified); }
            set { base.SetPropertyValue<Enums.AccountQuestionStatuses>("statuscode", PropertyType.Status, value); }
        }
        
        public Guid VendorQuestionId
        {
            get { return base.GetPropertyValue<Guid>("vsp_vendorquestionid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_vendorquestionid", PropertyType.Lookup, value); }
        }

        public Guid ClientProjectId
        {
            get { return base.GetPropertyValue<Guid>("vsp_clientprojectid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_clientprojectid", PropertyType.Lookup, value); }
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
