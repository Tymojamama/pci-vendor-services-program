using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.VendorQAImport.LegacyData
{
    public class VSMLegacyDataLogic : BaseDataLogic<VendorQuestion, int>
    {
        public override void Save(VendorQuestion dataObject)
        {
            throw new NotImplementedException();
        }

        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override VendorQuestion Retrieve(int id)
        {
            return Context.VendorQuestions.Where(d => d.VendorQuestionID == id).FirstOrDefault();
        }

        public override List<VendorQuestion> RetrieveAll()
        {
            return Context.VendorQuestions.ToList();
        }

        public List<Vendor> RetrieveAllVendors()
        {
            return Context.Vendors.ToList();
        }

        public List<VendorAnswer> RetrieveAllAnswers()
        {
            return Context.VendorAnswers.ToList();
        }

        public List<VendorProduct> RetrieveAllVendorProducts()
        {
            return Context.VendorProducts.ToList();
        }

        public VendorProduct RetrieveVendorProduct(int id)
        {
            return Context.VendorProducts.Where(d=> d.VendorProductID==id).FirstOrDefault();
        }

        public Vendor RetrieveVendor(int id)
        {
            return Context.Vendors.Where(d => d.VendorID == id).FirstOrDefault();
        }

        public VendorQuestion RetrieveVendorQuestion(int id)
        {
            return Context.VendorQuestions.Where(d => d.VendorQuestionID == id).FirstOrDefault();
        }

        
    }
}
