using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
namespace PCI.VSP.Services.Model
{
    public interface IAccountQuestion
    {
        // QuestionId
        Guid QuestionId { get; set; }
        Guid CategoryId { get; set; }
        Guid FunctionId { get; set; }
        Guid Id { get; set; } // (VendorQuestionId or ClientQuestionId)
        Guid AccountId { get; set; } // (VendorId or ClientId)
        Guid TempId { get; }
        Guid PlanAssumptionId { get; set; }

        String Name { get; set; }
        String AccountWording { get; set; } // (VendorWording or ClientWording)
        String Answer { get; set; }
        String AlternateAnswer { get; set; }
        String ChoiceAnswers { get; set; }

        string CommentToPCI { get; set; }
        string CommentFromPCI { get; set; }

        //String PciComment { get; set; }
        //String AssetClass { get; set; }
        String AssetFund { get; set; }
        String AssetSymbol { get; set; }
        Int32 Participants { get; set; }
        Guid AssetClassId { get; set; }
        Decimal Assets { get; set; }
        Decimal AnnualContributions { get; set; }
        
        /// <summary>
        /// The minimum value the answer (single or range) the answer can be.  Only works for integer or currency/money answer types
        /// </summary>
        String MinimumAnswerAllowed { get; set; }

        /// <summary>
        /// The maximum value the answer (single or range) the answer can be.  Only works for integer or currency/money answer types
        /// </summary>
        String MaximumAnswerAllowed { get; set; }


        Data.Enums.DataTypes QuestionDataType { get; set; }
        Data.Enums.QuestionTypes QuestionType { get; set; }
        Data.Enums.AnswerTypes AnswerType { get; set; }
        String AnswerRejectedReason { get; set; }
        Data.Enums.InvalidAnswerReasons InvalidAnswerReason { get; set; }
        Data.Enums.AccountQuestionStatuses Status { get; set; }

        string NoteBase64Data { get; set; }
        string NoteFileName { get; set; }
        List<PCI.VSP.Data.CRM.Model.Annotation> Notes { get; set; }
        PCI.VSP.Data.CRM.Model.Annotation DocumentTemplate { get; set; }
        Guid DocumentTemplateId { get; set; }
        //Boolean HasInvestmentAssumptions { get; }
        // TemplateId ???
        // VendorProductId ???

    }
}
