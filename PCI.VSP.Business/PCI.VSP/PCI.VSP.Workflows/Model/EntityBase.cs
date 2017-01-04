using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Workflows
{
    public abstract class EntityBase : DynamicEntity
    {
        public EntityBase() 
        {
            this.Properties = new PropertyCollection();
        }

        public EntityBase(DynamicEntity e)
        {
            this.Name = e.Name;
            this.Properties = new PropertyCollection();
            if (e.Properties != null)
                foreach (var p in e.Properties)
                    this.Properties.Add(p);
        }
        public EntityBase(String name) : base(name) 
        {
            this.Properties = new PropertyCollection();
        }

        protected T GetPropertyValue<T>(string attribute, PropertyType pt, T def)
        {
            try
            {
                switch (pt)
                {
                    case PropertyType.Key:
                        return (T)(object)((Key)this.Properties[attribute]).Value;
                    case PropertyType.Number:
                        return (T)(object)((CrmNumber)this.Properties[attribute]).Value;
                    case PropertyType.Decimal:
                        return (T)(object)Convert.ToDouble(((CrmDecimal)this.Properties[attribute]).Value);
                    case PropertyType.String:
                        return (T)(object)((string)this.Properties[attribute]);
                    case PropertyType.DateTime:
                        DateTime dotnetDate = ((CrmDateTime)this.Properties[attribute]).UniversalTime;
                        return (T)(object)dotnetDate;
                    case PropertyType.Picklist:
                        return (T)(object)((Picklist)this.Properties[attribute]).Value;
                    case PropertyType.Owner:
                        return (T)(object)((Owner)this.Properties[attribute]).Value;
                    case PropertyType.Lookup:
                        return (T)(object)((Lookup)this.Properties[attribute]).Value;
                    case PropertyType.Customer:
                        return (T)(object)((CrmReference)this.Properties[attribute]).Value;
                    case PropertyType.Status:
                        return (T)(object)((Status)this.Properties[attribute]).name;
                    default:
                        return def;
                }
            }
            catch { }
            return def;
        }
        protected void SetPropertyValue<T>(string attribute, PropertyType pt, T value)
        {
            try
            {
                switch (pt)
                {
                    case PropertyType.Key:
                        this.Properties[attribute] = new Key((Guid)(object)value);
                        break;
                    case PropertyType.Number:
                        this.Properties[attribute] = new CrmNumber((int)(object)value);
                        break;
                    case PropertyType.Decimal:
                        this.Properties[attribute] = new CrmDecimal((decimal)(object)value);
                        break;
                    case PropertyType.String:
                        this.Properties[attribute] = value;
                        break;
                    case PropertyType.DateTime:
                        this.Properties[attribute] = new CrmDateTime(((DateTime)(object)value).ToString("yyyy-MM-ddTHH:mm:ss Z"));
                        break;
                    case PropertyType.Picklist:
                        this.Properties[attribute] = new Picklist((int)(object)value);
                        break;
                    case PropertyType.Owner:
                        this.Properties[attribute] = new Owner("systemuser", (Guid)(object)value);
                        break;
                    case PropertyType.Lookup:
                        this.Properties[attribute] = new Lookup("systemuser", (Guid)(object)value);
                        break;
                    case PropertyType.Customer:
                        this.Properties[attribute] = new Customer("account", (Guid)(object)value);
                        break;
                    //case PropertyType.Status:
                    //    this.Properties[attribute] = new Status((int)(object)value);
                    //    break;
                }
            }
            catch { }
        }

        internal DynamicEntity GetDynamicEntity()
        {
            DynamicEntity de = new DynamicEntity();

            de.Name = Name;
            de.Properties = new PropertyCollection();
            if (this.Properties != null)
                foreach (var p in this.Properties)
                    de.Properties.Add(p);

            return de;
        }

        protected enum PropertyType
        {
            Key,
            Number,
            Decimal,
            String,
            DateTime,
            Picklist,
            Owner,
            Lookup,
            Customer,
            Status
        }

        protected DateTime? StringDateToNullable(String crmDateString)
        {
            DateTime? dotNetDate = new DateTime?();
            DateTime tempDate;
            if (DateTime.TryParse(crmDateString, out tempDate))
                dotNetDate = tempDate;
            return dotNetDate;
        }

        protected DateTime? DateToNullable(DateTime date)
        {
            DateTime? nullableDate = new DateTime?();
            if (date != DateTime.MinValue) { nullableDate = date; }
            return nullableDate;
        }

        protected DateTime NullableToDate(DateTime? nullableDate)
        {
            if (nullableDate.HasValue)
                return nullableDate.Value;
            else
                return DateTime.MinValue;
        }
    }
}
