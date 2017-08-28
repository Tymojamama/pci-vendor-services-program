

namespace PCI.VSP.Web.Enums
{
    public enum QuestionEntityTypes
    {
        Unspecified = 0,
        VendorProfileQuestion = 1,
        VendorProductQuestion = 2,
        ProjectInquiryVendorQuestion = 3,
        ClientQuestion = 4
    }

    public enum CommentTypes
    {
        Unspecified = 0,
        VendorToClient = 1,
        PciToVendor = 2,
        ClientToVendor = 3,
        PciToClient = 4,
        VendorToPci = 5,
        ClientToPci = 6
    }

    public enum UserType
    {
        Unspecified = 0,
        PCI = 1,
        Client = 2,
        Vendor = 3
    }
}