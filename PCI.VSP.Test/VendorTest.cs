using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCI.VSP.Data.CRM.DataLogic;

namespace PCI.VSP.Test
{
    [TestClass]
    public class VendorTest
    {
        private static Guid _vendorId = Guid.Parse("B522D105-BEBF-E111-9748-000423C7D319");
        private static Guid _productId = Guid.Parse("0d380e44-b7c6-e111-a526-000423c7d319");
        private static Guid _vendorProductId = Guid.Parse("0d380e44-b7c6-e111-a526-000423c7d319");
        private static Guid _questionId = Guid.Parse("f2482a97-c9c6-e111-a526-000423c7d319");
        private static Guid _vendorQuestionId = Guid.Parse("357fa368-a3ca-e111-875c-000423c7d319");

        [TestMethod]
        public void RetrieveVendorProductByVendor()
        {
            Globals.InitVspService();
            var result = new VendorProductDataLogic(Globals.GetGenericAuthRequest()).RetrieveVendorProductByVendor(_vendorId);
        }

        //[TestMethod]
        //public void RetrieveVendorProductQuestions()
        //{
        //    Globals.InitVspService();
        //    PCI.VSP.Services.Model.IUser user = new SecurityTest().AuthenticateUser();
        //    Services.VspService service = new Services.VspService();
        //    List<Services.Model.IAccountQuestion> result = service.GetVendorProductQuestions(user.AccountId, user.Id, _vendorProductId);
        //}

        [TestMethod]
        public void RetrieveVendorProfileQuestions()
        {
            Globals.InitVspService();

            PCI.VSP.Services.VspService vspService = new PCI.VSP.Services.VspService();
            PCI.VSP.Services.AuthenticationRequest authRequest = Globals.GetGenericAuthRequest();

            var result = new VendorQuestionDataLogic(authRequest).RetrieveVendorProfileQuestions(Guid.Parse("B522D105-BEBF-E111-9748-000423C7D319"));
        }

        [TestMethod]
        public void UpdateVendorProfileQuestion()
        {
            Globals.InitVspService();
            
            var result = new VendorQuestionDataLogic(Globals.GetGenericAuthRequest()).RetrieveVendorProfileQuestions(Guid.Parse("B522D105-BEBF-E111-9748-000423C7D319"));
            result[0].Answer = "456456";
            new VendorQuestionDataLogic(Globals.GetGenericAuthRequest()).Update(result[0]);
        }

        [TestMethod]
        public void UpdateVendorProductQuestion()
        {
            Globals.InitVspService();
            PCI.VSP.Services.Model.IUser user = new SecurityTest().AuthenticateUser();

            PCI.VSP.Data.CRM.DataLogic.VendorQuestionDataLogic vqdl = new VendorQuestionDataLogic(Globals.GetGenericAuthRequest());
            List<PCI.VSP.Data.CRM.Model.VendorQuestion> vpqs = vqdl.RetrieveVendorProductQuestions(_vendorId, user.Id, _vendorProductId);
            vpqs[0].Answer = "testing";
            
            vqdl.Save(vpqs[0], user.Id, null);
        }

        [TestMethod]
        public void RetrieveVendorProfilePercentComplete()
        {
            Globals.InitVspService();

            PCI.VSP.Services.VspService vspService = new PCI.VSP.Services.VspService();
            PCI.VSP.Services.AuthenticationRequest authRequest = Globals.GetGenericAuthRequest();

            var result = new VendorQuestionDataLogic(authRequest).RetrieveVendorProfilePercentComplete(Guid.Parse("B522D105-BEBF-E111-9748-000423C7D319"));
        }

        [TestMethod]
        public void RetrieveAgentProducts()
        {
            Globals.InitVspService();
            PCI.VSP.Data.CRM.DataLogic.VendorProductDataLogic vpdl = new VendorProductDataLogic(Globals.GetGenericAuthRequest());
            List<PCI.VSP.Data.CRM.Model.VendorProduct> vps = vpdl.RetrieveAgentProducts(_vendorId, new SecurityTest().AuthenticateUser().Id);
        }

        [TestMethod]
        public void RetrieveAccountByName()
        {
            Globals.InitVspService();
            PCI.VSP.Data.CRM.DataLogic.AccountDataLogic adl = new AccountDataLogic(Globals.GetGenericAuthRequest());
            List<Data.CRM.Model.Account> al = adl.RetrieveByName("Scott's Vendor");
        }

        [TestMethod]
        public void GetClientInquiryDashboard()
        {
            Globals.InitVspService();
            PCI.VSP.Services.VspService vspService = new PCI.VSP.Services.VspService();
            PCI.VSP.Services.Model.IUser user = new SecurityTest().AuthenticateUser();

            List<Services.Model.VendorProjectInquirySummary> vcis = vspService.RetrieveVendorProjectInquirySummary(new Guid("b522d105-bebf-e111-9748-000423c7d319"), user.Id);
        }

        [TestMethod]
        public void GetVendorClientInquiries()
        {
            Globals.InitVspService();
            PCI.VSP.Services.VspService vspService = new PCI.VSP.Services.VspService();
            PCI.VSP.Services.Model.IUser user = new SecurityTest().AuthenticateUser();
            List<Services.Model.IAccountQuestion> ql = vspService.GetVendorProjectInquiries(user.AccountId, user.Id, new Guid?());
        }

        [TestMethod]
        public void UpdateAgentProducts()
        {
            Globals.InitVspService();
            PCI.VSP.Data.CRM.DataLogic.VendorProductDataLogic vpdl = new VendorProductDataLogic(Globals.GetGenericAuthRequest());
            List<Data.CRM.DataLogic.VendorProductDataLogic.UpdateAgentProductRequest> vaprl = new List<VendorProductDataLogic.UpdateAgentProductRequest>();

            PCI.VSP.Services.Model.IUser user = new SecurityTest().AuthenticateUser();

            vaprl.Add(new VendorProductDataLogic.UpdateAgentProductRequest()
            {
                UpdateRequestType = VendorProductDataLogic.UpdateAgentProductRequest.RequestType.Associate,
                VendorAgentId = user.Id,
                VendorProductId = _vendorProductId
            });

            vpdl.UpdateAgentProducts(vaprl);

        }

        [TestMethod]
        public void RetrieveProjectVendors()
        {
            Globals.InitVspService();
            Data.CRM.DataLogic.ProjectVendorDataLogic pvdl = new ProjectVendorDataLogic(Globals.GetGenericAuthRequest());
            Services.Model.IUser user = new SecurityTest().AuthenticateUser();
            List<Data.CRM.Model.ProjectVendor> pvl = pvdl.RetrieveMultiple(user.AccountId, user.Id);
        }

        [TestMethod]
        public void RetrieveVendorProductDashboard()
        {
            Globals.InitVspService();
            PCI.VSP.Services.Model.IUser user = new SecurityTest().AuthenticateUser();
            PCI.VSP.Services.VspService vspService = new PCI.VSP.Services.VspService();

            List<Services.Model.VendorProductSummary> vpsl = vspService.GetVendorProductDashboard(user.AccountId, user.Id);
        }
    }
}
