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
        /// <summary>
        /// Compare a Dictionary of Vendor and Client Questions
        /// </summary>
        /// <param name="vendorQuestionByQuestionId">Dictionary of Vendor Questions</param>
        /// <param name="cqd">Dictionary of Client Questions</param>
        /// <param name="filterPhase">Filter Phase</param>
        /// <returns>List of Vendor Client Question</returns>
        private List<VendorClientQuestion> FilterProcess(Dictionary<Tuple<Guid, Guid>, VendorQuestion> vendorQuestionByQuestionId, List<ClientQuestion> clientQuestionsWithRank)
        {
            Trace.TraceInformation("Entering " + System.Reflection.MethodBase.GetCurrentMethod().Name);

            QuestionDataLogic questionDataLogic = new QuestionDataLogic(_getDefaultAuthRequest());
            Dictionary<Guid, Question> questionsByQuestionId = questionDataLogic.RetrieveComparisonTypes(clientQuestionsWithRank.Distinct().Select(z => z.QuestionId).AsEnumerable()).ToDictionary(a => a.QuestionId);

            List<VendorClientQuestion> vendorClientQuestions = new List<VendorClientQuestion>();
            CreateAndAddVendorClientQuestions(vendorQuestionByQuestionId, questionsByQuestionId, vendorClientQuestions);
            CompareVendorQuestionsToClientQuestions(vendorClientQuestions);

            Trace.TraceInformation("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().Name);

            return vendorClientQuestions;
        }
    }
}
