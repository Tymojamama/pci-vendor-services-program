using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace PCI.VSP.Workflows
{
    class VendorProfileDataLogic
    {
        private ICrmService _service;

        internal VendorProfileDataLogic(ICrmService service)
        {
            _service = service;
        }

        internal VendorProfile Retrieve(Guid vendorId)
        {
            BusinessEntity be = _service.Retrieve(VendorProfile._entityName, vendorId, new AllColumns());
            if (be == null) { return null; }

            DynamicEntity de = (DynamicEntity)be;
            if (de == null) { return null; }

            return new VendorProfile(de);
        }
    }
}
