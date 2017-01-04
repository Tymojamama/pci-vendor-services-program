using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCI.VSP.Data.CRM.Model;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;
using System.Web.Services.Protocols;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class VendorProductDataLogic : ServiceObjectBase<VendorProduct, Guid>
    {
        private static String[] _columnSet = new String[] { "vsp_accountid", "vsp_name", "vsp_productid", "vsp_vendorproductid", "vsp_lastupdatedby", "vsp_lastupdated", "modifiedby", "modifiedon", "new_legacyproductid" };
        private static String _entityName = "vsp_vendorproduct";

        public VendorProductDataLogic(IAuthenticationRequest authRequest)
            : base(authRequest, _entityName, _columnSet)
        {
        }

        /// <summary>
        /// Retrieve All Vendor Products By Vendor ID
        /// </summary>
        /// <param name="id">Vendor ID GUID</param>
        /// <returns>List of Vendor Products</returns>
        public List<VendorProduct> RetrieveVendorProductByVendor(Guid id)
        {
            List<VendorProduct> result = null;

            try
            {
                QueryExpression qe = new QueryExpression();
                qe.EntityName = "vsp_vendorproduct";
                qe.ColumnSet = new ColumnSet(_columnSet);
                qe.Criteria = new FilterExpression();
                qe.Criteria.Conditions.Add(new ConditionExpression("vsp_accountid", ConditionOperator.Equal, new object[] { id }));

                RetrieveMultipleRequest rmr = new RetrieveMultipleRequest();
                rmr.Query = qe;
                rmr.ReturnDynamicEntities = true;
                RetrieveMultipleResponse res = (RetrieveMultipleResponse)Execute(rmr);
                result = res.BusinessEntityCollection.BusinessEntities.Select(e => new VendorProduct((DynamicEntity)e)).ToList();
            }
            catch (SoapException se)
            {
                se.Data.Add("Id", id.ToString());
                throw;
            }
            catch (Exception e)
            {
                e.Data.Add("Id", id.ToString());
                throw;
            }

            return result;
        }

        public List<VendorProduct> RetrieveAgentProducts(Guid accountId, Guid contactId)
        {
            List<VendorProduct> ret = new List<VendorProduct>();
            try
            {
                QueryExpression query = GetAgentProductQueryExpression(accountId, contactId);
                List<DynamicEntity> results = base.RetrieveMultiple(query);
                if (results == null) { return null; }
                ret = results.Select<DynamicEntity, Model.VendorProduct>(vp => new Model.VendorProduct(vp)).ToList();
            }
            catch (Exception e)
            {
                e.Data.Add("AccountId", accountId.ToString());
                e.Data.Add("ContactId", contactId.ToString());
                throw;
            }
            return ret;
        }

        public new VendorProduct Retrieve(Guid vendorProductId)
        {
            try
            {
                QueryExpression query = new QueryExpression()
                {
                    EntityName = _entityName,
                    ColumnSet = new AllColumns()
                };
                query.Criteria.AddCondition("vsp_vendorproductid", ConditionOperator.Equal, vendorProductId);

                List<DynamicEntity> des = base.RetrieveMultiple(query);
                if (des == null) { return null; }
                return new VendorProduct(base.GetUniqueResult(des));
            }
            catch (Exception e)
            {
                e.Data.Add("VendorProductId", vendorProductId.ToString());
                throw;
            }
        }

        public List<Model.VendorProduct> RetrieveForPhase2Filter(Guid clientProjectId)
        {
            try
            {
                QueryExpression query = GetPhase2FilterExpression(clientProjectId);
                List<DynamicEntity> results = base.RetrieveMultiple(query);
                if (results == null) { return null; }
                return results.Select<DynamicEntity, Model.VendorProduct>(vp => new Model.VendorProduct(vp)).ToList();
            }
            catch (Exception e)
            {
                e.Data.Add("ClientProjectId", clientProjectId.ToString());
                throw;
            }
        }

        public List<Model.VendorProduct> RetrieveAll()
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            List<DynamicEntity> results = base.RetrieveMultiple(query);
            if (results == null) { return null; }
            return results.Select<DynamicEntity, Model.VendorProduct>(vp => new Model.VendorProduct(vp)).ToList();
        }

        private QueryExpression GetPhase2FilterExpression(Guid clientProjectId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };

            LinkEntity projectVendorLink = new LinkEntity(_entityName, "vsp_projectvendor", "vsp_vendorproductid", "vsp_vendorproductid", JoinOperator.Inner);
            //projectVendorLink.LinkCriteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);

            FilterExpression pfe = new FilterExpression();
            pfe.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
            pfe.FilterOperator = LogicalOperator.And;

            FilterExpression fe = new FilterExpression();
            fe.FilterOperator = LogicalOperator.Or;
            fe.AddCondition("vsp_phase1result", ConditionOperator.Equal, true);
            fe.AddCondition("vsp_phase2benchmark", ConditionOperator.Equal, true);

            pfe.AddFilter(fe);
            
            projectVendorLink.LinkCriteria.AddFilter(pfe);
            query.LinkEntities.Add(projectVendorLink);
            return query;
        }

        public void UpdateAgentProducts(IEnumerable<UpdateAgentProductRequest> uaprs)
        {
            if (uaprs == null || uaprs.Count() == 0) { return; }

            foreach (UpdateAgentProductRequest uapr in uaprs)
            {
                Request request = null;

                switch (uapr.UpdateRequestType)
                {
                    case UpdateAgentProductRequest.RequestType.Associate:
                        request = GetManyToManyAssociateRequest(uapr);
                        break;
                    case UpdateAgentProductRequest.RequestType.Disassociate:
                        request = GetManyToManyDissociateRequest(uapr);
                        break;
                }
                if (request == null) { continue; }

                try
                {
                    base.Execute(request);
                }
                catch(Exception ex)
                {
                    ex.Data.Add("VendorProductId", uapr.VendorProductId);
                    ex.Data.Add("ContactId", uapr.VendorAgentId);
                    ex.Data.Add("Action", Enum.GetName(uapr.UpdateRequestType.GetType(), uapr.UpdateRequestType));
                    throw;
                }
                
            }
            
        }

        private AssociateEntitiesRequest GetManyToManyAssociateRequest(UpdateAgentProductRequest uapr)
        {
            AssociateEntitiesRequest aer = new AssociateEntitiesRequest();
            aer.Moniker1 = new Moniker(_entityName, uapr.VendorProductId);
            aer.Moniker2 = new Moniker("contact", uapr.VendorAgentId);
            aer.RelationshipName = "vsp_vendorproduct_contact";
            return aer;
        }

        private DisassociateEntitiesRequest GetManyToManyDissociateRequest(UpdateAgentProductRequest uapr)
        {
            DisassociateEntitiesRequest der = new DisassociateEntitiesRequest();
            der.Moniker1 = new Moniker(_entityName, uapr.VendorProductId);
            der.Moniker2 = new Moniker("contact", uapr.VendorAgentId);
            der.RelationshipName = "vsp_vendorproduct_contact";
            return der;
        }

        public class UpdateAgentProductRequest
        {
            public enum RequestType
            {
                Unspecified = 0,
                Associate = 1,
                Disassociate = 2
            }

            public Guid VendorAgentId { get; set; }
            public Guid VendorProductId { get; set; }
            public RequestType UpdateRequestType { get; set; }
        }

        private QueryExpression GetAgentProductQueryExpression(Guid accountId, Guid contactId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = "vsp_vendorproduct",
                ColumnSet = new ColumnSet(_columnSet)
            };

            FilterExpression fe = new FilterExpression();
            fe.AddCondition(attributeName: "vsp_accountid", conditionOperator: ConditionOperator.Equal, value: accountId);

            // link entity from vendorproduct to vsp_vendorproduct_contact
            LinkEntity vple = new LinkEntity()
            {
                JoinOperator = JoinOperator.Inner,
                LinkFromEntityName = "vsp_vendorproduct",
                LinkFromAttributeName = "vsp_vendorproductid",
                LinkToEntityName = "vsp_vendorproduct_contact",
                LinkToAttributeName = "vsp_vendorproductid"
            };

            // link entity from vsp_vendorproduct_contact to contact
            LinkEntity cle = new LinkEntity()
            {
                JoinOperator = JoinOperator.Inner,
                LinkFromEntityName = "vsp_vendorproduct_contact",
                LinkFromAttributeName = "contactid",
                LinkToEntityName = "contact",
                LinkToAttributeName = "contactid"
            };

            cle.LinkCriteria = new FilterExpression();
            cle.LinkCriteria.AddCondition("contactid", ConditionOperator.Equal, contactId);

            vple.LinkEntities.Add(cle);
            query.LinkEntities.Add(vple);
            query.Criteria = fe;

            query.AddOrder("vsp_name", OrderType.Ascending);
            return query;
        }

        public void UpdateTimestamps(Guid vendorProductId, Guid contactId)
        {
            try
            {
                contact c = (contact)new ContactDataLogic(_authRequest).Retrieve(contactId); 
                String fullName = c.lastname + ", " + c.firstname;

                Model.VendorProduct vp = new Model.VendorProduct()
                {
                    VendorProductId = vendorProductId,
                    LastUpdated = DateTime.UtcNow,
                    LastUpdatedBy = fullName
                };
                base.Update(vp);
            }
            catch (Exception e)
            {
                e.Data.Add("VendorProductId", vendorProductId.ToString());
                throw;
            }
        }
    }
}
