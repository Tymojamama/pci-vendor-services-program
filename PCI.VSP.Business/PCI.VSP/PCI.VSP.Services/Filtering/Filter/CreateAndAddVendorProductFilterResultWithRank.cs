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
        private void CreateAndAddVendorProductFilterResultWithRank(VendorProduct vendorProduct, int rank)
        {
            Dictionary<Tuple<Guid, Guid>, VendorQuestion> vendorQuestionByQuestionId = GetVendorQuestionMap(vendorProduct);
            List<ClientQuestion> clientQuestionsWithRank = ClientQuestions.Where(z => z.Rank == rank).ToList();
            List<VendorClientQuestion> vendorClientQuestions = FilterProcess(vendorQuestionByQuestionId, clientQuestionsWithRank);

            VendorProductFilterResult vendorProductFilterResult = new VendorProductFilterResult()
            {
                VendorId = vendorProduct.VendorId,
                VendorProductId = vendorProduct.VendorProductId,
                Passed = false,
                CompleteMatch = false,
                IsBenchmark = _benchmarks.Exists(z => z.VendorProductId == vendorProduct.VendorProductId),
                QuestionRank = rank,
                MatchCount = 0,
                VendorClientQuestions = vendorClientQuestions
            };

            vendorProductFilterResult.MatchCount = vendorClientQuestions.Where(z => z.AreMatch).Count();
            vendorProductFilterResult.CompleteMatch = vendorProductFilterResult.Passed = (vendorProductFilterResult.MatchCount == vendorClientQuestions.Count);
            _vendorProductFilterResults.Add(vendorProductFilterResult);

        }
    }
}