using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Workflows
{
    class VendorQuestion : EntityBase
    {
        internal const String _entityName = "vsp_vendorquestion";

        internal VendorQuestion() : base(_entityName) { }
        internal VendorQuestion(DynamicEntity e) : base(e) { }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_vendorquestionid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_vendorquestionid", PropertyType.Key, value); }
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

        public int InvalidAnswerReason
        {
            get { return base.GetPropertyValue<int>("vsp_invalidanswerreason", PropertyType.Picklist, -1); }
            set { base.SetPropertyValue<int>("vsp_invalidanswerreason", PropertyType.Picklist, value); }
        }
        public string AnswerRejectedReason
        {
            get { return base.GetPropertyValue<string>("vsp_answerrejectedreason", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_answerrejectedreason", PropertyType.String, value); }
        }

    }
}
