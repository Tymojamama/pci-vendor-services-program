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
        private int GetMaximumResults()
        {
            int _maxResults = 0;

            if (_filter.FilterPhase == Filter.FilterPhases.Phase1)
            {
                _maxResults = _clientProjectDataLogic.Retrieve(ClientProject.Id).MaxPhase1Results;
            }
            else if (_filter.FilterPhase == Filter.FilterPhases.Phase2)
            {
                _maxResults = _clientProjectDataLogic.Retrieve(ClientProject.Id).MaxPhase2Results;
            }

            return _maxResults;
        }
    }
}
