using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PCI.VSP.Data.Enums;
using PCI.VSP.Data.CRM.Model;

namespace PCI.VSP.Web.CrmIFrames.Model
{
    internal class FilterCriteria
    {
        internal AnswerTypes ClientResponseType { get; set; }
        internal AnswerTypes VendorResponseType { get; set; }
        internal Question.ComparisonTypes ComparisonType { get; set; }
        internal DataTypes QuestionDataType { get; set; }
    }
}