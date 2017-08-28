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
        private void SavePhase1ProjectVendorResult(Filter.VendorProductFilterResult _selectedFilterResult)
        {
            ProjectVendorDataLogic _projectVendorDataLogic = new ProjectVendorDataLogic(_defaultAuthenticationRequest);
            ProjectVendor _projectVendor = _projectVendorDataLogic.RetrieveByClientProject(ClientProject.Id, _selectedFilterResult.VendorProductId);

            if (_projectVendor == null)
            {
                VendorProduct _vendorProduct = new VendorProductDataLogic(_defaultAuthenticationRequest).Retrieve(_selectedFilterResult.VendorProductId);
                Vendor _vendor = new AccountDataLogic(_defaultAuthenticationRequest).Retrieve(_vendorProduct.VendorId) as Vendor;

                _projectVendor = new ProjectVendor()
                {
                    Name = ClientProject.ClientProjectName,
                    ClientProjectId = ClientProject.Id,
                    Phase1Result = true,
                    VendorProductId = _selectedFilterResult.VendorProductId,
                    Status = ProjectVendorStatuses.Pending
                };
            }
            _projectVendor.Phase1Result = _selectedFilterResult.Passed;
            _projectVendorDataLogic.Save(_projectVendor);
        }
    }
}
