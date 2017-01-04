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
        private void SavePhase2ProjectVendorResult(Filter.VendorProductFilterResult _selectedFilterResult)
        {
            ProjectVendorDataLogic _projectVendorDataLogic = new ProjectVendorDataLogic(_defaultAuthenticationRequest);
            ProjectVendor _projectVendor = _projectVendorDataLogic.RetrieveByClientProject(ClientProject.Id, _selectedFilterResult.VendorProductId);

            if (_projectVendor != null)
            {
                _projectVendor.Phase2Result = _selectedFilterResult.Passed;
                _projectVendorDataLogic.Update(_projectVendor);
            }
        }
    }
}
