using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PCI.VSP.Data.Enums;
using PCI.VSP.Data.CRM.Model;

namespace PCI.VSP.Web.CrmIFrames.Model
{

    public class AllowedComparisonType
    {
        private List<DataTypes> _allowedDataTypes;

        internal AllowedComparisonType(Question.ComparisonTypes comparisonType, String wording)
            : base()
        {
            this.ComparisonType = comparisonType;
            this.Wording = wording;
        }

        internal AllowedComparisonType(Question.ComparisonTypes comparisonType, String wording, DataTypes[] allowedDataTypes)
        {
            this.ComparisonType = comparisonType;
            this.Wording = wording;
            this.AllowedDataTypes.AddRange(allowedDataTypes);
        }

        public Question.ComparisonTypes ComparisonType { get; set; }
        public String Wording { get; set; }
        public List<DataTypes> AllowedDataTypes
        {
            get
            {
                if (_allowedDataTypes == null)
                    _allowedDataTypes = new List<DataTypes>();
                return _allowedDataTypes;
            }
            set
            {
                _allowedDataTypes = value;
            }
        }
    }

    public class ComparisonTypeParentMatrix : Dictionary<AnswerTypes, ComparisonTypeMatrix>
    {
        internal ComparisonTypeParentMatrix()
            : base()
        {
            PopulateAllowedComparisons();
        }

        private void PopulateAllowedComparisons()
        {
            this.Clear();

            // Populate allowed comparisons for single-value
            ComparisonTypeMatrix ctm = new ComparisonTypeMatrix();
            ctm.Add(AnswerTypes.SingleValue, new AllowedComparisonType(Question.ComparisonTypes.EqualTo, "Vendor = Client",
                new DataTypes[] { DataTypes.Date, DataTypes.Integer, DataTypes.Money, DataTypes.Yes_No }));
            ctm.Add(AnswerTypes.SingleValue, new AllowedComparisonType(Question.ComparisonTypes.GreaterThanorEqualTo, "Vendor >= Client",
                new DataTypes[] { DataTypes.Date, DataTypes.Integer, DataTypes.Money }));
            ctm.Add(AnswerTypes.SingleValue, new AllowedComparisonType(Question.ComparisonTypes.LessThanOrEqualTo, "Vendor <= Client",
                new DataTypes[] { DataTypes.Date, DataTypes.Integer, DataTypes.Money }));
            ctm.Add(AnswerTypes.SingleValue, new AllowedComparisonType(Question.ComparisonTypes.AnyMatch, "Contains text like",
                new DataTypes[] { DataTypes.Text }));
            ctm.Add(AnswerTypes.SingleValue, new AllowedComparisonType(Question.ComparisonTypes.CompleteMatch, "Contains entirety of text",
                new DataTypes[] { DataTypes.Text }));
            ctm.Add(AnswerTypes.MultiValue, new AllowedComparisonType(Question.ComparisonTypes.AnyMatch, "At least one value exists in both"));
            ctm.Add(AnswerTypes.MultiValue, new AllowedComparisonType(Question.ComparisonTypes.CompleteMatch, "Lists are identical"));
            ctm.Add(AnswerTypes.Range, new AllowedComparisonType(Question.ComparisonTypes.WithinRange, "Client Within Range"));
            this.Add(AnswerTypes.SingleValue, ctm);

            // populate allowed comparisons for multi-value
            ctm = new ComparisonTypeMatrix();
            ctm.Add(AnswerTypes.SingleValue, new AllowedComparisonType(Question.ComparisonTypes.AnyMatch, "At least one value exists in both"));
            ctm.Add(AnswerTypes.MultiValue, new AllowedComparisonType(Question.ComparisonTypes.AnyMatch, "At least one value exists in both"));
            ctm.Add(AnswerTypes.MultiValue, new AllowedComparisonType(Question.ComparisonTypes.CompleteMatch, "Lists are identical"));
            this.Add(AnswerTypes.MultiValue, ctm);

            // populate allowed comparisons for range
            ctm = new ComparisonTypeMatrix();
            ctm.Add(AnswerTypes.Range, new AllowedComparisonType(Question.ComparisonTypes.EqualTo, "Overlaps"));
            ctm.Add(AnswerTypes.Range, new AllowedComparisonType(Question.ComparisonTypes.GreaterThanorEqualTo, "Vendor Within Client"));
            ctm.Add(AnswerTypes.Range, new AllowedComparisonType(Question.ComparisonTypes.LessThanOrEqualTo, "Client Within Vendor"));
            ctm.Add(AnswerTypes.SingleValue, new AllowedComparisonType(Question.ComparisonTypes.WithinRange, "Within Range"));
            this.Add(AnswerTypes.Range, ctm);
        }
    }

    public class ComparisonTypeMatrix : Dictionary<AnswerTypes, List<AllowedComparisonType>>
    {
        public void Add(AnswerTypes answerType, AllowedComparisonType comparisonType)
        {
            if (!this.ContainsKey(answerType))
            {
                List<AllowedComparisonType> act = new List<AllowedComparisonType>(new AllowedComparisonType[] { comparisonType });
                this.Add(answerType, act);
            }
            else
            {
                this[answerType].Add(comparisonType);
            }
        }
    }

    public class ComparisonMatrix : Dictionary<DataTypes, List<AnswerTypes>>
    {
        public ComparisonMatrix() : base()
        {
            PopulateAllowedComparisons();
        }

        private void PopulateAllowedComparisons()
        {
            this.Clear();

            this.Add(DataTypes.Integer, new List<AnswerTypes>(new AnswerTypes[] { AnswerTypes.Range, AnswerTypes.SingleValue }));
            this.Add(DataTypes.Money, new List<AnswerTypes>(new AnswerTypes[] { AnswerTypes.Range, AnswerTypes.SingleValue }));
            this.Add(DataTypes.Date, new List<AnswerTypes>(new AnswerTypes[] { AnswerTypes.Range, AnswerTypes.SingleValue }));
            this.Add(DataTypes.Text, new List<AnswerTypes>(new AnswerTypes[] { AnswerTypes.SingleValue, AnswerTypes.MultiValue }));
            this.Add(DataTypes.Choice, new List<AnswerTypes>(new AnswerTypes[] { AnswerTypes.SingleValue, AnswerTypes.MultiValue }));
            this.Add(DataTypes.Yes_No, new List<AnswerTypes>(new AnswerTypes[] { AnswerTypes.SingleValue }));
        }

    }
}