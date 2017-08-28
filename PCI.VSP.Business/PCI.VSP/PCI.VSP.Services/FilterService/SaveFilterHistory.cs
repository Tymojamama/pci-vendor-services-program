using PCI.VSP.Data;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Data.CRM.Model;
using PCI.VSP.Data.Enums;
using PCI.VSP.Services.Filtering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PCI.VSP.Services
{
    public partial class FilterService
    {
        private void SaveFilterHistory()
        {
            Trace.TraceInformation("Entering " + MethodBase.GetCurrentMethod().Name);

            if (_filterResultsAll == null || _filterResultsAll.Count == 0)
            {
                return;
            }

            Dictionary<Guid, VendorQuestion> _vendorQuestionsByQuestionId = new Dictionary<Guid, VendorQuestion>();

            foreach (List<Filter.VendorClientQuestion> _vendorClientQuestions in _filterResultsAll.Select(z => z.VendorClientQuestions).ToList())//filterResults.Matches.Values)
            {
                foreach (Filter.VendorClientQuestion _vendorClientQuestion in _vendorClientQuestions)
                {
                    if (_vendorClientQuestion.VendorQuestion != null && !_vendorQuestionsByQuestionId.ContainsKey(_vendorClientQuestion.VendorQuestion.Id))
                    {
                        _vendorQuestionsByQuestionId.Add(_vendorClientQuestion.VendorQuestion.Id, _vendorClientQuestion.VendorQuestion);
                    }
                }
            }

            foreach (VendorQuestion _vendorQuestion in _vendorQuestionsByQuestionId.Values)
            {
                VendorQuestionHistoryDataLogic _vendorQuestionHistoryDataLogic = new VendorQuestionHistoryDataLogic(_defaultAuthenticationRequest);
                VendorQuestionHistory _vendorQuestionHistory = CreateQuestionHistoryForFilter(_vendorQuestion);
                _vendorQuestionHistoryDataLogic.Save(_vendorQuestionHistory);
            }

            Trace.TraceInformation("Exiting " + MethodBase.GetCurrentMethod().Name);
        }
    }
}
