using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Services;
using PCI.VSP.Services.Model;
using PCI.VSP.Data.CRM.Model;
using PCI.VSP.Data.Enums;

namespace PCI.VSP.Web.CrmIFrames
{
    public partial class PCI_ClientQuestions : System.Web.UI.Page
    {
        #region Private Variables

        private Dictionary<Guid, Data.CRM.Model.QuestionCategory> QuestionCategories
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

        private Dictionary<Guid, Data.CRM.Model.QuestionFunction> QuestionFunctions
        {
            get
            {
                if (Session[Constants.QuestionFunctions] == null)
                {
                    Dictionary<Guid, Data.CRM.Model.QuestionFunction> qfd = new QuestionFunctionDataLogic(new AuthenticationRequest() { Username = Data.Globals.CrmServiceSettings.Username, Password = Data.Globals.CrmServiceSettings.Password }).RetrieveAllIntoDictionary();
                    Session[Constants.QuestionFunctions] = qfd;
                }
                return Session[Constants.QuestionFunctions] as Dictionary<Guid, Data.CRM.Model.QuestionFunction>;
            }
        }

        private List<InvestmentAssetClass> AssetClasses
        {
            get
            {
                return new InvestmentAssetClassDataLogic(new AuthenticationRequest() { Username = Data.Globals.CrmServiceSettings.Username, Password = Data.Globals.CrmServiceSettings.Password }).RetrieveAll();
            }
        }

        private Guid _clientProjectId = Guid.Empty;
        private Guid _categoryId = Guid.Empty;
        private Guid _functionId = Guid.Empty;
        private QuestionTypes _type = QuestionTypes.SearchQuestion_Filter1;

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Request.QueryString.AllKeys.Contains("id"))
                {
                    lblText.Text = "No Client Project ID Specified.";
#if DEBUG
                    _clientProjectId = Guid.Parse("BBAC4060-1629-E211-AF41-00155D016411");
                    if (!IsPostBack)
                        InitialLoad();
                    else
                    {
                        string ctrlname = Page.Request.Params.Get("__EVENTTARGET");
                        if (ctrlname == null || ctrlname == string.Empty)
                        {
                            reloadData();
                        }
                    }
#endif
                }
                else
                {
                    if (Guid.TryParse(Request.QueryString["id"], out _clientProjectId))
                        if (!IsPostBack) 
                            InitialLoad();
                        else
                        {
                            string ctrlname = Page.Request.Params.Get("__EVENTTARGET");
                            if (ctrlname == null || ctrlname == string.Empty)
                            {
                                reloadData();
                            }
                        }
                    else
                        lblText.Text = "Invalid Client Project ID Specified.";
                }
            }
            catch (Exception ex)
            {
                lblText.Text = "Message: " + ex.Message + " Stack Trace: " + ex.StackTrace;
            }
        }

        private void reloadData()
        {
            if (string.IsNullOrEmpty(hfType.Value)) return;

            _type = (QuestionTypes)int.Parse(hfType.Value);
            List<PCI.VSP.Services.Model.IAccountQuestion> aql = new List<IAccountQuestion>();
            List<PCI.VSP.Data.CRM.Model.ClientQuestion> cql = new List<Data.CRM.Model.ClientQuestion>();
            var cqdl = new ClientQuestionDataLogic(new AuthenticationRequest() { Username = Data.Globals.CrmServiceSettings.Username, Password = Data.Globals.CrmServiceSettings.Password });

            switch (_type)
            {
                case QuestionTypes.SearchQuestion_Filter1:
                case QuestionTypes.SearchQuestion_Filter2:
                    _categoryId = Guid.Parse(hfCategoryId.Value);
                    _functionId = Guid.Parse(hfFunctionId.Value);
                    cql = cqdl.Retrieve(_clientProjectId, _categoryId, _functionId, new int[] { Convert.ToInt32(QuestionTypes.SearchQuestion_Filter1), Convert.ToInt32(QuestionTypes.SearchQuestion_Filter2) });
                    aql = cql.Select<Data.CRM.Model.ClientQuestion, Services.Model.ClientQuestion>(cq => new Services.Model.ClientQuestion(cq)).Cast<IAccountQuestion>().ToList();
                    PopulateQAControl(aql);
                    break;
                case QuestionTypes.PlanAssumption:
                case QuestionTypes.ProjectSpecificQuestion:
                    hfType.Value = ((int)_type).ToString();    
                    cql = cqdl.Fetch(_clientProjectId, _type);                        
                    aql = cql.Select<Data.CRM.Model.ClientQuestion, Services.Model.ClientQuestion>(cq => new Services.Model.ClientQuestion(cq)).Cast<IAccountQuestion>().ToList();
                    PopulateQAControl(aql);
                    break;
                case QuestionTypes.InvestmentAssumption:
                    hfType.Value = ((int)_type).ToString();
                    cql = cqdl.RetrieveInvestmentAssumptions(_clientProjectId);
                    aql = cql.Select<Data.CRM.Model.ClientQuestion, Services.Model.ClientQuestion>(cq => new Services.Model.ClientQuestion(cq)).Cast<IAccountQuestion>().ToList();
                    PopulateInvestmentAssumpControl(aql, false);
                    break;
            }
        }

        public void category_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton button = (LinkButton)sender;
                _type = (QuestionTypes)int.Parse(button.CommandName);
                List<PCI.VSP.Services.Model.IAccountQuestion> aql = new List<IAccountQuestion>();
                List<PCI.VSP.Data.CRM.Model.ClientQuestion> cql = new List<Data.CRM.Model.ClientQuestion>();
                var cqdl = new ClientQuestionDataLogic(new AuthenticationRequest() { Username = Data.Globals.CrmServiceSettings.Username, Password = Data.Globals.CrmServiceSettings.Password });

                switch (_type)
                {
                    case QuestionTypes.SearchQuestion_Filter1:
                    case QuestionTypes.SearchQuestion_Filter2:
                        var qci = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<QuestionCategorizationItem>(button.CommandArgument);
                        hfCategoryId.Value = qci.CategoryId.ToString();
                        hfFunctionId.Value = qci.FunctionId.ToString();
                        hfType.Value = ((int)_type).ToString();
                        cql = cqdl.Retrieve(_clientProjectId, qci.CategoryId, qci.FunctionId);
                        aql = cql.Where(z => z.QuestionType == QuestionTypes.SearchQuestion_Filter1 || z.QuestionType == QuestionTypes.SearchQuestion_Filter2).Select<Data.CRM.Model.ClientQuestion, Services.Model.ClientQuestion>(cq => new Services.Model.ClientQuestion(cq)).Cast<IAccountQuestion>().ToList();
                        PopulateQAControl(aql);
                        break;
                    case QuestionTypes.PlanAssumption:
                    case QuestionTypes.ProjectSpecificQuestion:
                        hfType.Value = ((int)_type).ToString();
                        cql = cqdl.Fetch(_clientProjectId, _type);                        
                        aql = cql.Select<Data.CRM.Model.ClientQuestion, Services.Model.ClientQuestion>(cq => new Services.Model.ClientQuestion(cq)).Cast<IAccountQuestion>().ToList();
                        PopulateQAControl(aql);
                        break;
                    case QuestionTypes.InvestmentAssumption:
                        hfType.Value = ((int)_type).ToString();
                        cql = cqdl.RetrieveInvestmentAssumptions(_clientProjectId);
                        aql = cql.Select<Data.CRM.Model.ClientQuestion, Services.Model.ClientQuestion>(cq => new Services.Model.ClientQuestion(cq)).Cast<IAccountQuestion>().ToList();
                        PopulateInvestmentAssumpControl(aql, true);
                        break;
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
        /// Initial load of the page/data
        /// </summary>
        private void InitialLoad()
        {
            AuthenticationRequest ar = new AuthenticationRequest() { Username = Data.Globals.CrmServiceSettings.Username, Password = Data.Globals.CrmServiceSettings.Password };
            //var cql = new ClientQuestionDataLogic(ar).RetrieveMultipleByClientProject(_clientProjectId, true);
            var cql = new ClientQuestionDataLogic(ar).FetchByClientProject(_clientProjectId, true);

            if (cql == null) return;

            List<PCI.VSP.Services.Model.IAccountQuestion> laq = cql.Select<Data.CRM.Model.ClientQuestion, Services.Model.ClientQuestion>(cq => new Services.Model.ClientQuestion(cq)).Cast<IAccountQuestion>().ToList();
            List<QuestionCategorizationItem> qcil = new List<QuestionCategorizationItem>();

            if (laq.Where(a => a.QuestionType == Data.Enums.QuestionTypes.PlanAssumption).Count() > 0)
                qcil.Add(new QuestionCategorizationItem() { DisplayName = "Plan Assumptions", Type = QuestionCategorizationItem.CategorizationQuestionTypes.PlanAssumption, CategorySortOrder = -3 });

            if (laq.Where(a => a.QuestionType == Data.Enums.QuestionTypes.InvestmentAssumption).Count() > 0)
                qcil.Add(new QuestionCategorizationItem() { DisplayName = "Investment Assumptions", Type = QuestionCategorizationItem.CategorizationQuestionTypes.InvestmentAssumption, CategorySortOrder = -2 });

            if (laq.Where(a => a.QuestionType == Data.Enums.QuestionTypes.ProjectSpecificQuestion).Count() > 0)
                qcil.Add(new QuestionCategorizationItem() { DisplayName = "Project Specific Questions", Type = QuestionCategorizationItem.CategorizationQuestionTypes.ProjectSpecificQuestion, CategorySortOrder = -1 });

            #region Add Question Categories

            var Categories = laq.Where(a => a.QuestionType == QuestionTypes.SearchQuestion_Filter1 || a.QuestionType == QuestionTypes.SearchQuestion_Filter2).Select(a => a.CategoryId).Distinct();
            foreach (var item in Categories)
            {
                var qc = QuestionCategories.Where(a => a.Key == item).Select(a => a.Value).FirstOrDefault();
                if (qc != null)
                {
                    var qci = new QuestionCategorizationItem();
                    qci.CategoryId = qc.Id;
                    qci.DisplayName = qci.CategoryName = qc.Name;
                    qci.CategorySortOrder = qc.SortOrder;
                    qci.Type = QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion;
                    qci.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(qci);
                    qcil.Add(qci);
                }
                else // Uncategorized
                {
                    var qci = new QuestionCategorizationItem();
                    qci.CategoryId = Guid.Empty;
                    qci.DisplayName = qci.CategoryName = "Uncategorized";
                    qci.CategorySortOrder = int.MaxValue;
                    qci.Type = QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion;
                    qci.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(qci);
                    qcil.Add(qci);
                }
            }

            #endregion

            #region Add Question Functions

            var Functions = laq.Where(a => a.QuestionType == QuestionTypes.SearchQuestion_Filter1 || a.QuestionType == QuestionTypes.SearchQuestion_Filter2 || a.QuestionType == QuestionTypes.ProjectSpecificQuestion).Select(a => new { a.CategoryId, a.FunctionId }).Distinct();
            foreach (var f in Functions)
            {
                var qc = QuestionCategories.Where(a => a.Key == f.CategoryId).Select(a => a.Value).FirstOrDefault();
                var qf = QuestionFunctions.Where(a => a.Key == f.FunctionId).Select(a => a.Value).FirstOrDefault();

                if (qf != null && qc != null)
                {
                    var qci = new QuestionCategorizationItem()
                    {
                        CategoryId = qc.Id,
                        CategoryName = qc.Name,
                        CategorySortOrder = qc.SortOrder,
                        FunctionId = qf.Id,
                        DisplayName = qf.Name,
                        FunctionName = qf.Name,
                        FunctionSortOrder = qf.SortOrder,
                        SubItemLevel = 1,
                        Type = QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion
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
        /// Method to populate the QA User Control
        /// </summary>
        private void PopulateQAControl(List<PCI.VSP.Services.Model.IAccountQuestion> aql)
        {
            try
            {
                QAUserControl1.Visible = true;
                InvestmentAssumptionsControl1.Visible = false;
                QAUserControl1.SaveRequest += UpdateQuestions;
                QAUserControl1.UserType = Enums.UserType.PCI;
                QAUserControl1.Questions = aql;
                QAUserControl1.AllowedFileTypes = new DocumentTypeDataLogic(new AuthenticationRequest() { Username = Data.Globals.CrmServiceSettings.Username, Password = Data.Globals.CrmServiceSettings.Password }).RetrieveAllowedDocumentTypes();
                QAUserControl1.DataBind();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Method to populate the Investment Assumption User Control
        /// </summary>
        /// <param name="aql"></param>
        private void PopulateInvestmentAssumpControl(List<PCI.VSP.Services.Model.IAccountQuestion> aql, bool bindData)
        {
            QAUserControl1.Visible = false;
            InvestmentAssumptionsControl1.Visible = true;
            InvestmentAssumptionsControl1.SaveRequest += UpdateInvestmentAssumptions;
            InvestmentAssumptionsControl1.Audience = Enums.UserType.Client;
            InvestmentAssumptionsControl1.AssetClasses = AssetClasses;
            InvestmentAssumptionsControl1.InvestmentAssumptions = aql;
            if (bindData) InvestmentAssumptionsControl1.DataBind();
        }

        /// <summary>
        /// Method to save the client questions
        /// </summary>
        /// <param name="qac">QA User Control</param>
        /// <param name="e">QA User Control event arguments</param>
        private void UpdateQuestions(Controls.QAUserControl qac, Controls.QAUserControl.SaveRequestEventArgs e)
        {
            try
            {
                new Services.VspService().UpdateClientQuestions(e.AccountQuestions);
            }
            catch (Exception ex)
            {
                lblText.Text = "Message: " + ex.Message + " Stack Trace: " + ex.StackTrace;
            }
        }

        /// <summary>
        /// Method to save Client Investment Assumptions
        /// </summary>
        /// <param name="iac">Investment Assumptions Control</param>
        /// <param name="e">Save Request Event Args</param>
        private void UpdateInvestmentAssumptions(Controls.InvestmentAssumptionsControl iac, Controls.InvestmentAssumptionsControl.SaveRequestEventArgs e)
        {
            try
            {
                new Services.VspService().UpdateClientQuestions(e.AccountQuestions);
            }
            catch (Exception ex)
            {
                lblText.Text = "Message: " + ex.Message + " Stack Trace: " + ex.StackTrace;
            }
        }

        #endregion
    }
}