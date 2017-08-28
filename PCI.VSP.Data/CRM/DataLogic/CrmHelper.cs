using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.Sdk.Query;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class CrmHelper
    {
        public static Customer SetAccountCustomer(Guid accountId)
        {
            Customer customer = new Customer();
            customer.type = EntityName.account.ToString();
            customer.Value = accountId;
            return customer;
        }

        public static Owner SetOwner(Guid ownerId, string entityTypeName)
        {
            Owner owner = new Owner();
            owner.type = entityTypeName;
            owner.Value = ownerId;
            return owner;
        }

        public static Customer SetContactCustomer(Guid contactId)
        {
            Customer customer = new Customer();
            customer.type = EntityName.contact.ToString();
            customer.Value = contactId;
            return customer;
        }



        /// <summary>
        /// Gets the specified property from the dynamic entity
        /// </summary>
        /// <param name="entity">The dynamic entity to retrieve the property from</param>
        /// <param name="propertyName">Name of the property to retrieve</param>
        /// <returns>Returns the specified property from the dynamic entity</returns>
        public static Property GetDynamicEntityProperty(DynamicEntity entity, string propertyName)
        {
            foreach (Property entityProperty in entity.Properties)
            {
                if (entityProperty.Name == propertyName)
                {
                    return entityProperty;
                }
            }

            //TODO: throw new ArgumentException("No such property exists.");
            return null;
        }

        /// <summary>
        /// gets the policy value from the dynamic entity if it is not null, 
        /// otherwise gets it from the value passed in
        /// </summary>
        /// <param name="opportunityEntity">the opportunity dynamic entity</param>
        /// <param name="attributeName">the attribute's name</param>
        /// <param name="attributeValue">the attribute's value</param>
        /// <returns></returns>
        #region SetModifiedAttribute
        public static CrmBoolean SetModifiedAttribute(DynamicEntity dynamicEntity, string attributeName, CrmBoolean attributeValue)
        {
            CrmBooleanProperty property = (CrmBooleanProperty)GetDynamicEntityProperty(dynamicEntity, attributeName);
            if (property != null)
            {
                return property.Value;
            }
            else
            {
                return attributeValue;
            }
        }
        public static Picklist SetModifiedAttribute(DynamicEntity dynamicEntity, string attributeName, Picklist attributeValue)
        {
            PicklistProperty property = (PicklistProperty)GetDynamicEntityProperty(dynamicEntity, attributeName);
            if (property != null)
            {
                return property.Value;
            }
            else
            {
                return attributeValue;
            }
        }
        public static string SetModifiedAttribute(DynamicEntity dynamicEntity, string attributeName, string attributeValue)
        {
            StringProperty property = (StringProperty)GetDynamicEntityProperty(dynamicEntity, attributeName);
            if (property != null)
            {
                return property.Value;
            }
            else
            {
                return attributeValue;
            }
        }
        public static CrmFloat SetModifiedAttribute(DynamicEntity dynamicEntity, string attributeName, CrmFloat attributeValue)
        {
            CrmFloatProperty property = (CrmFloatProperty)GetDynamicEntityProperty(dynamicEntity, attributeName);
            if (property != null)
            {
                return property.Value;
            }
            else
            {
                return attributeValue;
            }
        }
        public static CrmDateTime SetModifiedAttribute(DynamicEntity dynamicEntity, string attributeName, CrmDateTime attributeValue)
        {
            CrmDateTimeProperty property = (CrmDateTimeProperty)GetDynamicEntityProperty(dynamicEntity, attributeName);
            if (property != null)
            {
                return property.Value;
            }
            else
            {
                return attributeValue;
            }
        }

        //public static void UpdateProperty<TPropertyType>(DynamicEntity dynamicEntity, string propertyName, object value)
        //{
        //    if (value != null)
        //    {
        //        Property[] updateProps = new Property[dynamicEntity.Properties.Length];

        //        int i = 0;
        //        bool propertyFound = false;

        //        foreach (Property property in dynamicEntity.Properties)
        //        {
        //            if (property != null)
        //            {
        //                if (propertyName != property.Name)
        //                {
        //                    updateProps[i] = property;
        //                    i++;
        //                }
        //                else
        //                {
        //                    propertyFound = true;
        //                }
        //            }
        //        }

        //        if (!propertyFound)
        //        {
        //            throw new ArgumentOutOfRangeException("A property with name of " + propertyName + " not found");
        //        }

        //        TPropertyType crmProperty = Activator.CreateInstance<TPropertyType>();

        //        try
        //        {
        //            PropertyInfo property = typeof(TPropertyType).GetProperty("Name");
        //            property.SetValue(crmProperty, propertyName, null);

        //            PropertyInfo propertyValue = typeof(TPropertyType).GetProperty("Value");
        //            Type propertyValueType = propertyValue.PropertyType;

        //            object crmPropertyValue = Activator.CreateInstance(propertyValueType);
        //            PropertyInfo propertyValueValue = crmPropertyValue.GetType().GetProperty("Value");

        //            propertyValueValue.SetValue(crmPropertyValue, value, null);
        //            propertyValue.SetValue(crmProperty, crmPropertyValue, null);

        //            updateProps[dynamicEntity.Properties.Length - 1] = crmProperty as Property;
        //            dynamicEntity.Properties = updateProps;

        //        }
        //        catch
        //        {
        //            throw new ArgumentException("SetCrmValue does not Support CRM Type:" + typeof(TPropertyType).Name);
        //        }
        //    }


        //}


        #endregion

        /// <summary>
        /// Deserializes an XML string into a dynamic entity
        /// </summary>
        /// <param name="entityXml">XML to deserialize</param>
        /// <returns>Returns a dynamic entity deserialized from an XML string</returns>
        public static DynamicEntity DeserializeXml(string entityXml)
        {
            TextReader reader = new StringReader(entityXml);

            // Check if serializer exists
            XmlSerializer serializer = GetSerializer();

            // deserialize & cast as a business entity
            BusinessEntity entity = (BusinessEntity)serializer.Deserialize(reader);

            return entity as DynamicEntity;
        }

        /// <summary>
        /// Creates and returns XML from a supplies DynamicEntity object
        /// </summary>
        public static string CreateXmlFromDynamicEntity(DynamicEntity dynamicEntity)
        {
            StringBuilder builder = new StringBuilder();
            StringWriter strWriter = new StringWriter(builder);

            // Check if serializer exists
            XmlSerializer serializer = GetSerializer();

            serializer.Serialize(strWriter, dynamicEntity);

            return builder.ToString();
        }

        /// <summary>
        /// Adds a supplied property to supplied dynamicEntity
        /// </summary>
        //public static void AddProperty(DynamicEntity dynamicEntity, Property property)
        //{
        //    Property[] updateProps = new Property[dynamicEntity.Properties.Length + 1];

        //    int i = 0;
        //    bool updated = false;
        //    foreach (Property prop in dynamicEntity.Properties)
        //    {
        //        if (prop.Name == property.Name)
        //        {
        //            dynamicEntity.Properties[i] = property;
        //            updated = true;
        //            break;
        //        }
        //        i++;
        //    }

        //    if (!updated)
        //    {
        //        dynamicEntity.Properties.CopyTo(updateProps, 0);
        //        updateProps[dynamicEntity.Properties.Length] = property;
        //        dynamicEntity.Properties = updateProps;
        //    }
        //}

        /// <summary>
        /// Creates or returns the XmlSerializer used in converting dynamic entities
        /// </summary>
        /// <returns>Serializer used in converting dynamic entities</returns>
        private static XmlSerializer GetSerializer()
        {
            XmlRootAttribute root = new XmlRootAttribute("BusinessEntity");
            root.Namespace = "http://schemas.microsoft.com/crm/2006/WebServices";

            XmlSerializer serializer = new XmlSerializer(typeof(BusinessEntity), root);

            return serializer;
        }

        /// <summary>
        /// Creates A Query Expression that will return the results of from a N-N Relationship (Many-to-Many).
        /// </summary>
        /// <param name="returnType">EntityName of the return type.</param>
        /// <param name="returnTypePK">Attribute (Field) name that is the Primary Key of the return type.</param>
        /// <param name="joinTable">Hidden EntityName of the join table (usually [org_]primaryentity_secondaryentitiy).</param>
        /// <param name="otherType">EntityName of the other entity in the relationship.</param>
        /// <param name="otherTypePK">Attribute (Field) name that is the primary key of the other entity in the relationship.</param>
        /// <param name="conditionAttribute">Attribute (Field) name that will be filtered by (usually <primarytable>id ).</param>
        /// <param name="conditionOperator">Operator that will be applied to the field (Attribute [Equals|GreaterThan] Value.</param>
        /// <param name="conditionValue">Value to compare to the Condition Attribute.</param>
        /// <param name="conditionValueUsed">Pass true unless the condition operator does not allow a value (Relative data comparators dont allow values, IE OnOrAfter).</param>
        /// <returns>Query Expression that can be passed to the CrmService.</returns>
        public static QueryExpression CreateManyToManyQueryExpression(string returnType, string returnTypePK, string joinTable, string otherType, string otherTypePK, string conditionAttribute, ConditionOperator conditionOperator, object conditionValue, bool conditionValueUsed)
        {
            return CreateManyToManyQueryExpression(returnType, returnTypePK, returnTypePK, joinTable, otherTypePK, otherType, otherTypePK, conditionAttribute, conditionOperator, conditionValue, conditionValueUsed);
        }

        /// <summary>
        /// Creates A Query Expression that will return the results of from a N-N Relationship (Many-to-Many).
        /// </summary>
        /// <param name="returnType">EntityName of the return type.</param>
        /// <param name="returnTypePK">Attribute (Field) name that is the Primary Key of the return type.</param>
        /// <param name="returnTypeAtrributeInJoinTable">Attribute Name in the Join table of the ReturnType's PK.</param>
        /// <param name="joinTable">Hidden EntityName of the join table (usually [org_]primaryentity_secondaryentitiy).</param>
        /// <param name="otherTypeAttibuteInJoinTable">Attribute Name in the Join table of the OtherType's PK.</param>
        /// <param name="otherType">EntityName of the other entity in the relationship.</param>
        /// <param name="otherTypePK">Attribute (Field) name that is the primary key of the other entity in the relationship.</param>
        /// <param name="conditionAttribute">Attribute (Field) name that will be filtered by (usually <primarytable>id ).</param>
        /// <param name="conditionOperator">Operator that will be applied to the field (Attribute [Equals|GreaterThan] Value.</param>
        /// <param name="conditionValue">Value to compare to the Condition Attribute.</param>
        /// <param name="conditionValueUsed">Pass true unless the condition operator does not allow a value (Relative data comparators dont allow values, IE OnOrAfter).</param>
        /// <returns>Query Expression that can be passed to the CrmService.</returns>
        public static QueryExpression CreateManyToManyQueryExpression(string returnType, string returnTypePK, string returnTypeAtrributeInJoinTable, string joinTable, string otherTypeAttibuteInJoinTable, string otherType, string otherTypePK, string conditionAttribute, ConditionOperator conditionOperator, object conditionValue, bool conditionValueUsed)
        {
            QueryExpression qe = new QueryExpression();
            qe.EntityName = returnType;
            qe.ColumnSet = new AllColumns();

            LinkEntity linkEntity = new LinkEntity();
            linkEntity.LinkFromEntityName = returnType;
            linkEntity.LinkFromAttributeName = returnTypePK;
            linkEntity.LinkToEntityName = joinTable;
            linkEntity.LinkToAttributeName = returnTypeAtrributeInJoinTable;

            //Join afs_afs_site_afs_adventuser to afs_adventuser
            LinkEntity linkEntity2 = new LinkEntity();
            linkEntity2.LinkFromEntityName = joinTable;
            linkEntity2.LinkFromAttributeName = otherTypeAttibuteInJoinTable;
            linkEntity2.LinkToEntityName = otherType;
            linkEntity2.LinkToAttributeName = otherTypePK;

            //Join on afs_siteid where afs_siteid=org_id
            ConditionExpression conditionExpression = new ConditionExpression();
            conditionExpression.AttributeName = conditionAttribute;
            conditionExpression.Operator = conditionOperator;
            if (conditionValueUsed)
                conditionExpression.Values = new object[] { conditionValue };
            else
                conditionExpression.Values = new object[] { };

            //Apply join condition to linkEntity
            linkEntity2.LinkCriteria = new FilterExpression();
            linkEntity2.LinkCriteria.AddCondition(conditionExpression);

            //Join the tables
            linkEntity.LinkEntities.Add(linkEntity2);
            qe.LinkEntities.Add(linkEntity);

            return qe;
        }

        //public static QueryExpression RetrieveAll(string entityName, string orderAttribute)
        //{
        //    return RetrieveAll(entityName, orderAttribute, new AllColumns());
        //}

        //public static QueryExpression RetrieveAll(string entityName, string orderAttribute, ColumnSetBase columns)
        //{
        //    QueryExpression qe = new QueryExpression();
        //    qe.ColumnSet = columns;
        //    qe.Criteria = new FilterExpression();
        //    qe.Criteria.Conditions = new ConditionExpression[0];
        //    qe.Distinct = true;
        //    qe.EntityName = entityName;
        //    if (!String.IsNullOrEmpty(orderAttribute))
        //    {
        //        qe.Orders = new OrderExpression[] { new OrderExpression() };
        //        qe.Orders[0].AttributeName = orderAttribute;
        //        qe.Orders[0].OrderType = OrderType.Ascending;
        //    }
        //    return qe;
        //}
    }
}
