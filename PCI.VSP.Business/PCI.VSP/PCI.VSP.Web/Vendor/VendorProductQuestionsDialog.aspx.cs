using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text.RegularExpressions;
using CRMModel = PCI.VSP.Data.CRM.Model;
using ServiceModel = PCI.VSP.Services.Model;
using PCI.VSP.Services;
using PCI.VSP.Data.Enums;
using PCI.VSP.Data.CRM.DataLogic;

namespace PCI.VSP.Web.Vendor
{
    public partial class VendorProductQuestionsDialog : System.Web.UI.Page
    {
        #region Private Properties

        /// <summary>
        /// CRM User Credentials so I don't have to keep hardcoding it everywhere
        /// </summary>
        private AuthenticationRequest _authRequest = new AuthenticationRequest() { Username = Data.Globals.CrmServiceSettings.Username, Password = Data.Globals.CrmServiceSettings.Password };

        /// <summary>
        /// Dictionary of Categories, stored in session
        /// </summary>
        private Dictionary<Guid, Data.CRM.Model.QuestionCategory> _questionCategories
        {
            get
            {
                if (Session[Constants.QuestionCategories] == null)
                {
                    Dictionary<Guid, Data.CRM.Model.QuestionCategory> qcd = new QuestionCategoryDataLogic(new AuthenticationRequest() { Username = Data.Globals.CrmServiceSettings.Username, Password = Data.Globals.CrmServiceSettings.Password }).RetrieveAllIntoDictionary();
                    Session[Constants.QuestionCategories] = qcd;
                }
                return Session[Constants.QuestionCategories] as Dictionary<Guid, Data.CRM.Model.QuestionCategory>;
            }
        }

        /// <summary>
        /// Dictionary of Functions, stored in session
        /// </summary>
        private Dictionary<Guid, Data.CRM.Model.QuestionFunction> _questionFunctions
        {
            get
            {
                if (Session[Constants.QuestionFunctions] == null)
                {
                    Dictionary<Guid, Data.CRM.Model.QuestionFunction> qfd = new QuestionFunctionDataLogic(_authRequest).RetrieveAllIntoDictionary();
                    Session[Constants.QuestionFunctions] = qfd;
                }
                return Session[Constants.QuestionFunctions] as Dictionary<Guid, Data.CRM.Model.QuestionFunction>;
            }
        }

        /// <summary>
        /// List of IAccountQuestion (Vendor Questions), stored in ViewState
        /// </summary>
        private List<ServiceModel.IAccountQuestion> _questions
        {
            get
            {
                if (ViewState[Constants.VendorProductQuestions] == null)
                {
                    ServiceModel.IUser user = new Security.Utility().GetUser();
                    var vql = new VendorQuestionDataLogic(_authRequest).RetrieveVendorProductQuestions(user.AccountId, _vendorProductId, new VendorProductDataLogic(_authRequest).Retrieve(_vendorProductId).ProductId, false).Select<CRMModel.VendorQuestion, ServiceModel.IAccountQuestion>(vq => new ServiceModel.VendorQuestion(vq));
                    List<ServiceModel.IAccountQuestion> liaq = new List<ServiceModel.IAccountQuestion>();
                    liaq.AddRange(vql);

                    foreach (var iaq in liaq)
                        iaq.Notes = new AnnotationDataLogic(_authRequest).RetrieveNotes(iaq.Id, "vsp_vendorquestion", new string[] { "annotationid", "filename", "createdon" });

                    ViewState[Constants.VendorProductQuestions] = liaq;
                }
                return ViewState[Constants.VendorProductQuestions] as List<ServiceModel.IAccountQuestion>;
            }
        }

        /// <summary>
        /// Vendor Product ID
        /// </summary>
        private Guid _vendorProductId
        {
            get { return Guid.Parse(Request.QueryString["VendorProductId"]); }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                InitialLoad();
            else
            {
                string ctrlname = Page.Request.Params.Get("__EVENTTARGET");
                if (ctrlname == null || ctrlname == string.Empty)
                    ReloadData(hfCategoryId.Value, hfFunctionId.Value);
            }
        }

        public void category_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as LinkButton;
                if (button != null)
                {
                    var qci = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ServiceModel.QuestionCategorizationItem>(button.CommandArgument);
                    hfCategoryId.Value = qci.CategoryId.ToString();
                    hfFunctionId.Value = qci.FunctionId.ToString();
                    ReloadData(qci.CategoryId.ToString(), qci.FunctionId.ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initial Load - Session, View State and Category Repeater
        /// </summary>
        private void InitialLoad()
        {
            ServiceModel.IUser user = new Security.Utility().GetUser();
            List<ServiceModel.QuestionCategorizationItem> qcil = new List<ServiceModel.QuestionCategorizationItem>();

            #region Add Question Categories

            var Categories = _questions.Where(a => a.QuestionType == QuestionTypes.SearchQuestion_Filter1).Select(a => a.CategoryId).Distinct();
            foreach (var item in Categories)
            {
                var qc = _questionCategories.Where(a => a.Key == item).Select(a => a.Value).FirstOrDefault();
                if (qc != null)
                {
                    var qci = new ServiceModel.QuestionCategorizationItem();
                    qci.CategoryId = qc.Id;
                    qci.DisplayName = qci.CategoryName = qc.Name;
                    qci.CategorySortOrder = qc.SortOrder;
                    qci.Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion;
                    qci.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(qci);
                    qcil.Add(qci);
                }
                else // Uncategorized
                {
                    var qci = new ServiceModel.QuestionCategorizationItem();
                    qci.CategoryId = Guid.Empty;
                    qci.DisplayName = qci.CategoryName = "Uncategorized";
                    qci.CategorySortOrder = int.MaxValue;
                    qci.Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion;
                    qci.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(qci);
                    qcil.Add(qci);
                }
            }

            #endregion

            #region Add Question Functions

            var Functions = _questions.Select(a => new { a.CategoryId, a.FunctionId }).Distinct();
            foreach (var f in Functions)
            {
                var qc = _questionCategories.Where(a => a.Key == f.CategoryId).Select(a => a.Value).FirstOrDefault();
                var qf = _questionFunctions.Where(a => a.Key == f.FunctionId).Select(a => a.Value).FirstOrDefault();

                if (qf != null && qc != null)
                {
                    var qci = new ServiceModel.QuestionCategorizationItem()
                    {
                        CategoryId = qc.Id,
                        CategoryName = qc.Name,
                        CategorySortOrder = qc.SortOrder,
                        FunctionId = qf.Id,
                        DisplayName = qf.Name,
                        FunctionName = qf.Name,
                        FunctionSortOrder = qf.SortOrder,
                        SubItemLevel = 1,
                        Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion
                    };
                    qci.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(qci);

                    qcil.Add(qci);
                }
            }

            #endregion

            var qcf = qcil.OrderBy(a => a.CategorySortOrder).ThenBy(a => a.FunctionSortOrder).ToList();
            rptCategories.DataSource = qcf;
            rptCategories.DataBind();
        }
        
        /// <summary>
        /// Re-databind QA Control
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <param name="functionId">Function ID</param>
        private void ReloadData(string categoryId, string functionId)
        {
            List<PCI.VSP.Services.Model.IAccountQuestion> aql = new List<ServiceModel.IAccountQuestion>();
            Guid c, f;
            Guid.TryParse(categoryId, out c);
            Guid.TryParse(functionId, out f);
            
            if (f != Guid.Empty)
                PopulateQAControl(_questions.Where(a => a.CategoryId == c && a.FunctionId == f).ToList());
            else
                PopulateQAControl(_questions.Where(a => a.CategoryId == c).ToList());
        }

        /// <summary>
        /// Populate the QA User Control with Vendor Products Questions
        /// </summary>
        /// <param name="aql">List of IAccountQuestion (Vendor Question)</param>
        private void PopulateQAControl(List<PCI.VSP.Services.Model.IAccountQuestion> aql)
        {
            try
            {
                QAUserControl1.Visible = true;
                QAUserControl1.SaveRequest += UpdateVendorProductQuestions;
                QAUserControl1.UserType = Enums.UserType.Vendor;
                QAUserControl1.Closeable = true;
                QAUserControl1.Questions = aql;
                QAUserControl1.AllowedFileTypes = new DocumentTypeDataLogic(_authRequest).RetrieveAllowedDocumentTypes();
                QAUserControl1.DataBind();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Save Vendor Product Questions
        /// </summary>
        /// <param name="qac">QA User Control</param>
        /// <param name="e">Save Request Event Args</param>
        private void UpdateVendorProductQuestions(Controls.QAUserControl qac, Controls.QAUserControl.SaveRequestEventArgs e)
        {
            //Services.VspService service = new Services.VspService();
            new VspService().UpdateVendorQuestions(e.AccountQuestions, new Security.Utility().GetUser().Id, false);
            //InitializeQA_Control();
        }

        #endregion
    }
}
