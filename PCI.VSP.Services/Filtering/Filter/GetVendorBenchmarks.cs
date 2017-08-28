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
        private List<ProjectVendor> GetVendorBenchmarks()
        {
            ProjectVendorDataLogic projectVendorDataLogic = new ProjectVendorDataLogic(_getDefaultAuthRequest());
            List<ProjectVendor> list = new List<ProjectVendor>();

            if (FilterPhase == FilterPhases.Phase1)
            {
                list = projectVendorDataLogic.RetrieveFilter1Benchmarks(ClientProject.Id);
            }
            else if (FilterPhase == FilterPhases.Phase2)
            {
                list = projectVendorDataLogic.RetrieveFilter2Benchmarks(ClientProject.Id);
            }

            return list;
        }
    }
}
