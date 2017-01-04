using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;
using System.Diagnostics;
using Microsoft.Crm.SdkTypeProxy.Metadata;

namespace PCI.VSP.Plugins.DataLogic
{
    abstract class ServiceObjectBase<T> where T : Model.EntityBase
    {
        protected ICrmService _crmService = null;

        protected ServiceObjectBase(ICrmService crmService)
        {
            _crmService = crmService;
        }

        protected Guid Create(T dataObject)
        {
            // loop all properties
            // if type lookup and value is Guid
            // set innervalue of lookup property to null
            // if guid.empty then set to null

            var de = dataObject.GetDynamicEntity();

            foreach (var item in de.Properties)
            {
                Property p = item;
            }

            return _crmService.Create(de);
        }
        public bool Associate(string entity, string name1, Guid id1, string name2, Guid id2)
        {
            try
            {        // Create an AssociateEntities request.        
                AssociateEntitiesRequest request = new AssociateEntitiesRequest();

                // Set the ID of Moniker1 to the ID of the lead.        
                request.Moniker1 = new Moniker(name1, id1);  

                // Set the ID of Moniker2 to the ID of the contact.        
                request.Moniker2 = new Moniker(name2, id2); 

                // Set the relationship name to associate on.        
                request.RelationshipName = entity;

                // Execute the request.        
                _crmService.Execute(request); 

                return true;
            }

            catch (System.Web.Services.Protocols.SoapException ex) { return false; }
        }

        protected void Update(T dataObject)
        {
            var de = dataObject.GetDynamicEntity();
            _crmService.Update(de);
        }

        protected T Retrieve(string entityName, Guid id)
        {
            return (T)_crmService.Retrieve(entityName, id, new AllColumns());
        }

        protected List<T> RetrieveMultiple(QueryExpression query)
        {
            Trace.WriteLine("PCI.VSP.Plugins.DataLogic.ServiceObjectBase.RetrieveMultiple()");
            //Trace.WriteLine("QueryExression:");

            //QueryExpressionToFetchXmlRequest request = new QueryExpressionToFetchXmlRequest();
            //request.Query = query;

            //QueryExpressionToFetchXmlResponse response = _crmService.Execute(request) as QueryExpressionToFetchXmlResponse;
            //Trace.WriteLine(response.FetchXml);

            //Trace.WriteLine("Executing query against CrmService.");

            RetrieveMultipleRequest request = new RetrieveMultipleRequest()
            {
                ReturnDynamicEntities = true,
                Query = query
            };

            BusinessEntityCollection bec = ((RetrieveMultipleResponse)_crmService.Execute(request)).BusinessEntityCollection;
            if (bec == null || bec.BusinessEntities.Count == 0) { return null; }

            Trace.WriteLine("Casting from business entity to <T>.");
            List<T> questions = bec.BusinessEntities.Select<BusinessEntity, T>(be => (T)Activator.CreateInstance(typeof(T), be as DynamicEntity)).ToList();
            Trace.WriteLine("Finished casting from business entity to <T>.");
            return questions;
        }
    }
}
