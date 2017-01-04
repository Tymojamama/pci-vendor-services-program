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
        public ClientProject ClientProject;

        private AuthenticationRequest _defaultAuthenticationRequest
        {
            get
            {
                return new AuthenticationRequest()
                {
                    Username = Globals.CrmServiceSettings.Username,
                    Password = Globals.CrmServiceSettings.Password
                };
            }
        }
        private ClientProjectDataLogic _clientProjectDataLogic;
        private ClientQuestionDataLogic _clientQuestionDataLogic;
        private Filter _filter;
        private FilterSummary _filterSummary;
        private List<ClientQuestion> _clientQuestions;
        private List<Filter.VendorProductFilterResult> _filterResultsAll;
        private List<Filter.VendorProductFilterResult> _filterResultsSelected;

        public FilterService(Guid _clientProjectId, Filter.FilterPhases _filterPhase)
        {
            _clientProjectDataLogic = new ClientProjectDataLogic(_defaultAuthenticationRequest);
            ClientProject = _clientProjectDataLogic.Retrieve(_clientProjectId);

            _clientQuestionDataLogic = new ClientQuestionDataLogic(_defaultAuthenticationRequest);
            _clientQuestions = _clientQuestionDataLogic.RetrieveForPhase1Filter(ClientProject.Id);

            _filter = new Filter(ClientProject, _filterPhase);
            _filterSummary = new FilterSummary(_filterResultsAll, _filterResultsSelected);
        }

        public string PerformFilter()
        {
            if (_filter.FilterPhase == Filter.FilterPhases.Phase1)
            {
                return PerformPhase1Filter();
            }
            else if (_filter.FilterPhase == Filter.FilterPhases.Phase2)
            {
                return PerformPhase2Filter();
            }
            else
            {
                return String.Empty;
            }
        }
    }
}
