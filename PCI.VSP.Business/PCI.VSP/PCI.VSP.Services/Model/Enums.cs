using System;
using PCI.VSP.Data.Enums;

namespace PCI.VSP.Services.Model.Enums
{

    internal class ClientInquirySummaryStatusesConvertor
    {
        public static String ToString(ProjectVendorStatuses status)
        {
            switch (status)
            {
                case ProjectVendorStatuses.Pending:
                    return "Pending";
                case ProjectVendorStatuses.ClientApproved:
                    return "Client Approved";
                case ProjectVendorStatuses.VendorApproved:
                    return "Vendor Approved";
                case ProjectVendorStatuses.PCI_Approved:
                    return "PCI Approved";
                default:
                    return "Unspecified";
            }
        }
    }
}