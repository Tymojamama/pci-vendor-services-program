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
    public class Comparison
    {
        Filter Filter;
        Question.ComparisonTypes ComparisonType;

        /// <summary>
        /// Filter helper class used to compare vendor and client questions.
        /// </summary>
        /// <param name="_filter">Filter instance that is requesting comparison.</param>
        /// <param name="_comparisonType">Type of comparison for the two questions</param>
        public Comparison(Filter _filter, Question.ComparisonTypes _comparisonType)
        {
            Filter = _filter;
            ComparisonType = _comparisonType;
        }

        /// <summary>
        /// Compare a Vendor and Client Question
        /// </summary>
        /// <param name="comparisonType">Question Comparison Type</param>
        /// <param name="aq1">Client Question</param>
        /// <param name="aq2">Vendor Question</param>
        /// <returns>Whether or not the answers were the same</returns>
        public bool Compare(Model.IAccountQuestion aq1, Model.IAccountQuestion aq2)
        {
            Trace.TraceInformation("Entering " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            ValidateComparison(aq1, aq2);
            bool result = false;
            switch (aq1.QuestionDataType)
            {
                case Data.Enums.DataTypes.Choice:
                    result = CompareChoice(aq1, aq2);
                    break;
                case Data.Enums.DataTypes.Date:
                    result = CompareDate(aq1, aq2);
                    break;
                case Data.Enums.DataTypes.Integer:
                case Data.Enums.DataTypes.Money:
                case Data.Enums.DataTypes.Double:
                    result = CompareNumeric(aq1, aq2);
                    break;
                case Data.Enums.DataTypes.Yes_No:
                    result = CompareBoolean(aq1, aq2);
                    break;
                case Data.Enums.DataTypes.Text:
                    result = CompareText(aq1, aq2);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported QuestionDataType: " + aq1.QuestionDataType.ToString());
            }
            Trace.TraceInformation("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            return result;
        }

        private void ValidateComparison(Model.IAccountQuestion aq1, Model.IAccountQuestion aq2)
        {
            if (aq1.QuestionId != aq2.QuestionId)
                throw new InvalidOperationException("Unable to compare IAccountQuestions with different QuestionIds.");
            if (aq1.QuestionDataType != aq2.QuestionDataType)
                throw new InvalidOperationException("Unable to compare different QuestionDataTypes.");
        }


        private bool CompareChoice(Model.IAccountQuestion aq1, Model.IAccountQuestion aq2)
        {
            return CompareMultiLineText(aq1.Answer, aq2.Answer);
        }

        private bool CompareMultiLineText(String a1, String a2)
        {
            System.Collections.Specialized.StringCollection a1s = new System.Collections.Specialized.StringCollection();
            a1s.AddRange(this.SplitMultiLineText(a1));

            System.Collections.Specialized.StringCollection a2s = new System.Collections.Specialized.StringCollection();
            a2s.AddRange(this.SplitMultiLineText(a2));

            switch (ComparisonType)
            {
                case Data.CRM.Model.Question.ComparisonTypes.AnyMatch:
                    return CompareMultiLineTextAny(a1s, a2s);
                case Data.CRM.Model.Question.ComparisonTypes.CompleteMatch:
                default:
                    return CompareMultiLineTextComplete(a1s, a2s);
            }
        }

        private bool CompareMultiLineTextAny(StringCollection a1s, StringCollection a2s)
        {
            if (a1s == null || a2s == null) { return false; }
            foreach (String a1 in a1s)
                if (a2s.Contains(a1))
                    return true;

            foreach (String a2 in a2s)
                if (a1s.Contains(a2))
                    return true;
            return false;
        }

        private bool CompareMultiLineTextComplete(StringCollection a1s, StringCollection a2s)
        {
            if (a1s == null || a2s == null) { return false; }
            if (a1s.Count != a2s.Count) { return false; }
            foreach (String a1 in a1s)
                if (!a2s.Contains(a1))
                    return false;

            foreach (String a2 in a2s)
                if (!a1s.Contains(a2))
                    return false;
            return true;
        }

        private bool CompareDate(Model.IAccountQuestion aq1, Model.IAccountQuestion aq2)
        {
            const String errorMsg = "Unsupported comparison between two different answer types.";

            // Alternate answer is for the date range
            try
            {
                switch (aq1.AnswerType)
                {
                    case Data.Enums.AnswerTypes.MultiValue:
                        return CompareMultiLineText(aq1.Answer, aq2.Answer);
                    case Data.Enums.AnswerTypes.SingleValue:
                        switch (aq2.AnswerType)
                        {
                            case Data.Enums.AnswerTypes.Range:
                                return CompareSingleRangeDate(aq1.Answer, aq2.Answer, aq2.AlternateAnswer);
                            case Data.Enums.AnswerTypes.SingleValue:
                                return CompareSingleDate(aq1.Answer, aq2.Answer);
                            default:
                                throw new InvalidOperationException(errorMsg);
                        }
                    case Data.Enums.AnswerTypes.Range:
                        switch (aq2.AnswerType)
                        {
                            case Data.Enums.AnswerTypes.Range:
                                return CompareRangeDate(aq1.Answer, aq1.AlternateAnswer, aq2.Answer, aq2.AlternateAnswer);
                            case Data.Enums.AnswerTypes.SingleValue:
                                return CompareSingleRangeDate(aq2.Answer, aq1.Answer, aq1.AlternateAnswer);
                            default:
                                throw new InvalidOperationException(errorMsg);
                        }
                    default:
                        throw new InvalidOperationException(errorMsg);
                }
            }
            catch (InvalidOperationException ex)
            {
                ex.Data.Add("aq1.Answer", aq1.Answer); ex.Data.Add("aq1.AlternateAnswer", aq1.AlternateAnswer);
                ex.Data.Add("aq2.Answer", aq2.Answer); ex.Data.Add("aq2.AlternateAnswer", aq2.AlternateAnswer);
                ex.Data.Add("aq1.AnswerType", aq1.AnswerType.ToString()); ex.Data.Add("aq2.AnswerType", aq2.AnswerType.ToString());
                throw;
            }
        }

        private bool CompareSingleRangeDate(String a1, String a2a, String a2b)
        {
            DateTime d1, d2a, d2b;

            try
            {
                d1 = DateTime.Parse(a1);
                d2a = DateTime.Parse(a2a); d2b = DateTime.Parse(a2b);
            }
            catch (InvalidCastException ex)
            {
                ex.Data.Add("a1", a1);
                ex.Data.Add("a2a", a2a); ex.Data.Add("a2b", a2b);
                throw;
            }

            Range<DateTime> range = new Range<DateTime>(d2a, d2b);
            Int32 compareResult;
            try
            {
                compareResult = range.CompareTo(d1);
            }
            catch (InvalidCompareResultException ex)
            {
                ex.Data.Add("a1", a1);
                ex.Data.Add("a2a", a2a); ex.Data.Add("a2b", a2b);
                throw;
            }
            return ProcessComparisonResult(compareResult);
        }

        private bool CompareSingleDate(String a1, String a2)
        {
            DateTime dt1, dt2;
            if (!DateTime.TryParse(a1.Trim(), out dt1)) { return false; }
            if (!DateTime.TryParse(a2.Trim(), out dt2)) { return false; }

            switch (ComparisonType)
            {
                case Data.CRM.Model.Question.ComparisonTypes.GreaterThanorEqualTo:
                    return (dt1 >= dt2);
                case Data.CRM.Model.Question.ComparisonTypes.LessThanOrEqualTo:
                    return (dt1 <= dt2);
                case Data.CRM.Model.Question.ComparisonTypes.CompleteMatch:
                case Data.CRM.Model.Question.ComparisonTypes.EqualTo:
                    return (dt1 == dt2);
                default:
                    InvalidOperationException ex = new InvalidOperationException("Unsupported comparison type.");
                    ex.Data.Add("a1", a1);
                    ex.Data.Add("a2", a2);
                    ex.Data.Add("ComparisonType", ComparisonType.ToString());
                    throw ex;
            }
        }

        private bool CompareRangeDate(String a1a, String a1b, String a2a, String a2b)
        {
            DateTime d1a, d1b, d2a, d2b;

            try
            {
                d1a = DateTime.Parse(a1a); d1b = DateTime.Parse(a1b);
                d2a = DateTime.Parse(a2a); d2b = DateTime.Parse(a2b);
            }
            catch (InvalidCastException ex)
            {
                ex.Data.Add("a1a", a1a); ex.Data.Add("a1b", a1b);
                ex.Data.Add("a2a", a2a); ex.Data.Add("a2b", a2b);
                throw;
            }

            Range<DateTime> range1 = new Range<DateTime>(d1a, d1b);
            Range<DateTime> range2 = new Range<DateTime>(d2a, d2b);
            Int32 compareResult;

            try
            {
                compareResult = range1.CompareTo(range2);
            }
            catch (InvalidCompareResultException ex)
            {
                ex.Data.Add("a1a", a1a); ex.Data.Add("a1b", a1b);
                ex.Data.Add("a2a", a2a); ex.Data.Add("a2b", a2b);
                throw;
            }

            return ProcessComparisonResult(compareResult);
        }

        private bool CompareBoolean(Model.IAccountQuestion aq1, Model.IAccountQuestion aq2)
        {
            if (aq1.Answer.Trim().ToLower() == aq2.Answer.Trim().ToLower())
                return true;
            return false;
        }

        /// <summary>
        /// Compare two Numeric Answers
        /// </summary>
        /// <param name="aq1">Client Question</param>
        /// <param name="aq2">Vendor Question</param>
        /// <returns>Whether or not the answers matched</returns>
        private bool CompareNumeric(Model.IAccountQuestion aq1, Model.IAccountQuestion aq2)
        {
            // returns true if a match is found, else false
            String regexPattern = "[^0-9.\n]";
            String answer1 = System.Text.RegularExpressions.Regex.Replace(aq1.Answer, regexPattern, String.Empty);
            String answer2 = System.Text.RegularExpressions.Regex.Replace(aq2.Answer, regexPattern, String.Empty);
            const String errorMsg = "Unsupported comparison between two different answer types.";

            try
            {
                switch (aq1.AnswerType)
                {
                    case Data.Enums.AnswerTypes.MultiValue:
                        return CompareMultiLineText(answer1, answer2);
                    case Data.Enums.AnswerTypes.SingleValue:
                        switch (aq2.AnswerType)
                        {
                            case Data.Enums.AnswerTypes.Range:
                                String altAnswer2 = System.Text.RegularExpressions.Regex.Replace(aq2.AlternateAnswer, regexPattern, String.Empty);
                                return CompareSingleRangeNumeric(answer1, answer2, altAnswer2);
                            case Data.Enums.AnswerTypes.SingleValue:
                                return CompareSingleLineNumeric(answer1, answer2);
                            default:
                                throw new InvalidOperationException(errorMsg);
                        }
                    case Data.Enums.AnswerTypes.Range:
                        switch (aq2.AnswerType)
                        {
                            case Data.Enums.AnswerTypes.Range:
                                String altAnswer1 = System.Text.RegularExpressions.Regex.Replace(aq1.AlternateAnswer, regexPattern, String.Empty);
                                String altAnswer2 = System.Text.RegularExpressions.Regex.Replace(aq2.AlternateAnswer, regexPattern, String.Empty);
                                return CompareRangeNumeric(answer1, altAnswer1, answer2, altAnswer2);
                            case Data.Enums.AnswerTypes.SingleValue:
                                String altAnswer1sv = System.Text.RegularExpressions.Regex.Replace(aq1.AlternateAnswer, regexPattern, String.Empty);
                                return CompareSingleRangeNumeric(answer2, answer1, altAnswer1sv);
                            default:
                                throw new InvalidOperationException(errorMsg);
                        }
                    default:
                        throw new InvalidOperationException(errorMsg);
                }
            }
            catch (InvalidOperationException ex)
            {
                ex.Data.Add("aq1.Answer", aq1.Answer); ex.Data.Add("aq1.AlternateAnswer", aq1.AlternateAnswer);
                ex.Data.Add("aq2.Answer", aq2.Answer); ex.Data.Add("aq2.AlternateAnswer", aq2.AlternateAnswer);
                ex.Data.Add("aq1.AnswerType", aq1.AnswerType.ToString()); ex.Data.Add("aq2.AnswerType", aq2.AnswerType.ToString());
                throw;
            }

        }

        private bool CompareSingleRangeNumeric(String a1, String a2a, String a2b)
        {
            Decimal d1, d2a, d2b;

            try
            {
                if (!Decimal.TryParse(a1, out d1))
                    return false;
                //d1 = Convert.ToDecimal(a1);

                d2a = Convert.ToDecimal(a2a);
                d2b = Convert.ToDecimal(a2b);
            }
            catch (InvalidCastException ex)
            {
                ex.Data.Add("a1", a1);
                ex.Data.Add("a2a", a2a); ex.Data.Add("a2b", a2b);
                throw;
            }

            Range<Decimal> range = new Range<Decimal>(d2a, d2b);
            Int32 compareResult;
            try
            {
                compareResult = range.CompareTo(d1);
            }
            catch (InvalidCompareResultException ex)
            {
                ex.Data.Add("a1", a1);
                ex.Data.Add("a2a", a2a); ex.Data.Add("a2b", a2b);
                throw;
            }
            return ProcessComparisonResult(compareResult);
        }

        private bool CompareRangeNumeric(String a1a, String a1b, String a2a, String a2b)
        {
            Decimal d1a, d1b, d2a, d2b;

            try
            {
                d1a = Convert.ToDecimal(a1a); d1b = Convert.ToDecimal(a1b);
                d2a = Convert.ToDecimal(a2a); d2b = Convert.ToDecimal(a2b);
            }
            catch (InvalidCastException ex)
            {
                ex.Data.Add("a1a", a1a); ex.Data.Add("a1b", a1b);
                ex.Data.Add("a2a", a2a); ex.Data.Add("a2b", a2b);
                throw;
            }

            Range<Decimal> range1 = new Range<Decimal>(d1a, d1b);
            Range<Decimal> range2 = new Range<Decimal>(d2a, d2b);
            Int32 compareResult;

            try
            {
                compareResult = range1.CompareTo(range2);
            }
            catch (InvalidCompareResultException ex)
            {
                ex.Data.Add("a1a", a1a); ex.Data.Add("a1b", a1b);
                ex.Data.Add("a2a", a2a); ex.Data.Add("a2b", a2b);
                throw;
            }

            return ProcessComparisonResult(compareResult);
        }

        private bool ProcessComparisonResult(Int32 comparisonResult)
        {
            switch (ComparisonType)
            {
                case Question.ComparisonTypes.GreaterThanorEqualTo:
                    return comparisonResult >= 0;
                case Question.ComparisonTypes.LessThanOrEqualTo:
                    return comparisonResult <= 0;
                case Question.ComparisonTypes.EqualTo:
                case Question.ComparisonTypes.CompleteMatch:
                case Question.ComparisonTypes.WithinRange:
                    return comparisonResult == 0;
                default:
                    InvalidOperationException ex = new InvalidOperationException();
                    ex.Data.Add("comparisonType", ComparisonType.ToString());
                    ex.Data.Add("comparisonResult", comparisonResult);
                    throw ex;
            }
        }

        private bool CompareSingleLineNumeric(String a1, String a2)
        {
            Decimal d1, d2;
            if (!Decimal.TryParse(a1.Trim(), out d1)) { return false; }
            if (!Decimal.TryParse(a2.Trim(), out d2)) { return false; }

            switch (ComparisonType)
            {
                case Data.CRM.Model.Question.ComparisonTypes.GreaterThanorEqualTo:
                    return (d1 >= d2);
                case Data.CRM.Model.Question.ComparisonTypes.LessThanOrEqualTo:
                    return (d1 <= d2);
                case Data.CRM.Model.Question.ComparisonTypes.CompleteMatch:
                case Data.CRM.Model.Question.ComparisonTypes.EqualTo:
                    return (d1 == d2);
                default:
                    InvalidOperationException ex = new InvalidOperationException();
                    ex.Data.Add("a1", a1); ex.Data.Add("a2", a2);
                    ex.Data.Add("ComparisonType", ComparisonType.ToString());
                    throw ex;
            }
        }

        private bool CompareText(Model.IAccountQuestion aq1, Model.IAccountQuestion aq2)
        {
            // returns true if a match is found, else false
            if (aq1.AnswerType == Data.Enums.AnswerTypes.MultiValue && aq2.AnswerType == Data.Enums.AnswerTypes.MultiValue)
                return CompareMultiLineText(aq1.Answer, aq2.Answer);
            else if (aq1.AnswerType == Data.Enums.AnswerTypes.MultiValue && aq2.AnswerType == Data.Enums.AnswerTypes.SingleValue)
                return CompareSingleToMultiLineText(aq2.Answer, aq1.Answer);
            else if (aq1.AnswerType == Data.Enums.AnswerTypes.SingleValue && aq2.AnswerType == Data.Enums.AnswerTypes.MultiValue)
                return CompareSingleToMultiLineText(aq1.Answer, aq2.Answer);

            return CompareSingleLineText(aq1.Answer, aq2.Answer);
        }

        private bool CompareSingleToMultiLineText(String a1, String a2)
        {
            System.Collections.Specialized.StringCollection a2s = new System.Collections.Specialized.StringCollection();
            a2s.AddRange(this.SplitMultiLineText(a2));

            switch (ComparisonType)
            {
                case Question.ComparisonTypes.AnyMatch:
                    foreach (String a in a2s)
                    {
                        if (a1.ToLower().Contains(a.ToLower()) || a.ToLower().Contains(a1.ToLower()))
                            return true;
                    }
                    break;
                case Question.ComparisonTypes.CompleteMatch:
                case Question.ComparisonTypes.EqualTo:
                    foreach (String a in a2s)
                    {
                        if (a1.ToLower() == a.ToLower())
                            return true;
                    }
                    break;
                default:
                    InvalidOperationException ex = new InvalidOperationException();
                    ex.Data.Add("a1", a1); ex.Data.Add("a2", a2);
                    ex.Data.Add("ComparisonType", ComparisonType.ToString());
                    throw ex;
            }
            return false;
        }

        private bool CompareSingleLineText(String a1, String a2)
        {
            switch (ComparisonType)
            {
                case Data.CRM.Model.Question.ComparisonTypes.AnyMatch:
                    return (a1.ToLower().Contains(a2.ToLower()) || a2.ToLower().Contains(a1.ToLower()));
                case Data.CRM.Model.Question.ComparisonTypes.EqualTo:
                case Data.CRM.Model.Question.ComparisonTypes.CompleteMatch:
                    return (a1.Trim().ToLower().CompareTo(a2.Trim().ToLower()) == 0);
                default:
                    InvalidOperationException ex = new InvalidOperationException();
                    ex.Data.Add("a1", a1); ex.Data.Add("a2", a2);
                    ex.Data.Add("ComparisonType", ComparisonType.ToString());
                    throw ex;
            }
        }

        private String[] SplitMultiLineText(String text)
        {
            String formattedText = text.Trim().ToLower();
            return formattedText.Split(new Char[] { Convert.ToChar("\n") }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
