using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PCI.VSP.Data.Enums;

namespace PCI.VSP.Web.CrmIFrames.Model
{
    public class VendorResponseType
    {
        public Int32 Value { get; set; }
        public String Name { get; set; }
    }

    internal class VendorResponseTypes : List<VendorResponseType>
    {
        public VendorResponseTypes(Model.FilterCriteria fc)
            : base()
        {
            PopulateFiltered(fc);
        }

        public VendorResponseTypes()
            : base()
        {
            PopulateDefault();
        }

        private void PopulateDefault()
        {
            this.Clear();

            for (Int32 counter = 0; counter < Properties.Settings.Default.DefaultVendorResponseTypes.Count; counter++)
            {
                this.Add(new VendorResponseType() { Name = Properties.Settings.Default.DefaultVendorResponseTypes[counter], Value = counter + 1 });
            }
        }

        private void PopulateFiltered(Model.FilterCriteria fc)
        {
            this.Clear();
            ComparisonMatrix cm = new ComparisonMatrix();

            if (!cm.ContainsKey(fc.QuestionDataType)) { return; }
            List<AnswerTypes> allowedAnswerTypes = cm[fc.QuestionDataType];

            if (fc.ClientResponseType == AnswerTypes.Unspecified)
            {
                foreach (AnswerTypes answerType in allowedAnswerTypes)
                {
                    Int32 value = Convert.ToInt32(answerType);
                    this.Add(new VendorResponseType() { Name = Properties.Settings.Default.DefaultVendorResponseTypes[value - 1], Value = value });
                }
                return;
            }

            ComparisonTypeParentMatrix ctpm = new ComparisonTypeParentMatrix();
            foreach (AnswerTypes answerType in ctpm[fc.ClientResponseType].Keys)
            {
                if (!allowedAnswerTypes.Contains(answerType)) { continue; }
                Int32 value = Convert.ToInt32(answerType);
                this.Add(new VendorResponseType() { Name = Properties.Settings.Default.DefaultVendorResponseTypes[value - 1], Value = value });
            }
        }

    }
}