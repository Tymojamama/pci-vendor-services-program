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
        private void InitializePrivateVariables()
        {
            _benchmarks = GetVendorBenchmarks();
            _clientQuestionDataLogic = new ClientQuestionDataLogic(_getDefaultAuthRequest());
            _comparisonType = Question.ComparisonTypes.Unspecified;
            _ranks = ClientQuestions.Select(cq => cq.Rank).Distinct().OrderBy(rank => rank);
            _vendorProducts = GetVendorProductsForComparison();
            _vendorProductFilterResults = new List<VendorProductFilterResult>();
        }
    }
}
