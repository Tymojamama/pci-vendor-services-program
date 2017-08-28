using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCI.VSP.Data.CRM.Model;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class AnnotationDataLogic : ServiceObjectBase<Annotation, Guid>
    {
        private const string _entityName = "annotation";
        //private string[] _columnSet = new string[] { "annotationid", "createdby", "createdon", "documentbody", "filename", "filesize", "importsequencenumber", "isdocument", "langid", "mimetype", "modifiedby", "modifiedon", "notetext", "objectid", "objecttypecode", "overriddencreatedon", "ownerid", "owningbusinessunit", "stepid", "subject" };

        public AnnotationDataLogic(IAuthenticationRequest authRequest) : base(authRequest, _entityName, null) { }

        /// <summary>
        /// Retrieve a note by ID
        /// </summary>
        /// <param name="dataObjectId">Annotation ID</param>
        /// <param name="columnSet">annotation columns to return</param>
        /// <returns>Annotation</returns>
        public Annotation Retrieve(Guid dataObjectId, string[] columnSet)
        {
            FilterExpression fe = new FilterExpression();
            fe.AddCondition(new ConditionExpression("annotationid", ConditionOperator.Equal, dataObjectId));
            QueryExpression qe = new QueryExpression() { EntityName = "annotation", Criteria = fe };

            if (columnSet == null)
                qe.ColumnSet = new AllColumns();
            else
                qe.ColumnSet = new ColumnSet(columnSet);

            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest() { Query = qe };
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);

            if (res.BusinessEntityCollection.BusinessEntities.Count > 1 || res.BusinessEntityCollection.BusinessEntities.Count == 0)
                return null;
            else
                return new Annotation(res.BusinessEntityCollection.BusinessEntities[0] as annotation);
        }

        /// <summary>
        /// Retrieve a (first) note associated with a specific entity record
        /// </summary>
        /// <param name="dataObjectId">Guid of the entity record</param>
        /// <param name="entityName">Name of the entity</param>
        /// <param name="columnSet">annotation columns to return</param>
        /// <returns>Annotation</returns>
        public Annotation Retrieve(Guid dataObjectId, string entityName, string[] columnSet)
        {
            FilterExpression fe = new FilterExpression();
            fe.AddCondition(new ConditionExpression("objectid", ConditionOperator.Equal, dataObjectId));
            fe.AddCondition(new ConditionExpression("objecttypecode", ConditionOperator.Equal, entityName));
            QueryExpression qe = new QueryExpression() { EntityName = "annotation", Criteria = fe };

            if (columnSet == null)
                qe.ColumnSet = new AllColumns();
            else
                qe.ColumnSet = new ColumnSet(columnSet);

            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest() { Query = qe };
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);

            if (res.BusinessEntityCollection.BusinessEntities.Count == 1)
                return res.BusinessEntityCollection.BusinessEntities.Select<BusinessEntity, Annotation>(a => new Annotation(a as annotation)).FirstOrDefault();
            else
                return new Annotation();
        }

        /// <summary>
        /// Retrieve the notes associated with a specific entity record
        /// </summary>
        /// <param name="dataObjectId">Guid of the entity record</param>
        /// <param name="entityName">Name of the entity</param>
        /// <param name="columnSet">annotation columns to return</param>
        /// <returns>List of Annotation</returns>
        public List<Annotation> RetrieveNotes(Guid dataObjectId, string entityName, string[] columnSet)
        {
            FilterExpression fe = new FilterExpression();
            fe.AddCondition(new ConditionExpression("objectid", ConditionOperator.Equal, dataObjectId));
            fe.AddCondition(new ConditionExpression("objecttypecode", ConditionOperator.Equal, entityName));
            QueryExpression qe = new QueryExpression() { EntityName = "annotation", Criteria = fe };

            if (columnSet == null)
                qe.ColumnSet = new AllColumns();
            else
                qe.ColumnSet = new ColumnSet(columnSet);

            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest() { Query = qe };
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);

            if (res.BusinessEntityCollection.BusinessEntities.Count > 0)
                return res.BusinessEntityCollection.BusinessEntities.Select<BusinessEntity, Annotation>(a => new Annotation(a as annotation)).ToList();
            else
                return new List<Annotation>();
        }
    }
}
