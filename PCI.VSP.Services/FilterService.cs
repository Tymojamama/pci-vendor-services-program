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

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterService"/> class.
        /// </summary>
        /// <param name="clientProjectId">Used to get related data such as client questions.</param>
        /// <param name="filterPhase">Used to perform specific methods related to the <see cref="FilterPhases"/></param>
        public FilterService(Guid clientProjectId, Filter.FilterPhases filterPhase)
        {
            _clientProjectDataLogic = new ClientProjectDataLogic(_defaultAuthenticationRequest);
            ClientProject = _clientProjectDataLogic.Retrieve(clientProjectId);

            _clientQuestionDataLogic = new ClientQuestionDataLogic(_defaultAuthenticationRequest);
            _clientQuestions = _clientQuestionDataLogic.RetrieveForPhase1Filter(ClientProject.Id);

            _filter = new Filter(ClientProject, filterPhase);
            _filterSummary = new FilterSummary(_filterResultsAll, _filterResultsSelected);
        }

        /// <summary>
        /// Executes a filter based on the current filter phase of the instance.
        /// </summary>
        /// <returns></returns>
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
