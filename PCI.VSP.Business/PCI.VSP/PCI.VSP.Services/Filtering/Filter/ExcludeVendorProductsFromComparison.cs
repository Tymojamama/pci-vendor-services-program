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
        private void ExcludeCurrentVendorProductsFromComparison()
        {
            ProjectVendorDataLogic projectVendorDataLogic = new ProjectVendorDataLogic(_getDefaultAuthRequest());
            List<ProjectVendor> excludedProjectVendors = projectVendorDataLogic.RetrieveProjectVendors(ClientProject.Id, true);

            foreach (ProjectVendor excludedProjectVendor in excludedProjectVendors)
            {
                Guid excludedVendorID = _vendorProducts.Where(z => z.VendorProductId == excludedProjectVendor.VendorProductId).Select(z => z.VendorId).FirstOrDefault();
                if (excludedVendorID == null)
                {
                    continue;
                }

                List<VendorProduct> excludedVendorProducts = _vendorProducts.Where(z => z.VendorId == excludedVendorID).ToList();
                foreach (VendorProduct excludedVendorProduct in excludedVendorProducts)
                {
                    _vendorProducts.Remove(excludedVendorProduct);
                }
            }
        }
    }
}
