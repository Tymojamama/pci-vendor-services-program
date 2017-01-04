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
        private void CompareVendorQuestionsToClientQuestions(List<VendorClientQuestion> vendorClientQuestions)
        {
            foreach (VendorClientQuestion vendorClientQuestion in vendorClientQuestions)
            {
                try
                {
                    if (vendorClientQuestion.DataType == Data.Enums.DataTypes.InvestmentAssumption)
                    {
                        vendorClientQuestion.RequiredMatch = vendorClientQuestion.ClientQuestion.VendorMustOfferFund;
                        vendorClientQuestion.AreMatch = vendorClientQuestion.VendorQuestion.InvestmentAssumptionsConfirmed;
                    }
                    else
                    {
                        Model.ClientQuestion clientQuestion = new Model.ClientQuestion(vendorClientQuestion.ClientQuestion);
                        Model.VendorQuestion vendorQuestion = new Model.VendorQuestion(vendorClientQuestion.VendorQuestion);

                        Comparison comparison = new Comparison(this, vendorClientQuestion.ComparisonType);
                        vendorClientQuestion.AreMatch = comparison.Compare(clientQuestion, vendorQuestion);
                    }
                }
                catch (Exception ex)
                {
                    vendorClientQuestion.CompareException = ex;
                }
            }
        }
    }
}
