using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;

namespace PCI.VSP.Data.CRM.Model
{
    [Serializable]
    public class Annotation : EntityBase
    {
        private const string _entityName = "annotation";

        public Annotation() : base(_entityName) { }
        //public Annotation(DynamicEntity e) : base(e) { }
        public Annotation(annotation a) : base(_entityName) 
        {
            Id = a.annotationid.Value;
            if (a.createdon != null) CreatedOn = a.createdon.UserTime;
            DocumentBody = a.documentbody;
            FileName = a.filename;
            if (a.objectid != null) ObjectId = a.objectid.Value;
            if (a.objecttypecode != null) ObjectTypeCode = a.objecttypecode.Value;
        }

        public Guid Id
        {
            get { return base.GetPropertyValue<Guid>("annotationid", PropertyType.Key, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("annotationid", PropertyType.Key, value); }
        }

        public DateTime? CreatedOn
        {
            get
            {
                DateTime? date = base.GetPropertyValue<DateTime>("createdon", PropertyType.DateTime, DateTime.MinValue);
                if (date == DateTime.MinValue) date = null;
                return date;
            }
            set
            {
                if (value.HasValue)
                    base.SetPropertyValue<DateTime>("createdon", PropertyType.DateTime, value.Value);
            }
        }

        public string DocumentBody
        {
            get { return base.GetPropertyValue<string>("documentbody", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<string>("documentbody", PropertyType.String, value); }
        }

        public string FileName
        {
            get { return base.GetPropertyValue<string>("filename", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<string>("filename", PropertyType.String, value); }
        }

        public Guid ObjectId
        {
            get { return base.GetPropertyValue<Guid>("objectid", PropertyType.Lookup, Guid.Empty); }
            set { base.SetPropertyValue<Guid>("objectid", PropertyType.Lookup, value); }
        }

        public string ObjectTypeCode
        {
            get { return base.GetPropertyValue<string>("objecttypecode", PropertyType.String, String.Empty); }
            set { base.SetPropertyValue<string>("objecttypecode", PropertyType.String, value); }
        }
    }
}
