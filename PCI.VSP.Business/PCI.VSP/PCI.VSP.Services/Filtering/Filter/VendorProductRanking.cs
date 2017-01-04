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
        public class VendorProductRanking
        {
            public int QuestionRank { get; set; }
            public bool Passed { get; set; }
            public int MatchCount { get; set; }
            public List<VendorClientQuestion> VendorClientQuestions { get; set; }
        }
    }
}
