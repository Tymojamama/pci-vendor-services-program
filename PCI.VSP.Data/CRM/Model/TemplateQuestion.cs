using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Data.CRM.Model
{
    public class TemplateQuestion : EntityBase
    {
        public TemplateQuestion() : base("templatequestion") { }
        public TemplateQuestion(DynamicEntity e) : base(e) { }

        public Guid QuestionId {
            get { return base.GetPropertyValue<Guid>("vsp_questionid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_questionid", PropertyType.Lookup, value); }
        }
        public Guid TemplateId {
            get { return base.GetPropertyValue<Guid>("vsp_templateid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_templateid", PropertyType.Lookup, value); }
        }
        
        public Guid QuestionCategoryId
        {
            get { return base.GetPropertyValue<Guid>("vsp_questioncategoryid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_questioncategoryid", PropertyType.Lookup, value); }
        }

        public Guid QuestionFunctionId
        {
            get { return base.GetPropertyValue<Guid>("vsp_questionfunctionid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_questionfunctionid", PropertyType.Lookup, value); }
        }
        
        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("vsp_templatequestionid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("vsp_templatequestionid", PropertyType.Key, value); }
        }

        public Int32 SortOrder
        {
            get { return base.GetPropertyValue<Int32>("vsp_sortorder", PropertyType.Number, 0); }
            set { base.SetPropertyValue<Int32>("vsp_sortorder", PropertyType.Number, value); }
        }

        public string Name
        {
            get { return base.GetPropertyValue<string>("vsp_name", PropertyType.String, string.Empty); }
            set { base.SetPropertyValue<string>("vsp_name", PropertyType.String, value); }
        }
    }
}
