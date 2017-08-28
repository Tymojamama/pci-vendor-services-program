using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.Sdk;
using PCI.VSP.Data.CRM.Model;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class DocumentTypeDataLogic : ServiceObjectBase<Model.DocumentType, Guid>
    {
        private const String _entityName = "vsp_documenttype";
        //private static String[] _columnSet = new String[] { "vsp_clientprojectid", "vsp_questionid" };
        public DocumentTypeDataLogic(Model.IAuthenticationRequest authRequest)
            : base(authRequest, _entityName, null)
        {
        }

        public List<Model.DocumentType> RetrieveAllDocumentTypes()
        {
            QueryExpression qe = new QueryExpression();
            qe.EntityName = "vsp_documenttype";
            qe.ColumnSet = new AllColumns();
            qe.Criteria = new FilterExpression();
            
            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest();
            rmr.Query = qe;
            rmr.ReturnDynamicEntities = true;
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);
            return res.BusinessEntityCollection.BusinessEntities.Select(e => new Model.DocumentType((DynamicEntity)e)).ToList();        }

        public List<string> RetrieveAllowedDocumentTypes()
        {
            QueryExpression qe = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns(), Criteria = new FilterExpression() };
            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest() { Query = qe, ReturnDynamicEntities = true };
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);
            List<DocumentType> dtl = res.BusinessEntityCollection.BusinessEntities.Select(e => new Model.DocumentType((DynamicEntity)e)).ToList();

            List<string> result = new List<string>();
            foreach (var dt in dtl)
                result.Add(dt.Name);

            return result;
        }
    }
}
