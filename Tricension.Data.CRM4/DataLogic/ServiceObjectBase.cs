using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using System.Collections;
using System.Configuration;
using Tricension.Data.CRM4.DataLogic.CrmDiscoveryServices;

namespace Tricension.Data.CRM4.DataLogic
{
    public abstract class DataObjectBase<TDataObject, TDataKey> where TDataObject : Model.EntityBase
    {
        protected String[] ColumnSet { get; set; }
        protected String EntityName { get; set; }

        protected DataObjectBase(string entityName, string[] columnSet)
        {
            this.EntityName = entityName;
            if (columnSet != null) { ColumnSet = columnSet; }
        }

        protected DynamicEntity GetUniqueResult(List<DynamicEntity> list) 
        {
            if (list.Count > 1)
                throw new Model.CustomExceptions.TooManyResultsException();
            else if (list.Count == 0)
                return null;
            return list.FirstOrDefault();
        }
    }

    public abstract class ServiceObjectBase<TDataObject, TDataKey> : DataObjectBase<TDataObject, TDataKey> where TDataObject : Model.EntityBase
    {
        protected Model.IAuthenticationRequest _authRequest;

        public ServiceObjectBase(Model.IAuthenticationRequest authRequest, String entityName, String[] columnSet)
            : base(entityName, columnSet)
        {
            _authRequest = authRequest;
        }

        public Guid Create(TDataObject dataObject)
        {
            return ServiceBroker.GetServiceInstance(_authRequest).Create(dataObject.GetDynamicEntity());
        }

        public void Update(TDataObject dataObject)
        {
            ServiceBroker.GetServiceInstance(_authRequest).Update(dataObject.GetDynamicEntity());
        }

        public void Delete(TDataObject dataObject)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid dataObjectId)
        {
            ServiceBroker.GetServiceInstance(_authRequest).Delete(base.EntityName, dataObjectId);
        }

        public BusinessEntity Retrieve(Guid dataObjectId)
        {
            return ServiceBroker.GetServiceInstance(_authRequest).Retrieve(base.EntityName, dataObjectId, new ColumnSet(base.ColumnSet));
        }

        public BusinessEntity Retrieve(Guid dataObjectId, String[] columnSet)
        {
            ColumnSetBase cs;
            if (columnSet == null || columnSet.Length == 0)
                cs = new AllColumns();
            else
                cs = new ColumnSet(columnSet);
            return ServiceBroker.GetServiceInstance(_authRequest).Retrieve(base.EntityName, dataObjectId, cs);
        }

        public Object Execute(Microsoft.Crm.SdkTypeProxy.Request obj)
        {
            return ServiceBroker.GetServiceInstance(_authRequest).Execute(obj);
        }

        public List<DynamicEntity> RetrieveMultiple(FilterExpression filterExpression, OrderExpression orderExpression)
        {
            return RetrieveMultiple(filterExpression, orderExpression, base.ColumnSet);
        }

        public List<DynamicEntity> RetrieveMultiple(FilterExpression filterExpression)
        {
            return RetrieveMultiple(filterExpression, null);
        }

        public List<DynamicEntity> RetrieveMultiple(FilterExpression filterExpression, OrderExpression orderExpression, String[] columnSet)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = base.EntityName,
                Criteria = filterExpression,
                ColumnSet = new ColumnSet(columnSet)
            };
            if (orderExpression != null)
                query.Orders.Add(orderExpression);

            Microsoft.Crm.SdkTypeProxy.RetrieveMultipleRequest request = new Microsoft.Crm.SdkTypeProxy.RetrieveMultipleRequest()
            {
                ReturnDynamicEntities = true,
                Query = query
            };

            Microsoft.Crm.SdkTypeProxy.CrmService service = ServiceBroker.GetServiceInstance(_authRequest);
            return ((Microsoft.Crm.SdkTypeProxy.RetrieveMultipleResponse)service.Execute(request)).BusinessEntityCollection.BusinessEntities.Select(e => (DynamicEntity)e).ToList();
        }

        public List<DynamicEntity> RetrieveMultiple(QueryExpression queryExpression)
        {
            if (queryExpression == null)
                throw new ArgumentNullException("The 'queryExpression' argument cannot be null.");

            Microsoft.Crm.SdkTypeProxy.RetrieveMultipleRequest request = new Microsoft.Crm.SdkTypeProxy.RetrieveMultipleRequest()
            {
                ReturnDynamicEntities = true,
                Query = queryExpression
            };

            Microsoft.Crm.SdkTypeProxy.CrmService service = ServiceBroker.GetServiceInstance(_authRequest);
            return ((Microsoft.Crm.SdkTypeProxy.RetrieveMultipleResponse)service.Execute(request)).BusinessEntityCollection.BusinessEntities.Select(e => (DynamicEntity)e).ToList();
        }

    }
}
