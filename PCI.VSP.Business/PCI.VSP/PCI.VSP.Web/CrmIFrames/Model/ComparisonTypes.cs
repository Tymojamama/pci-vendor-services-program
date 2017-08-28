using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PCI.VSP.Data.Enums;

namespace PCI.VSP.Web.CrmIFrames.Model
{
    public class ComparisonType
    {
        public Int32 Value { get; set; }
        public String Name { get; set; }
    }

    public class ComparisonTypes : List<ComparisonType>
    {

        internal ComparisonTypes(FilterCriteria fc)
            : base()
        {
            PopulateFiltered(fc);
        }

        private void PopulateFiltered(FilterCriteria fc)
        {
            this.Clear();
            ComparisonMatrix cm = new ComparisonMatrix();
            
            if (!cm.ContainsKey(fc.QuestionDataType)) { return; }
            List<AnswerTypes> allowedAnswerTypes = cm[fc.QuestionDataType];

            ComparisonTypeParentMatrix ctpm = new ComparisonTypeParentMatrix();
            ComparisonTypeMatrix ctm = ctpm[fc.ClientResponseType];
            List<AllowedComparisonType> allowedComparisonTypes = ctm[fc.VendorResponseType];

            foreach (AllowedComparisonType allowedComparisonType in allowedComparisonTypes)
            {
                if (allowedComparisonType.AllowedDataTypes.Count > 0 && !allowedComparisonType.AllowedDataTypes.Contains(fc.QuestionDataType)) { continue; }
                this.Add(new ComparisonType() { Name = allowedComparisonType.Wording, Value = Convert.ToInt32(allowedComparisonType.ComparisonType) });
            }
        }

    }
}