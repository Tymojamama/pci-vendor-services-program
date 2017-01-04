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
        /// <summary>
        /// Resets previous filter data needed to execute a new filter.
        /// </summary>
        private void ResetFilters()
        {
            Trace.TraceInformation("Entering " + MethodBase.GetCurrentMethod().Name);

            ProjectVendorDataLogic projectVendorDataLogic = new ProjectVendorDataLogic(_defaultAuthenticationRequest);
            List<ProjectVendor> projectVendors = projectVendorDataLogic.RetrieveMultipleByClientProject(ClientProject.Id);

            foreach (ProjectVendor projectVendor in projectVendors)
            {
                if (_filter.FilterPhase == Filter.FilterPhases.Phase1 && projectVendor.Phase1Benchmark == false && projectVendor.Phase2Benchmark == false && projectVendor.Excluded == false)
                {
                    projectVendorDataLogic.Delete(projectVendor.Id);
                }
                else
                {
                    if (_filter.FilterPhase == Filter.FilterPhases.Phase1)
                    {
                        projectVendor.Phase1Result = false;
                    }
                    projectVendor.Phase2Result = false;
                    projectVendorDataLogic.Save(projectVendor);
                }
            }

            Trace.TraceInformation("Exiting " + MethodBase.GetCurrentMethod().Name);
        }
    }
}
