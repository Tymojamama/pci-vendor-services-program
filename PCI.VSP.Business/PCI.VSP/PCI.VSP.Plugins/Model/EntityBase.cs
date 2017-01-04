using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Plugins.Model
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
        protected Guid GetId(string attribute)
        {
            var ret = Guid.Empty;
            object val = base.Properties.Where(p => p.Name == attribute).FirstOrDefault();
            if (val != null)
            {
                if (val is KeyProperty)
                    val = ((KeyProperty)val).Value;
                if (val is Key)
                    val = ((Key)val).Value;
                if (val is LookupProperty)
                    val = ((LookupProperty)val).Value;
                if (val is Lookup)
                    val = ((Lookup)val).Value;
                if (val is Guid?)
                    val = ((Guid?)val).Value;
                if (val is Guid)
                    ret = (Guid)val;
            }

            return ret;
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
                        return (T)(object)Convert.ToDecimal(((CrmDecimal)this.Properties[attribute]).Value);
                    case PropertyType.String:
                        return (T)(object)((string)this.Properties[attribute]);
                    case PropertyType.DateTime:
                        DateTime dotnetDate = ((CrmDateTime)this.Properties[attribute]).UniversalTime;
                        return (T)(object)dotnetDate;
                    case PropertyType.Picklist:
                        if (typeof(T) == typeof(Picklist))
                        {
                            return (T)(object)((Picklist)this.Properties[attribute]);
                        }
                        else
                        {
                            return (T)(object)((Picklist)this.Properties[attribute]).Value;
                        }
                    case PropertyType.Owner:
                        return (T)(object)((Owner)this.Properties[attribute]).Value;
                    case PropertyType.Lookup:
                        return (T)(object)((Lookup)this.Properties[attribute]).Value;
                    case PropertyType.Customer:
                        return (T)(object)((CrmReference)this.Properties[attribute]).Value;
                    case PropertyType.Bit:
                        return (T)(object)((CrmBoolean)this.Properties[attribute]).Value;
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
                        Guid formattedValue = (Guid)(object)value;
                        if (formattedValue == Guid.Empty)
                        {
                            if (this.Properties.Contains(attribute))
                                this.Properties.Remove(attribute);
                        }
                        else
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
                        int intValue = (int)(object)value;
                        Picklist picklist = new Picklist();
                        if (intValue == 0)
                        {
                            picklist.IsNull = true;
                            picklist.IsNullSpecified = true;
                        }
                        else
                            picklist.Value = intValue;
                        this.Properties[attribute] = picklist;
                        break;
                    case PropertyType.Owner:
                        this.Properties[attribute] = new Owner("systemuser", (Guid)(object)value);
                        break;
                    case PropertyType.Lookup:
                        Guid guidValue = (Guid)(object)value;
                        Lookup lookup = new Lookup();
                        if (guidValue == Guid.Empty)
                        {
                            lookup.IsNull = true;
                            lookup.IsNullSpecified = true;
                        }
                        else
                            lookup.Value = guidValue;
                        this.Properties[attribute] = lookup;
                        //this.Properties[attribute] = new Lookup("systemuser", (Guid)(object)value);
                        break;
                    case PropertyType.Customer:
                        this.Properties[attribute] = new Customer("account", (Guid)(object)value);
                        break;
                    case PropertyType.Bit:
                        this.Properties[attribute] = new CrmBoolean((Boolean)(object)value);
                        break;
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
            Status,
            Bit
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
