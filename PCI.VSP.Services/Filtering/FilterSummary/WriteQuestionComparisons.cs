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
        private void WriteQuestionComparisons()
        {
            List<int> ranks = AllResultsFromFilter.Select(z => z.QuestionRank).Distinct().ToList();

            foreach (var rank in ranks)
            {
                StringBuilder.Append("-- RANK ").Append(rank).Append(" RESULTS --").AppendLine().AppendLine();

                foreach (var vpfr in AllResultsFromFilter.Where(z => z.QuestionRank == rank))
                {
                    WriteVendorProductRecord(vpfr, rank);
                }
            }
        }
    }
}
