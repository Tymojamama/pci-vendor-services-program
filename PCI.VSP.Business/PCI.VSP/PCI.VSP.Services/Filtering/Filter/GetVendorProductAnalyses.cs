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
        // This method may no longer be used.
        private List<VendorProductAnalysis> __GetVendorProductAnalyses(Guid _clientProjectId, Dictionary<Guid, List<VendorClientQuestion>> _filterComparisons)
        {
            Trace.TraceInformation("Entering " + System.Reflection.MethodBase.GetCurrentMethod().Name);

            List<VendorProductAnalysis> _vendorProductAnalyses = new List<VendorProductAnalysis>();
            foreach (KeyValuePair<Guid, List<VendorClientQuestion>> _keyValuePair in _filterComparisons)
            {
                VendorProductAnalysis _vendorProductAnalysis = new VendorProductAnalysis()
                {
                    VendorId = (_keyValuePair.Value as List<VendorClientQuestion>)[0].VendorQuestion.VendorId,
                    VendorProductId = _keyValuePair.Key,
                    ClientProjectId = _clientProjectId
                };

                foreach (VendorClientQuestion _vendorClientQuestion in _keyValuePair.Value)
                {
                    if (!_vendorClientQuestion.AreMatch)
                    {
                        continue;
                    }

                    _vendorProductAnalysis.Add(_vendorClientQuestion.ClientQuestion.Rank, _vendorClientQuestion.ClientQuestion.QuestionId);
                }
                if (_vendorProductAnalysis.Count > 0)
                {
                    _vendorProductAnalyses.Add(_vendorProductAnalysis);
                }
            }

            Trace.TraceInformation("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().Name);

            return _vendorProductAnalyses;
        }
    }
}
