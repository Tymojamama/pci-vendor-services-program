using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using PCI.VSP.Data.Enums;

namespace PCI.VSP.Web.CrmIFrames.Model
{
    public class ClientResponseType
    {
        public Int32 Value { get; set; }
        public String Name { get; set; }
    }

    internal class ClientResponseTypes : List<ClientResponseType>
    {
        public ClientResponseTypes()
            : base()
        {
            PopulateDefault();
        }

        public ClientResponseTypes(Model.FilterCriteria fc) : base()
        {
            PopulateFiltered(fc);
        }

        private void PopulateDefault()
        {
            this.Clear();
            for (Int32 counter = 0; counter < Properties.Settings.Default.DefaultClientResponseTypes.Count; counter++)
                this.Add(new ClientResponseType() { Name = Properties.Settings.Default.DefaultClientResponseTypes[counter], Value = counter + 1 });
        }

        private void PopulateFiltered(Model.FilterCriteria fc)
        {
            this.Clear();
            ComparisonMatrix cm = new ComparisonMatrix();

            if (!cm.ContainsKey(fc.QuestionDataType)) { return; }
            List<AnswerTypes> allowedAnswerTypes = cm[fc.QuestionDataType];

            if (fc.VendorResponseType == AnswerTypes.Unspecified)
            {
                foreach (AnswerTypes answerType in allowedAnswerTypes)
                {
                    Int32 value = Convert.ToInt32(answerType);
                    this.Add(new ClientResponseType() { Name = Properties.Settings.Default.DefaultClientResponseTypes[value - 1], Value = value });
                }
                return;
            }

            ComparisonTypeParentMatrix ctpm = new ComparisonTypeParentMatrix();
            foreach (AnswerTypes answerType in ctpm[fc.VendorResponseType].Keys)
            {
                if (!allowedAnswerTypes.Contains(answerType)) { continue; }
                Int32 value = Convert.ToInt32(answerType);
                this.Add(new ClientResponseType() { Name = Properties.Settings.Default.DefaultClientResponseTypes[value - 1], Value = value });
            }
        }
    }
}