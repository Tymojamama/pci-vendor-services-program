using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PCI.VSP.Test
{
    [TestClass]
    public class QuestionTest
    {
        private static Guid _productId = Guid.Parse("7d56f627-b7c6-e111-a526-000423c7d319");

        [TestMethod]
        public void RetrieveTemplatesByProductId()
        {
            Globals.InitVspService();
            PCI.VSP.Data.CRM.DataLogic.TemplateDataLogic tdl = new Data.CRM.DataLogic.TemplateDataLogic(Globals.GetGenericAuthRequest());
            List<PCI.VSP.Data.CRM.Model.Template> templates = tdl.RetrieveTemplatesByProductId(_productId);
        }

        [TestMethod]
        public void GetQuestionCategories()
        {
            Globals.InitVspService();
            PCI.VSP.Data.CRM.DataLogic.QuestionCategoryDataLogic qcdl = new Data.CRM.DataLogic.QuestionCategoryDataLogic(Globals.GetGenericAuthRequest());
            List<Data.CRM.Model.QuestionCategory> qcl = qcdl.RetrieveMultiple();
        }

        //[TestMethod]
        //public void GetNonNullComparisonTypes()
        //{
        //    Globals.InitVspService();
        //    PCI.VSP.Data.CRM.DataLogic.QuestionDataLogic qdl = new Data.CRM.DataLogic.QuestionDataLogic(Globals.GetGenericAuthRequest());
        //    List<Data.CRM.Model.Question> ql = qdl.GetNonNullComparisonTypes();
        //}
    }
}
