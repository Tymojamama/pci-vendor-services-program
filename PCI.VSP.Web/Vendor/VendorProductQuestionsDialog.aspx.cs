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

                    VendorQuestionDataLogic vendorQuestionDataLogic = new VendorQuestionDataLogic(_authRequest);
                    List<CRMModel.VendorQuestion> vendorQuestions = vendorQuestionDataLogic.RetrieveVendorProductQuestions(user.AccountId, _vendorProductId, new VendorProductDataLogic(_authRequest).Retrieve(_vendorProductId).ProductId, false);
                    IEnumerable<ServiceModel.IAccountQuestion> accountQuestions = vendorQuestions.Select<CRMModel.VendorQuestion, ServiceModel.IAccountQuestion>(vq => new ServiceModel.VendorQuestion(vq));
                    
                    List<ServiceModel.IAccountQuestion> accountQuestionsList = new List<ServiceModel.IAccountQuestion>();
                    accountQuestionsList.AddRange(accountQuestions);

                    foreach (ServiceModel.IAccountQuestion accountQuestion in accountQuestionsList)
                    {
                        accountQuestion.Notes = new AnnotationDataLogic(_authRequest).RetrieveNotes(accountQuestion.Id, "vsp_vendorquestion", new string[] { "annotationid", "filename", "createdon" });
                    }

                    ViewState[Constants.VendorProductQuestions] = accountQuestionsList;
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

        /// <summary>
        /// Initial Load - Session, View State and Category Repeater
        /// </summary>
        private void InitialLoad()
        {
            ServiceModel.IUser user = new Security.Utility().GetUser();
            List<ServiceModel.QuestionCategorizationItem> lstQuestionCategorizationItem = new List<ServiceModel.QuestionCategorizationItem>();

            AddQuestionCategories(lstQuestionCategorizationItem);
            AddQuestionFunctions(lstQuestionCategorizationItem);

            rptCategories.DataSource = lstQuestionCategorizationItem.OrderBy(a => a.CategorySortOrder).ThenBy(a => a.FunctionSortOrder).ToList();
            rptCategories.DataBind();
        }

        private void AddQuestionFunctions(List<ServiceModel.QuestionCategorizationItem> lstQuestionCategorizationItem)
        {
            var functions = _questions.Select(a => new { a.CategoryId, a.FunctionId }).Distinct();
            foreach (var function in functions)
            {
                CRMModel.QuestionCategory questionCategory = _questionCategories.Where(a => a.Key == function.CategoryId).Select(a => a.Value).FirstOrDefault();
                CRMModel.QuestionFunction questionFunction = _questionFunctions.Where(a => a.Key == function.FunctionId).Select(a => a.Value).FirstOrDefault();

                if (questionFunction != null && questionCategory != null)
                {
                    ServiceModel.QuestionCategorizationItem questionCategorizationItem = new ServiceModel.QuestionCategorizationItem()
                    {
                        CategoryId = questionCategory.Id,
                        CategoryName = questionCategory.Name,
                        CategorySortOrder = questionCategory.SortOrder,
                        FunctionId = questionFunction.Id,
                        DisplayName = questionFunction.Name,
                        FunctionName = questionFunction.Name,
                        FunctionSortOrder = questionFunction.SortOrder,
                        SubItemLevel = 1,
                        Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion
                    };

                    questionCategorizationItem.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(questionCategorizationItem);

                    lstQuestionCategorizationItem.Add(questionCategorizationItem);
                }
            }
        }

        private void AddQuestionCategories(List<ServiceModel.QuestionCategorizationItem> lstQuestionCategorizationItem)
        {
            var categories = _questions.Where(a => a.QuestionType == QuestionTypes.SearchQuestion_Filter1).Select(a => a.CategoryId).Distinct();
            foreach (var category in categories)
            {
                CRMModel.QuestionCategory questionCategory = _questionCategories.Where(a => a.Key == category).Select(a => a.Value).FirstOrDefault();

                ServiceModel.QuestionCategorizationItem questionCategorizationItem = new ServiceModel.QuestionCategorizationItem();

                if (questionCategory != null)
                {
                    questionCategorizationItem.CategoryId = questionCategory.Id;
                    questionCategorizationItem.DisplayName = questionCategorizationItem.CategoryName = questionCategory.Name;
                    questionCategorizationItem.CategorySortOrder = questionCategory.SortOrder;
                    questionCategorizationItem.Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion;
                    questionCategorizationItem.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(questionCategorizationItem);
                }
                else // Uncategorized
                {
                    questionCategorizationItem.CategoryId = Guid.Empty;
                    questionCategorizationItem.DisplayName = questionCategorizationItem.CategoryName = "Uncategorized";
                    questionCategorizationItem.CategorySortOrder = int.MaxValue;
                    questionCategorizationItem.Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion;
                    questionCategorizationItem.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(questionCategorizationItem);
                }

                lstQuestionCategorizationItem.Add(questionCategorizationItem);
            }
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
            {
                PopulateQAControl(_questions.Where(a => a.CategoryId == c && a.FunctionId == f).ToList());
            }
            else
            {
                PopulateQAControl(_questions.Where(a => a.CategoryId == c).ToList());
            }
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
    }
}
