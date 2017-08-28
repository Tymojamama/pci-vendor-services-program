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
    public partial class Filter
    {
        private void CreateAndAddVendorClientQuestions(Dictionary<Tuple<Guid, Guid>, VendorQuestion> vendorQuestionByQuestionId, Dictionary<Guid, Question> questionsByQuestionId, List<VendorClientQuestion> vendorClientQuestions)
        {
            foreach (ClientQuestion clientQuestion in ClientQuestions)
            {
                Question question = questionsByQuestionId.Where(z => z.Key == clientQuestion.QuestionId).Select(z => z.Value).FirstOrDefault();
                if (question == null)
                {
                    continue;
                }

                VendorQuestion vendorQuestion;
                if (clientQuestion.QuestionType == Data.Enums.QuestionTypes.InvestmentAssumption || clientQuestion.QuestionType == Data.Enums.QuestionTypes.Fee)
                {
                    vendorQuestion = vendorQuestionByQuestionId.Where(z => z.Key.Item1 == clientQuestion.QuestionId && z.Key.Item2 == clientQuestion.Id).Select(z => z.Value).FirstOrDefault();
                }
                else
                {
                    vendorQuestion = vendorQuestionByQuestionId.Where(z => z.Key.Item1 == clientQuestion.QuestionId).Select(z => z.Value).FirstOrDefault();
                }

                VendorClientQuestion vendorClientQuestion = new VendorClientQuestion(vendorQuestion, clientQuestion)
                {
                    ComparisonType = question.ComparisonType,
                    DataType = question.QuestionDataType,
                    RequiredMatch = clientQuestion.VendorMustOfferFund
                };

                vendorClientQuestions.Add(vendorClientQuestion);
            }
        }
    }
}
