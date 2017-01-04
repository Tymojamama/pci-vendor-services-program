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
        // This method may no longer be used.
        private Dictionary<Guid, List<VendorClientQuestion>> __RunFilterComparison(List<VendorProduct> _VendorProducts, List<ClientQuestion> _ClientQuestions, FilterPhases _filterPhase)
        {
            Trace.TraceInformation("Entering " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            // convert to dictionary
            Dictionary<Guid, ClientQuestion> _dctClientQuestion = _ClientQuestions.ToDictionary(cq => cq.Id);
            _ClientQuestions = null;

            // get lists of vendorProducts
            //List<VendorProduct> vpl = new VendorProductDataLogic(GetDefaultAuthRequest()).RetrieveMultipleByClientProject(clientProjectId);


            // convert to dictionary
            Dictionary<Guid, VendorProduct> _dctVendorProduct = _VendorProducts.ToDictionary(vp => vp.VendorProductId);
            _VendorProducts = null;

            Dictionary<Guid, Dictionary<Guid, VendorQuestion>> vqd = new Dictionary<Guid, Dictionary<Guid, VendorQuestion>>();

            // get lists of vendorQuestions
            // retrieve vendorQuestions by vendorProduct
            VendorQuestionDataLogic vqdl = new VendorQuestionDataLogic(_getDefaultAuthRequest());
            List<VendorQuestion> tvql = new List<VendorQuestion>();
            foreach (VendorProduct vp in _dctVendorProduct.Values)
            {
                List<VendorQuestion> vql = vqdl.RetrieveVendorProductQuestionsForFilter(vp.VendorId, vp.VendorProductId);
                if (_filterPhase == FilterPhases.Phase1) // include vendor profile questions
                {
                    List<VendorQuestion> _lstVendorProductQuestion = vqdl.RetrieveVendorProfileQuestionsNoCreate(vp.VendorId);
                    if (_lstVendorProductQuestion != null && _lstVendorProductQuestion.Count > 0)
                        vql.AddRange(_lstVendorProductQuestion);
                }
                if (vql == null || vql.Count == 0) { continue; }
                vqd.Add(vp.VendorProductId, vql.ToDictionary(vq => vq.Id));
                tvql.AddRange(vql);
            }
            _VendorProducts = null;

            // get intersect, then comparison types
            IEnumerable<Guid> xect = from VendorQuestion vq in tvql
                                     join ClientQuestion cq in _dctClientQuestion.Values on vq.QuestionId equals cq.QuestionId
                                     select vq.QuestionId;

            QuestionDataLogic qdl = new QuestionDataLogic(_getDefaultAuthRequest());
            List<Question> ql = qdl.RetrieveComparisonTypes(xect);
            tvql = null;
            xect = null;

            if (ql == null) { ql = new List<Question>(); }

            Dictionary<Guid, Question> qd = ql.ToDictionary(q => q.QuestionId);
            ql = null;

            Dictionary<Guid, List<VendorClientQuestion>> vcqd = new Dictionary<Guid, List<VendorClientQuestion>>();

            // match up client questions with vendor questions for each product
            foreach (KeyValuePair<Guid, Dictionary<Guid, VendorQuestion>> tvqd in vqd)
            {
                IEnumerable<VendorClientQuestion> results = from VendorQuestion vq in tvqd.Value.Values
                                                            join ClientQuestion cq in _dctClientQuestion.Values on vq.QuestionId equals cq.QuestionId
                                                            select new VendorClientQuestion(vq, cq);

                if (results == null) { continue; }
                List<VendorClientQuestion> vcql = results.ToList();
                results = null;

                foreach (VendorClientQuestion vcq in vcql)
                {
                    try
                    {
                        vcq.ComparisonType = qd[vcq.VendorQuestion.QuestionId].ComparisonType;

                        if (qd.ContainsKey(vcq.VendorQuestion.QuestionId))
                        {
                            if (vcq.ClientQuestion.QuestionType == Data.Enums.QuestionTypes.InvestmentAssumption && _filterPhase == FilterPhases.Phase2)
                            {
                                // logic to match vendor & client investment assumptions
                                vcq.AreMatch = vcq.VendorQuestion.InvestmentAssumptionsConfirmed;
                            }
                            else
                            {
                                Comparison _comparison = new Comparison(this, vcq.ComparisonType);
                                vcq.AreMatch = _comparison.Compare(new Model.ClientQuestion(vcq.ClientQuestion), new Model.VendorQuestion(vcq.VendorQuestion));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        vcq.CompareException = ex;
                    }
                }

                if (vcql.Count > 0)
                    vcqd.Add(tvqd.Key, vcql);
            }
            Trace.TraceInformation("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            return vcqd;
        }
    }
}
