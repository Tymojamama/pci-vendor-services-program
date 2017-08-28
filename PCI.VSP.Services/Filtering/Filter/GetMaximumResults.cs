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
        private void GetMaximumResults(out Int32? _maxResults)
        {
            ClientProjectDataLogic _clientProjectDataLogic = new ClientProjectDataLogic(_getDefaultAuthRequest());
            ClientProject _clientProject = _clientProjectDataLogic.Retrieve(ClientProject.Id);
            _maxResults = new Int32?();

            if (_clientProject != null)
            {
                switch (FilterPhase)
                {
                    case FilterPhases.Phase1:
                        _maxResults = _clientProject.MaxPhase1Results;
                        break;
                    case FilterPhases.Phase2:
                        _maxResults = _clientProject.MaxPhase2Results;
                        break;
                }
            }

            if (_maxResults.HasValue && _maxResults.Value == 0)
            {
                _maxResults = new Int32?();
            }
        }

        private void GetMaximumResults(out int _maxResults)
        {
            _maxResults = 0;

            if (ClientProject != null)
            {
                switch (FilterPhase)
                {
                    case FilterPhases.Phase1:
                        _maxResults = ClientProject.MaxPhase1Results;
                        break;
                    case FilterPhases.Phase2:
                        _maxResults = ClientProject.MaxPhase2Results;
                        break;
                }
            }
        }
    }
}
