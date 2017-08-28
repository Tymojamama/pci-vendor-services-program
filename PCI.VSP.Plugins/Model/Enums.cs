namespace PCI.VSP.Plugins.Model.Enums
{
    public enum AnswerTypes
    {
        Unspecified = 0,
        SingleValue = 1,
        MultiValue = 2,
        Range = 3,
        None = 4,
        TieredFee = 5
    }

    public enum FilterCategory
    {
        Unspecified = 0,
        Filter1 = 1,
        Filter2 = 2
    }

    public enum InvalidAnswerReasons
    {
        Unspecified = 0,
        Invalid = 1,
        Expired = 2,
        WordingChange = 3
    }

    public enum VendorMonitoringAnswerTypes
    {
        None = 20000,
        Estimate = 20001,
        Actual = 20002,
        Both = 20003
    }

    public enum DataTypes
    {
        Unspecified = 0,
        Integer = 1,
        Text = 2,
        Yes_No = 3,
        Money = 4,
        Choice = 5,
        Date = 6
    }

    public enum QuestionTypes
    {
        Unspecified = 0,
        SearchQuestion_Filter1 = 1, // Filter 1
        PlanAssumption = 2, // Filter 1
        Fee = 3,
        InvestmentAssumption = 4, // Filter 2
        ProjectSpecificQuestion = 5, // Filter 2
        SearchQuestion_Filter2 = 6 // Filter 2
    }

    public enum VendorQuestionStatuses
    {
        Unspecified = 0,
        Answered = 1,
        VendorConfirmed = 3,
        Rejected = 4,
        PCI_Confirmed = 5
    }

    public enum ProjectVendorStatuses
    {
        Unspecified = 0,
        Pending = 1,
        ClientApproved = 3,
        VendorApproved = 6,
        PCI_Approved = 7
    }

    public enum EntityName
    {
        Unspecified = 0,
        NotMapped = 1,
        Account = 2,
        VendorProduct = 3
    }

    public enum AttributeDataType
    {
        Unspecified = 0,
        nvarchar = 1,
        integer = 2,
        bit = 3,
        datetime = 4,
        decimaltype = 5,
        money = 6
    }

    public enum FeeType
    {
        Unspecified = 0,
        Fixed = 1,
        PerOccurrence = 2,
        AssetBased = 3
    }

    public enum RevenueSharingCalculationTypes
    {
        Unspecified = 0,
        BasisPoints = 1,
        Both = 2,
        GreaterThan = 2,
        LesserOf = 3,
        NA = 4,
        PerHead = 5
    }

    public enum AccountQuestionStatuses
    {
        Unspecified = 0,
        Answered = 1,
        AccountConfirmed = 3,
        Rejected = 4,
        PCI_Confirmed = 5
    }

    public enum ProjectTypes
    {
        Search = 1,
        VendorMonitoring = 2
    }

    public class ProjectVendorStatusesConvertor
    {
        public static string ToString(ProjectVendorStatuses status)
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