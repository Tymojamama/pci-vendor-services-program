using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Data.CRM.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PCI.VSP.Services.Filtering
{
    public partial class Filter
    {
        public ClientProject ClientProject;
        public FilterPhases FilterPhase;
        public List<ClientQuestion> ClientQuestions;

        private AuthenticationRequest _getDefaultAuthRequest()
        {
            return new AuthenticationRequest()
            {
                Username = Data.Globals.CrmServiceSettings.Username,
                Password = Data.Globals.CrmServiceSettings.Password
            };
        }
        private ClientQuestionDataLogic _clientQuestionDataLogic;
        private Int32 _maximumResults = 0;
        private Int32 _totalVendorCount = 0;
        private IOrderedEnumerable<Int32> _ranks;
        private List<ProjectVendor> _benchmarks;
        private List<VendorProduct> _vendorProducts;
        private List<VendorProductFilterResult> _vendorProductFilterResults;
        private Question.ComparisonTypes _comparisonType;

        public Filter(ClientProject _clientProject, FilterPhases _filterPhase)
        {
            ClientProject = _clientProject;
            FilterPhase = _filterPhase;

            GetClientQuestions();
            InitializePrivateVariables();
        }

        /// <summary>
        /// Execute the Filtering Process
        /// </summary>
        /// <param name="clientProjectId">Client Project ID</param>
        /// <param name="cql">List of Client Questions</param>
        /// <param name="filterPhase">Filter Phase</param>
        /// <returns>List of Vendor Product Filter Result</returns>
        public List<VendorProductFilterResult> ExecuteFilter()
        {
            Trace.TraceInformation("Entering " + MethodBase.GetCurrentMethod().Name);

            GetMaximumResults(out _maximumResults);

            if (StopFilterExecution())
            {
                return null;
            }

            SetClientQuestionDefaultRanksTo99();
            ExcludeCurrentVendorProductsFromComparison();
            FilterProductsByRanksAndMatches();

            SetTotalVendorCount();

            if (_totalVendorCount < _maximumResults)
            {
                AddNextBestMatches();

            }

            Trace.TraceInformation("Exiting " + MethodBase.GetCurrentMethod().Name);

            return _vendorProductFilterResults;
        }
    }
}
