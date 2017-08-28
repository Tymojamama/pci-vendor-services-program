using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services
{
    class VendorService
    {
        //// Vendor Products
        //List<PCI.VSP.Data.CRM.Model.VendorProduct> GetAgentProducts(Guid accountId, Guid contactId);
        //void UpdateAgentProducts(Guid accountId, Guid contactId, IEnumerable<Guid> vendorProductIds);
        //Data.CRM.Model.VendorProduct GetVendorProduct(Guid vendorProductId);
        //List<PCI.VSP.Data.CRM.Model.VendorProduct> GetVendorProducts(Guid accountId);

        //// Dashboard
        //List<Model.VendorProjectInquirySummary> GetClientInquiryDashboard(Guid accountId, Guid contactId);
        //List<Model.VendorProductSummary> GetVendorProductDashboard(Guid accountId, Guid contactId);

        //// Vendors
        //Data.CRM.Model.Vendor GetVendor(Guid vendorId);

        //// Vendor Agent Management
        //List<Model.VendorAgent> GetAgentSummary(Guid accountId);
        //Guid CreateAgent(Model.VendorAgent vendorAgent);
        //void UpdateAgent(Model.VendorAgent vendorAgent);
  
        private AuthenticationRequest GetDefaultAuthRequest()
        {
            return new AuthenticationRequest()
            {
                Username = Data.Globals.CrmServiceSettings.Username,
                Password = Data.Globals.CrmServiceSettings.Password
            };
        }

        public void UpdateAgentProducts(Guid accountId, Guid contactId, IEnumerable<Guid> vendorProductIds)
        {
            Data.CRM.DataLogic.VendorProductDataLogic vpdl = new Data.CRM.DataLogic.VendorProductDataLogic(GetDefaultAuthRequest());

            // get assigned vendorProducts for this contact
            List<Data.CRM.Model.VendorProduct> assignedProducts = vpdl.RetrieveAgentProducts(accountId, contactId);

            // dissociate any vendorProducts that aren't included in the parameter
            List<Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest> dissociateRequests = GetVendorProductDissociations(contactId, vendorProductIds, assignedProducts);

            // associate any vendorProducts that aren't already associated
            List<Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest> associateRequests = GetVendorProductAssociations(contactId, vendorProductIds, assignedProducts);

            // merge the two lists
            List<Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest> updateRequests = new List<Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest>();
            updateRequests.AddRange(dissociateRequests);
            updateRequests.AddRange(associateRequests);

            // make the update request
            vpdl.UpdateAgentProducts(updateRequests);
        }

        private List<Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest> GetVendorProductDissociations(Guid contactId, IEnumerable<Guid> vendorProductIds, List<Data.CRM.Model.VendorProduct> assignedProducts)
        {
            List<Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest> dissociateRequests = new List<Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest>();
            foreach (Data.CRM.Model.VendorProduct assignedProduct in assignedProducts)
            {
                if (!vendorProductIds.Contains(assignedProduct.VendorProductId))
                {
                    dissociateRequests.Add(new Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest()
                    {
                        UpdateRequestType = Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest.RequestType.Disassociate,
                        VendorAgentId = contactId,
                        VendorProductId = assignedProduct.VendorProductId
                    });
                }
            }
            return dissociateRequests;
        }

        private List<Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest> GetVendorProductAssociations(Guid contactId, IEnumerable<Guid> vendorProductIds, List<Data.CRM.Model.VendorProduct> assignedProducts)
        {

            List<Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest> associateRequests = new List<Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest>();
            foreach (Guid vendorProductId in vendorProductIds)
            {
                bool foundMatch = false;
                foreach (Data.CRM.Model.VendorProduct assignedProduct in assignedProducts)
                {
                    if (vendorProductId == assignedProduct.VendorProductId)
                    {
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch)
                {
                    associateRequests.Add(new Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest()
                    {
                        UpdateRequestType = Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest.RequestType.Associate,
                        VendorAgentId = contactId,
                        VendorProductId = vendorProductId
                    });
                }
            }
            return associateRequests;
        }
    }
}
