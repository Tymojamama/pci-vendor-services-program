using PCI.VSP.Data;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Data.CRM.Model;
using PCI.VSP.Data.Enums;
using PCI.VSP.Services.Filtering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PCI.VSP.Services.Filtering
{
    public partial class FilterSummary
    {
        private void WriteVendorProductRecord(Filter.VendorProductFilterResult vpfr, int rank)
        {
            VendorProduct vp = VspService.GetVendorProduct(vpfr.VendorProductId);
            Vendor v = null;

            try
            {
                v = VspService.GetVendor(vp.VendorId);
            }
            catch
            {

            }

            if (v != null)
            {
                StringBuilder.AppendLine("Vendor: " + v.Name);
            }
            if (vp != null)
            {
                StringBuilder.Append("Vendor Product");
                if (vpfr.IsBenchmark) StringBuilder.Append(" (Benchmark)");
                StringBuilder.AppendLine(": " + vp.VendorProductName);
            }

            StringBuilder.Append(vpfr.MatchCount).Append((vpfr.CompleteMatch ? " (All)" : string.Empty)).Append(" Matched Questions At Question Rank ").Append(vpfr.QuestionRank).AppendLine();
            StringBuilder.Append((vpfr.Passed ? "Passed" : "Failed")).Append(" At Question Rank ").Append(vpfr.QuestionRank).AppendLine().AppendLine();
            StringBuilder.Append("-- BEGINNING OF ").Append(vp.VendorProductName).Append(" RANK ").Append(rank).Append(" QUESTIONS --").AppendLine().AppendLine();

            foreach (var vcq in vpfr.VendorClientQuestions)
            {
                WriteVendorClientQuestion(vcq);
            }

            StringBuilder.Append("-- END OF ").Append(vp.VendorProductName).Append(" RANK ").Append(rank).Append(" QUESTIONS --").AppendLine().AppendLine();
        }
    }
}
