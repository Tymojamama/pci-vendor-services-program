using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCI.VSP.Web.CrmIFrames.Model
{
    public class QuestionDataType
    {
        public Int32 Value { get; set; }
        public String Name { get; set; }
    }
    
    internal class QuestionDataTypes : List<QuestionDataType>
    {
        public QuestionDataTypes() : base()
        {
            PopulateDefault();
        }

        private void PopulateDefault()
        {
            this.Clear();
            for (Int32 counter = 0; counter < Properties.Settings.Default.DefaultQuestionDataTypes.Count; counter++)
                this.Add(new QuestionDataType() { Name = Properties.Settings.Default.DefaultQuestionDataTypes[counter], Value = counter + 1 });
        }
    }
}