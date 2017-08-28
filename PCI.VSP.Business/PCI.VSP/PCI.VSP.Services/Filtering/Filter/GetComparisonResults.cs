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
        private FilterResults GetComparisonResults(Dictionary<Guid, List<VendorClientQuestion>> _dictionary)
        {
            Trace.TraceInformation("Entering " + System.Reflection.MethodBase.GetCurrentMethod().Name);

            Dictionary<Guid, List<VendorClientQuestion>> _errorDictionary = new Dictionary<Guid, List<VendorClientQuestion>>();
            Dictionary<Guid, List<VendorClientQuestion>> _matchDictionary = new Dictionary<Guid, List<VendorClientQuestion>>();
            Dictionary<Guid, List<VendorClientQuestion>> _mismatchDictionary = new Dictionary<Guid, List<VendorClientQuestion>>();

            // Add errors to _errorDictionary
            foreach (KeyValuePair<Guid, List<VendorClientQuestion>> _keyValuePair in _dictionary)
            {
                List<VendorClientQuestion> _vendorClientQuestions = new List<VendorClientQuestion>();
                foreach (VendorClientQuestion _vendorClientQuestion in _keyValuePair.Value)
                {
                    if (_vendorClientQuestion.CompareException != null)
                    {
                        _vendorClientQuestions.Add(_vendorClientQuestion);
                    }
                }

                if (_vendorClientQuestions.Count > 0)
                {
                    _errorDictionary.Add(_keyValuePair.Key, _vendorClientQuestions);
                }
            }

            // Add matches and mismatches to _matchDictionary and _mismatchDictionary, respectively
            foreach (KeyValuePair<Guid, List<VendorClientQuestion>> _keyValuePair in _dictionary)
            {
                List<VendorClientQuestion> _unmatchedVendorClientQuestions = new List<VendorClientQuestion>();
                List<VendorClientQuestion> _matchedVendorClientQuestions = new List<VendorClientQuestion>();
                foreach (VendorClientQuestion _vendorClientQuestion in _keyValuePair.Value)
                {
                    if (_vendorClientQuestion.AreMatch == false)
                    {
                        _unmatchedVendorClientQuestions.Add(_vendorClientQuestion);
                    }
                    else
                    {
                        _matchedVendorClientQuestions.Add(_vendorClientQuestion);
                    }
                }

                if (_unmatchedVendorClientQuestions.Count > 0)
                {
                    _mismatchDictionary.Add(_keyValuePair.Key, _unmatchedVendorClientQuestions);
                }

                if (_matchedVendorClientQuestions.Count > 0)
                {
                    _matchDictionary.Add(_keyValuePair.Key, _matchedVendorClientQuestions);
                }
            }

            Trace.TraceInformation("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().Name);

            return new FilterResults(_mismatchDictionary, _matchDictionary, _errorDictionary);
        }
    }
}
