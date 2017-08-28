using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Tricension.Data.CRM4.Model;

namespace PCI.VSP.Management.Model
{
    class VendorQuestion : EntityBase
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
    }
}
