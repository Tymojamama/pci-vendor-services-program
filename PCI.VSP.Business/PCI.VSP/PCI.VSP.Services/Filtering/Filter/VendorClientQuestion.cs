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
        public class VendorClientQuestion
        {
            private readonly VendorQuestion _vendorQuestion;
            private readonly ClientQuestion _clientQuestion;

            internal VendorClientQuestion(VendorQuestion vendorQuestion, ClientQuestion clientQuestion)
            {
                _vendorQuestion = vendorQuestion;
                _clientQuestion = clientQuestion;
            }

            public VendorQuestion VendorQuestion { get { return _vendorQuestion; } }
            public ClientQuestion ClientQuestion { get { return _clientQuestion; } }
            public Data.CRM.Model.Question.ComparisonTypes ComparisonType { get; internal set; }
            public Data.Enums.DataTypes DataType { get; internal set; }
            public Boolean AreMatch { get; internal set; }
            public bool RequiredMatch { get; internal set; } // Current Only Used For Investment Assumptions
            public Exception CompareException { get; internal set; }
        }
    }
}
