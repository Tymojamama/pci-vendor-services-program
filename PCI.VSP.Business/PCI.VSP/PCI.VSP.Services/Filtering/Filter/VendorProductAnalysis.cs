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
        public class VendorProductAnalysis : Dictionary<Int32, List<Guid>>
        {
            public Guid VendorId { get; set; }
            public Guid VendorProductId { get; set; }
            public Guid ClientProjectId { get; set; }

            public void Add(Int32 rank, Guid matchingQuestionId)
            {
                if (!this.ContainsKey(rank))
                {
                    List<Guid> matchList = new List<Guid>();
                    matchList.Add(matchingQuestionId);
                    base.Add(rank, matchList);
                }
                else
                {
                    if (!base[rank].Contains(matchingQuestionId))
                        base[rank].Add(matchingQuestionId);
                }
            }
        }
    }
}
