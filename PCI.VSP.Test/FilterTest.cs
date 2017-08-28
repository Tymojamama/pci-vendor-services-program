using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PCI.VSP.Test
{
    [TestClass]
    public class FilterTest
    {
        //private static Guid _clientProjectId = Guid.Parse("{2CB4C063-D5DC-E111-875C-000423C7D319}"); // Scott's Mismatched Client Project

        //private static Guid _clientProjectId = Guid.Parse("B89D193F-0CD0-E111-875C-000423C7D319"); // Scott's Client Project
        // {19B56457-C4E7-E111-875C-000423C7D319}

        private static Guid _clientProjectId = Guid.Parse("{19B56457-C4E7-E111-875C-000423C7D319}"); // Lexicon, Inc. 401(k) Plan Vendor Search
        
        //[TestMethod]
        //public void TestFilterPhase1()
        //{
        //    Globals.InitVspService();
        //    Services.VspService service = new Services.VspService();
        //    Services.Filtering.FilterResults filterResults = service.PerformPhase1Filter(_clientProjectId);
        //}

        //[TestMethod]
        //public void TestFilterPhase2()
        //{
        //    Globals.InitVspService();
        //    Services.VspService service = new Services.VspService();
        //    Services.Filtering.FilterResults filterResults = service.PerformPhase2Filter(_clientProjectId);
        //}

    }
}
