using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCI.VSP.Data.Enums;

namespace PCI.VSP.Services.Model
{
    public class VendorProjectInquirySummary
    {
        public Guid ProjectVendorId { get; set; }
        public Guid VendorProductId { get; set; }
        public String ProductName { get; set; }
        public DateTime? LastUpdated { get; set; }
        public Decimal PercentComplete { get; set; }
        internal Guid ClientProjectId { get; set; }
        public String VendorWording { get; set; }
        public String Status { get; set; }
        public string ClientProjectName { get; set; }

        internal static ProjectVendorStatuses CalculateStatus(List<VendorQuestion> vql)
        {
            if (vql == null || vql.Count == 0) { return ProjectVendorStatuses.Unspecified; }

            // configure status priorities
            Dictionary<Int32, ProjectVendorStatuses> priorities = GetStatusPriorities();
            Dictionary<Data.Enums.AccountQuestionStatuses, Int32> vqPriorities = GetVqStatusPriorities();
            Int32 lowestStatus = 99;

            // get lowest status
            foreach (VendorQuestion vq in vql)
            {
                if (vqPriorities[vq.Status] < lowestStatus)
                    lowestStatus = vqPriorities[vq.Status];
            }

            return priorities[lowestStatus];
        }

        private static Dictionary<Int32, ProjectVendorStatuses> GetStatusPriorities()
        {
            Dictionary<Int32, ProjectVendorStatuses> priorities = new Dictionary<Int32, ProjectVendorStatuses>();
            priorities.Add(99, ProjectVendorStatuses.Unspecified);
            priorities.Add(0, ProjectVendorStatuses.Pending);
            priorities.Add(1, ProjectVendorStatuses.ClientApproved);
            priorities.Add(2, ProjectVendorStatuses.VendorApproved);
            priorities.Add(3, ProjectVendorStatuses.PCI_Approved);
            return priorities;
        }

        private static Dictionary<Data.Enums.AccountQuestionStatuses, Int32> GetVqStatusPriorities()
        {
            Dictionary<Data.Enums.AccountQuestionStatuses, Int32> priorities = new Dictionary<Data.Enums.AccountQuestionStatuses,int>();
            priorities.Add(Data.Enums.AccountQuestionStatuses.Unspecified, 99);
            priorities.Add(Data.Enums.AccountQuestionStatuses.Answered, 0);
            priorities.Add(Data.Enums.AccountQuestionStatuses.Rejected, 1);
            priorities.Add(Data.Enums.AccountQuestionStatuses.AccountConfirmed, 2);
            priorities.Add(Data.Enums.AccountQuestionStatuses.PCI_Confirmed, 3);
            return priorities;
        }
    }
}
