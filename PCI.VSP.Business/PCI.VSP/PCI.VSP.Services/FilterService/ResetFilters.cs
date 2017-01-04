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
        private void ResetFilters()
        {
            Trace.TraceInformation("Entering " + MethodBase.GetCurrentMethod().Name);

            ProjectVendorDataLogic _projectVendorDataLogic = new ProjectVendorDataLogic(_defaultAuthenticationRequest);
            List<ProjectVendor> _projectVendors = _projectVendorDataLogic.RetrieveMultipleByClientProject(ClientProject.Id);

            foreach (ProjectVendor _projectVendor in _projectVendors)
            {
                if (_filter.FilterPhase == Filter.FilterPhases.Phase1 && _projectVendor.Phase1Benchmark == false && _projectVendor.Phase2Benchmark == false && _projectVendor.Excluded == false)
                {
                    _projectVendorDataLogic.Delete(_projectVendor.Id);
                }
                else
                {
                    if (_filter.FilterPhase == Filter.FilterPhases.Phase1)
                    {
                        _projectVendor.Phase1Result = false;
                    }
                    _projectVendor.Phase2Result = false;
                    _projectVendorDataLogic.Save(_projectVendor);
                }
            }

            Trace.TraceInformation("Exiting " + MethodBase.GetCurrentMethod().Name);
        }
    }
}
