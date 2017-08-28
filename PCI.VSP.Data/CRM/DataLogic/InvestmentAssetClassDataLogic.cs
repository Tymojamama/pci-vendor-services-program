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
    public class InvestmentAssetClassDataLogic : ServiceObjectBase<InvestmentAssetClass, Guid>
    {
        private const String _entityName = "vsp_investmentassetclass";
        public InvestmentAssetClassDataLogic(Model.IAuthenticationRequest authRequest) : base(authRequest, _entityName, null) { }

        /// <summary>
        /// Retrieves a List of all Investment (Assumption) Asset Classes
        /// </summary>
        /// <returns>List of Investment Asset Classes</returns>
        public List<InvestmentAssetClass> RetrieveAll()
        {
            QueryExpression qe = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns(), Criteria = new FilterExpression() };
            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest() { Query = qe, ReturnDynamicEntities = true };
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);
            return res.BusinessEntityCollection.BusinessEntities.Select(e => new InvestmentAssetClass((DynamicEntity)e)).ToList();
        }

        /// <summary>
        /// Retrieves Investment (Assumption) Asset Class data in Dictionary Form
        /// </summary>
        /// <returns>Dictionary of Investment Asset Class</returns>
        public Dictionary<Guid, InvestmentAssetClass> RetrieveAllAsDictionary()
        {
            QueryExpression qe = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns(), Criteria = new FilterExpression() };
            RetrieveMultipleRequest rmr = new RetrieveMultipleRequest() { Query = qe, ReturnDynamicEntities = true };
            RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);
            return res.BusinessEntityCollection.BusinessEntities.Select(e => new InvestmentAssetClass((DynamicEntity)e)).ToDictionary(e => e.Id);
        }
    }
}
