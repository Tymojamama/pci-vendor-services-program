using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Data.CRM.Model;
using System.Diagnostics;

namespace PCI.VSP.Services.Filtering
{
    public partial class Filter
    {
        private Dictionary<Tuple<Guid, Guid>, VendorQuestion> GetVendorQuestionMap(VendorProduct vendorProduct)
        {
            Dictionary<Tuple<Guid, Guid>, VendorQuestion> dictionary = new Dictionary<Tuple<Guid, Guid>, VendorQuestion>();
            VendorQuestionDataLogic vendorQuestionDataLogic = new VendorQuestionDataLogic(_getDefaultAuthRequest());

            if (FilterPhase == FilterPhases.Phase1)
            {
                List<VendorQuestion> vendorQuestions = vendorQuestionDataLogic.RetrieveFilter1VendorQuestions(vendorProduct.VendorId, vendorProduct.VendorProductId);
                dictionary = vendorQuestions.ToDictionary(vq => new Tuple<Guid, Guid>(vq.QuestionId, vq.Id));
            }
            else if (FilterPhase == FilterPhases.Phase2)
            {
                List<VendorQuestion> vendorQuestions = vendorQuestionDataLogic.RetrieveFilter2VendorQuestions(vendorProduct.VendorId, vendorProduct.VendorProductId);
                dictionary = vendorQuestions.ToDictionary(vq => new Tuple<Guid, Guid>(vq.QuestionId, vq.Id));
            }

            return dictionary;
        }
    }
}
