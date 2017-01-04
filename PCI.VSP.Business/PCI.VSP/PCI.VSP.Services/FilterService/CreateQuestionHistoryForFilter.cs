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

namespace PCI.VSP.Services
{
    public partial class FilterService
    {
        private VendorQuestionHistory CreateQuestionHistoryForFilter(VendorQuestion _vendorQuestion)
        {
            VendorQuestionHistory _vendorQuestionHistory = new VendorQuestionHistory()
            {
                AlternateAnswer = _vendorQuestion.AlternateAnswer,
                Answer = _vendorQuestion.Answer,
                AnswerType = _vendorQuestion.AnswerType,
                CategoryId = _vendorQuestion.CategoryId,
                ChoiceAnswers = _vendorQuestion.ChoiceAnswers,
                LastUpdated = _vendorQuestion.LastUpdated,
                Name = _vendorQuestion.Name,
                QuestionDataType = _vendorQuestion.QuestionDataType,
                QuestionId = _vendorQuestion.QuestionId,
                Status = _vendorQuestion.Status,
                TemplateId = _vendorQuestion.TemplateId,
                VendorId = _vendorQuestion.VendorId,
                VendorProductId = _vendorQuestion.VendorProductId,
                VendorQuestionId = _vendorQuestion.Id,
                VendorWording = _vendorQuestion.VendorWording,
                ClientProjectId = ClientProject.Id
            };

            switch (_filter.FilterPhase)
            {
                case Filter.FilterPhases.Phase1:
                    _vendorQuestionHistory.FilterCategory = FilterCategory.Filter1;
                    break;
                case Filter.FilterPhases.Phase2:
                    _vendorQuestionHistory.FilterCategory = FilterCategory.Filter2;
                    break;
            }

            return _vendorQuestionHistory;
        }
    }
}
