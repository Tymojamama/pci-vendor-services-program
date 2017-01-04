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
        private List<VendorProduct> GetVendorProductsForComparison()
        {
            VendorProductDataLogic _vendorProductDataLogic = new VendorProductDataLogic(_getDefaultAuthRequest());
            List<VendorProduct> _list = new List<VendorProduct>();

            switch (FilterPhase)
            {
                case FilterPhases.Phase1:
                    _list = _vendorProductDataLogic.RetrieveAll();
                    break;
                case FilterPhases.Phase2:
                    _list = _vendorProductDataLogic.RetrieveForPhase2Filter(ClientProject.Id);
                    break;
            }

            return _list;
        }
    }
}
