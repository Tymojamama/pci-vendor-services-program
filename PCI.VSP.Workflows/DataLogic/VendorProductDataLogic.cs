using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace PCI.VSP.Workflows
{
    class VendorProductDataLogic
    {
        private ICrmService _service;
        private String[] _columnSet = { "vsp_emailthreshold", "vsp_abovethreshold", "vsp_vendorproductid", "vsp_name", "vsp_accountid", "vsp_productid" };

        internal VendorProductDataLogic(ICrmService service)
        {
            _service = service;
        }

        internal VendorProduct Retrieve(Guid vendorProductId)
        {
            BusinessEntity be = _service.Retrieve(VendorProduct._entityName, vendorProductId, new ColumnSet(_columnSet));
            if (be == null) { return null; }

            DynamicEntity de = (DynamicEntity)be;
            if (de == null) { return null; }

            return new VendorProduct(de);
        }
    }
}
