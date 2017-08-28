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
        private void WriteVendorClientQuestion(Filter.VendorClientQuestion _vendorClientQuestion)
        {
            ClientQuestion _clientQuestion = _vendorClientQuestion.ClientQuestion;
            VendorQuestion _vendorQuestion = _vendorClientQuestion.VendorQuestion;

            StringBuilder.Append("Question Type: ").AppendLine(_clientQuestion.QuestionType.ToString());
            StringBuilder.Append("Question Data Type: ").AppendLine(_clientQuestion.QuestionDataType.ToString());
            StringBuilder.Append("Client Wording: ").AppendLine(_clientQuestion.ClientWording);
            StringBuilder.Append("Client Answer: ").AppendLine(String.Join(", ", _clientQuestion.Answer.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)));
            if (!String.IsNullOrWhiteSpace(_clientQuestion.AlternateAnswer))
            {
                StringBuilder.Append("ClientAlternateAnswer: ").AppendLine(_clientQuestion.AlternateAnswer);
            }

            StringBuilder.Append("Comparison Type: ").AppendLine(_vendorClientQuestion.ComparisonType.ToString());
            StringBuilder.Append("Vendor Wording: ").AppendLine((_vendorQuestion != null ? _vendorQuestion.VendorWording : string.Empty));
            StringBuilder.Append("Vendor Answer: ").AppendLine((_vendorQuestion != null ? String.Join(", ", _vendorQuestion.Answer.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)) : string.Empty));
            if (_vendorClientQuestion.VendorQuestion != null && !String.IsNullOrWhiteSpace(_vendorQuestion.AlternateAnswer))
            {
                StringBuilder.Append("Vendor Alternate Answer: ").AppendLine(_vendorQuestion.AlternateAnswer);
            }

            StringBuilder.Append("Did The Answers Match: ").AppendLine(_vendorClientQuestion.AreMatch.ToString());
            StringBuilder.AppendLine();
        }
    }
}
