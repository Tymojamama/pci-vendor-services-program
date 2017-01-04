using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using CRMModel = PCI.VSP.Data.CRM.Model;
using ServiceModel = PCI.VSP.Services.Model;
using PCI.VSP.Web.Security;
using System.Drawing;
using PCI.VSP.Services;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Data.Enums;

namespace PCI.VSP.Web.Vendor
{
    public partial class ProjectInquiriesDialog : System.Web.UI.Page
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
                if (ViewState[Constants.ProjectInquiries] == null)
                {
                    ServiceModel.IUser user = new Security.Utility().GetUser();
                    List<ServiceModel.IAccountQuestion> liaq = new VspService().GetVendorProjectInquiries(user.AccountId, user.Id, ProjectVendorId);

                    foreach (var iaq in liaq)
                    {
                        if (iaq is CRMModel.VendorQuestion)
                            iaq.Notes = new AnnotationDataLogic(_authRequest).RetrieveNotes(iaq.Id, "vsp_vendorquestion", new string[] { "annotationid", "filename", "createdon" });
                        else if (iaq is CRMModel.ClientQuestion)
                            iaq.Notes = new AnnotationDataLogic(_authRequest).RetrieveNotes(iaq.Id, "vsp_clientquestion", new string[] { "annotationid", "filename", "createdon" });
                    }

                    ViewState[Constants.ProjectInquiries] = liaq;
                }
                return ViewState[Constants.ProjectInquiries] as List<ServiceModel.IAccountQuestion>;
            }
        }

        private List<CRMModel.InvestmentAssetClass> AssetClasses
        {
            get
            {
                return new InvestmentAssetClassDataLogic(_authRequest).RetrieveAll();
            }
        }

        private Guid? ProjectVendorId
        {
            get
            {
                if (!Request.QueryString.AllKeys.Contains("ProjectVendorId"))
                    return new Guid?();
                return Guid.Parse(Request.QueryString["ProjectVendorId"]);
            }
        }

        private Guid? VendorProductId
        {
            get
            {
                const String vendorProductIdDesc = "VendorProductId";
                Guid? projectVendorId = ProjectVendorId;
                if (!projectVendorId.HasValue)
                    return new Guid?();
                if (ViewState[vendorProductIdDesc] != null)
                    return (Guid)ViewState[vendorProductIdDesc];

                List<Services.Model.VendorProjectInquirySummary> vcisl = (List<Services.Model.VendorProjectInquirySummary>)Session[Constants.VendorClientInquirySummary];
                foreach (Services.Model.VendorProjectInquirySummary vcis in vcisl)
                {
                    if (vcis.ProjectVendorId == projectVendorId.Value)
                    {
                        ViewState[vendorProductIdDesc] = vcis.VendorProductId;
                        return vcis.VendorProductId;
                    }
                }
                return new Guid?();
            }
        }

        private bool DisableAll
        {
            get
            {
                if (ViewState["DisableAll"] == null)
                {
                    if (ProjectVendorId.HasValue)
                        ViewState["DisableAll"] = (new ProjectVendorDataLogic(_authRequest).Retrieve(ProjectVendorId.Value).Status != ProjectVendorStatuses.Pending);
                    else
                        ViewState["DisableAll"] = false;
                }
                return (bool)ViewState["DisableAll"];
            }
            set
            {
                ViewState["DisableAll"] = value;
            }
        }

        private Data.CRM.Model.ClientProject ClientProject
        {
            get
            {
                if (Session[Constants.ClientProject] == null)
                {
                    Services.VspService service = new Services.VspService();
                    Session[Constants.ClientProject] = service.GetClientProjectByProjectVendor(this.ProjectVendorId.Value);
                }
                return Session[Constants.ClientProject] as Data.CRM.Model.ClientProject;
            }
        }

        private ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes _type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion;

        private int _offset = 0;

        #endregion

        #region Events

        protected void Page_Init(object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                answers_confirm.Visible = false;
                hfType.Value = ((int)_type).ToString();
                InitialLoad();
            }
            else
            {
                string ctrlname = Page.Request.Params.Get("__EVENTTARGET");
                if (ctrlname == null || ctrlname == string.Empty)
                {
                    _type = (ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes)int.Parse(hfType.Value);

                    switch (_type)
                    {
                        case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion:
                            ReloadData(hfCategoryId.Value, hfFunctionId.Value);
                            break;
                        case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.ProjectSpecificQuestion:
                        case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.Fee:
                            LoadData(_type, hfCategoryId.Value, hfFunctionId.Value);
                            break;
                        case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.PlanAssumption:
                        case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.InvestmentAssumption:
                        case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.RevenueSharing:
                            LoadData(_type, null, null);
                            break;
                    }

                    answers_confirm.Visible = (QAUserControl1.Visible && !QAUserControl1.DisableAll) || (InvestmentAssumptionsControl1.Visible && !InvestmentAssumptionsControl1.DisableAll) || (RevenueSharingControl1.Visible && !RevenueSharingControl1.DisableAll);
                }
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
                    hfType.Value = ((int)qci.Type).ToString();

                    switch (qci.Type)
                    {
                        case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion:
                        case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.ProjectSpecificQuestion:
                            hfCategoryId.Value = qci.CategoryId.ToString();
                            hfFunctionId.Value = qci.FunctionId.ToString();
                            LoadData(qci.Type, qci.CategoryId.ToString(), qci.FunctionId.ToString());
                            break;
                        case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.PlanAssumption:
                        case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.Fee:
                            hfCategoryId.Value = qci.CategoryId.ToString();
                            hfFunctionId.Value = qci.FunctionId.ToString();
                            LoadData(qci.Type, qci.CategoryId.ToString(), qci.FunctionId.ToString());
                            break;
                        case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.InvestmentAssumption:
                            PopulateInvestmentAssumpControl(_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.InvestmentAssumption).ToList(), true);
                            break;
                        case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.RevenueSharing:
                            PopulateRevenueSharingControl(_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.InvestmentAssumption && (a as CRMModel.VendorQuestion).InvestmentAssumptionsConfirmed).ToList(), true);
                            break;
                    }

                    answers_confirm.Visible = (QAUserControl1.Visible && !QAUserControl1.DisableAll) || (InvestmentAssumptionsControl1.Visible && !InvestmentAssumptionsControl1.DisableAll) || (RevenueSharingControl1.Visible && !RevenueSharingControl1.DisableAll);
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
            List<ServiceModel.QuestionCategorizationItem> qcil = new List<ServiceModel.QuestionCategorizationItem>();

            if (_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.PlanAssumption).Count() > 0)
            {
                var qci = new ServiceModel.QuestionCategorizationItem() { DisplayName = "Plan Assumptions", Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.PlanAssumption, CategorySortOrder = GetSortOrder() };
                qci.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(qci);
                qcil.Add(qci);
            }

            if (_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.InvestmentAssumption).Count() > 0)
            {
                var qci = new ServiceModel.QuestionCategorizationItem() { DisplayName = "Investment Assumptions", Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.InvestmentAssumption, CategorySortOrder = GetSortOrder() };
                qci.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(qci);
                qcil.Add(qci);

                qci = new ServiceModel.QuestionCategorizationItem() { DisplayName = "Revenue Sharing", Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.RevenueSharing, CategorySortOrder = GetSortOrder() };
                qci.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(qci);
                qcil.Add(qci);
            }

            if (_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.ProjectSpecificQuestion).Count() > 0)
            {
                var qci = new ServiceModel.QuestionCategorizationItem() { DisplayName = "Project Specific Questions", Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.ProjectSpecificQuestion, CategorySortOrder = GetSortOrder() };
                qci.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(qci);
                qcil.Add(qci);
            }

            if (_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.Fee).Count() > 0)
            {
                var fees = new ServiceModel.QuestionCategorizationItem() { DisplayName = "Fees", Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.Fee, CategorySortOrder = GetSortOrder() };
                fees.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(fees);
                qcil.Add(fees);
                
                foreach (var item in _questions.Where(z => z.QuestionType == Data.Enums.QuestionTypes.Fee).OrderBy(z => z.CategoryId).Select(z => new { z.CategoryId, z.FunctionId }).Distinct())
                {
                    var qc = _questionCategories.Where(a => a.Key == item.CategoryId).Select(a => a.Value).FirstOrDefault();
                    int categorySortOrder = 0;

                    if (qcil.Where(z => z.CategoryId == item.CategoryId && z.FunctionId == Guid.Empty && z.Type == ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.Fee && z.DisplayName != "Fees").Count() == 0)
                    {
                        #region Create Category

                        if (qc != null)
                        {
                            var qci = new ServiceModel.QuestionCategorizationItem();
                            qci.CategoryId = qc.Id;
                            qci.DisplayName = qci.CategoryName = qc.Name;
                            qci.CategorySortOrder = categorySortOrder = GetSortOrder(fees.CategorySortOrder, qc.SortOrder);
                            qci.Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.Fee;
                            qci.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(qci);
                            qci.SubItemLevel = 1;
                            qcil.Add(qci);
                        }
                        else // Uncategorized
                        {
                            var qci = new ServiceModel.QuestionCategorizationItem();
                            qci.CategoryId = Guid.Empty;
                            qci.DisplayName = qci.CategoryName = "Uncategorized";
                            qci.CategorySortOrder = categorySortOrder = -1;
                            qci.Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.Fee;
                            qci.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(qci);
                            qci.SubItemLevel = 1;
                            qcil.Add(qci);
                        }

                        #endregion
                    }
                    else
                    {
                        categorySortOrder = qcil.Where(z => z.CategoryId == item.CategoryId && z.FunctionId == Guid.Empty).Select(z => z.CategorySortOrder).FirstOrDefault();
                    }

                    #region Create Function

                    var qf = _questionFunctions.Where(a => a.Key == item.FunctionId).Select(a => a.Value).FirstOrDefault();

                    if (qf != null && qc != null)
                    {
                        var qci = new ServiceModel.QuestionCategorizationItem()
                        {
                            CategoryId = qc.Id,
                            CategoryName = qc.Name,
                            CategorySortOrder = categorySortOrder + 1,
                            FunctionId = qf.Id,
                            DisplayName = qf.Name,
                            FunctionName = qf.Name,
                            FunctionSortOrder = GetSortOrder(categorySortOrder, qf.SortOrder),
                            SubItemLevel = 2,
                            Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.Fee
                        };
                        qci.JSON = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(qci);

                        qcil.Add(qci);
                    }

                    #endregion
                }
            }

            #region Add Question Categories

            var Categories = _questions.Where(a => a.QuestionType == QuestionTypes.SearchQuestion_Filter2).Select(a => a.CategoryId).Distinct();
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

            var Functions = _questions.Where(a => a.QuestionType == QuestionTypes.SearchQuestion_Filter2 || a.QuestionType == QuestionTypes.ProjectSpecificQuestion).Select(a => new { a.CategoryId, a.FunctionId }).Distinct();
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
                PopulateQAControl(_questions.Where(a => a.CategoryId == c && a.FunctionId == f && a is CRMModel.VendorQuestion && (a.QuestionType == QuestionTypes.SearchQuestion_Filter2 || a.QuestionType == QuestionTypes.ProjectSpecificQuestion)).ToList());
            else
                PopulateQAControl(_questions.Where(a => a.CategoryId == c && a is CRMModel.VendorQuestion && (a.QuestionType == QuestionTypes.SearchQuestion_Filter2 || a.QuestionType == QuestionTypes.ProjectSpecificQuestion)).ToList());
        }

        private void LoadData(ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes type, string categoryId, string functionId)
        {
            List<PCI.VSP.Services.Model.IAccountQuestion> aql = new List<ServiceModel.IAccountQuestion>();
            Guid c, f;
            Guid.TryParse(categoryId, out c);
            Guid.TryParse(functionId, out f);

            switch (type)
            {
                case ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.SearchQuestion:
                    if (f != Guid.Empty)
                        PopulateQAControl(_questions.Where(a => a.CategoryId == c && a.FunctionId == f && a is CRMModel.VendorQuestion && (a.QuestionType == QuestionTypes.SearchQuestion_Filter2)).ToList());
                    else
                        PopulateQAControl(_questions.Where(a => a.CategoryId == c && a is CRMModel.VendorQuestion && (a.QuestionType == QuestionTypes.SearchQuestion_Filter2)).ToList());
                    break;
                case ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.ProjectSpecificQuestion:
                    PopulateQAControl(_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.ProjectSpecificQuestion).ToList(), false);
                    break;
                case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.PlanAssumption:
                    PopulateQAControl(_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.PlanAssumption).ToList(), true);
                    break;
                case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.Fee:
                    if (c == Guid.Empty && f == Guid.Empty)
                        PopulateQAControl(_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.Fee).ToList());
                    else if (c != Guid.Empty && f != Guid.Empty)
                        PopulateQAControl(_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.Fee && a.CategoryId == c && a.FunctionId == f).ToList());
                    else
                        PopulateQAControl(_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.Fee && a.CategoryId == c).ToList());
                    break;
                case PCI.VSP.Services.Model.QuestionCategorizationItem.CategorizationQuestionTypes.InvestmentAssumption:
                    PopulateInvestmentAssumpControl(_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.InvestmentAssumption).ToList(), false);
                    break;
                case ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.RevenueSharing:
                    PopulateRevenueSharingControl(_questions.Where(a => a.QuestionType == Data.Enums.QuestionTypes.InvestmentAssumption && (a as CRMModel.VendorQuestion).InvestmentAssumptionsConfirmed).ToList(), false);
                    break;
            }
        }

        /// <summary>
        /// Populate the QA User Control with Client Project Questions
        /// </summary>
        /// <param name="aql">List of IAccountQuestion (Vendor Question)</param>
        private void PopulateQAControl(List<PCI.VSP.Services.Model.IAccountQuestion> aql, bool disableAllOverride = false)
        {
            try
            {
                QAUserControl1.DisableAll = disableAllOverride ? disableAllOverride : DisableAll || DisplayedQuestionsAreConfirmed(aql);
                QAUserControl1.Visible = true;
                InvestmentAssumptionsControl1.Visible = false;
                RevenueSharingControl1.Visible = false;
                QAUserControl1.SaveRequest += UpdateClientProjectQuestions;
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
        /// Method to populate the Investment Assumption User Control
        /// </summary>
        /// <param name="aql">List of IAccountQuestion (Vendor Question)</param>
        private void PopulateInvestmentAssumpControl(List<PCI.VSP.Services.Model.IAccountQuestion> aql, bool bindData)
        {
            InvestmentAssumptionsControl1.DisableAll = DisableAll || DisplayedQuestionsAreConfirmed(aql);
            QAUserControl1.Visible = false;
            InvestmentAssumptionsControl1.Visible = true;
            RevenueSharingControl1.Visible = false;
            InvestmentAssumptionsControl1.SaveRequest += UpdateInvestmentAssumptions;
            InvestmentAssumptionsControl1.Audience = Enums.UserType.Vendor;
            InvestmentAssumptionsControl1.AssetClasses = AssetClasses;
            InvestmentAssumptionsControl1.InvestmentAssumptions = aql;
            InvestmentAssumptionsControl1.Closeable = true;
            if (bindData) InvestmentAssumptionsControl1.DataBind();
        }

        /// <summary>
        /// Method to populate the Revenue Sharing User Control
        /// </summary>
        /// <param name="aql">List of IAccountQuestion (Vendor Question)</param>
        private void PopulateRevenueSharingControl(List<PCI.VSP.Services.Model.IAccountQuestion> aql, bool bindData)
        {
            QAUserControl1.Visible = false;
            InvestmentAssumptionsControl1.Visible = false;
            RevenueSharingControl1.DisableAll = DisableAll || DisplayedQuestionsAreConfirmed(aql); ;
            RevenueSharingControl1.Visible = true;
            RevenueSharingControl1.SaveRequest += UpdateRevenueSharing;
            RevenueSharingControl1.Audience = Enums.UserType.Vendor;
            RevenueSharingControl1.AssetClasses = AssetClasses;
            RevenueSharingControl1.Closeable = true;
            RevenueSharingControl1.InvestmentAssumptions = aql;
            if (bindData) RevenueSharingControl1.DataBind();
        }

        /// <summary>
        /// Save Client Project Questions
        /// </summary>
        /// <param name="qac">QA User Control</param>
        /// <param name="e">Save Request Event Args</param>
        private void UpdateClientProjectQuestions(Controls.QAUserControl qac, Controls.QAUserControl.SaveRequestEventArgs e)
        {
            if (answers_confirm.Checked)
                for (int i = 0; i < e.AccountQuestions.Count; i++)
                    if (e.AccountQuestions[i].Status == Data.Enums.AccountQuestionStatuses.Answered || e.AccountQuestions[i].Status == Data.Enums.AccountQuestionStatuses.Rejected || e.AccountQuestions[i].Status == Data.Enums.AccountQuestionStatuses.Unspecified)
                        e.AccountQuestions[i].Status = Data.Enums.AccountQuestionStatuses.AccountConfirmed;
            new VspService().UpdateVendorQuestions(e.AccountQuestions, new Security.Utility().GetUser().Id, answers_confirm.Checked);
            UpdateProjectVendorToVendorApproved();
        }

        /// <summary>
        /// Method to save Vendor Investment Assumptions
        /// </summary>
        /// <param name="iac">Investment Assumptions Control</param>
        /// <param name="e">Save Request Event Args</param>
        private void UpdateInvestmentAssumptions(Controls.InvestmentAssumptionsControl iac, Controls.InvestmentAssumptionsControl.SaveRequestEventArgs e)
        {
            try
            {
                if (answers_confirm.Checked)
                    for (int i = 0; i < e.AccountQuestions.Count; i++)
                        if (e.AccountQuestions[i].Status == Data.Enums.AccountQuestionStatuses.Answered || e.AccountQuestions[i].Status == Data.Enums.AccountQuestionStatuses.Rejected || e.AccountQuestions[i].Status == Data.Enums.AccountQuestionStatuses.Unspecified)
                            e.AccountQuestions[i].Status = Data.Enums.AccountQuestionStatuses.AccountConfirmed;

                new Services.VspService().UpdateVendorQuestions(e.AccountQuestions, new Security.Utility().GetUser().Id, answers_confirm.Checked);
                UpdateProjectVendorToVendorApproved();
                PopulateInvestmentAssumpControl(e.AccountQuestions, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method to save Vendor Revenue Sharing
        /// </summary>
        /// <param name="iac">Revenue Sharing Control</param>
        /// <param name="e">Save Request Event Args</param>
        private void UpdateRevenueSharing(Controls.RevenueSharingControl iac, Controls.RevenueSharingControl.SaveRequestEventArgs e)
        {
            try
            {
                if (answers_confirm.Checked)
                    for (int i = 0; i < e.AccountQuestions.Count; i++)
                        if (e.AccountQuestions[i].Status == Data.Enums.AccountQuestionStatuses.Answered || e.AccountQuestions[i].Status == Data.Enums.AccountQuestionStatuses.Rejected || e.AccountQuestions[i].Status == Data.Enums.AccountQuestionStatuses.Unspecified)
                            e.AccountQuestions[i].Status = Data.Enums.AccountQuestionStatuses.AccountConfirmed;

                new Services.VspService().UpdateVendorQuestions(e.AccountQuestions, new Security.Utility().GetUser().Id, answers_confirm.Checked);
                //new ProjectVendorDataLogic(_authRequest).SetProjectVendorStatusToVendorApproved(ClientProject.Id, VendorProductId.Value);
                UpdateProjectVendorToVendorApproved();
                PopulateRevenueSharingControl(e.AccountQuestions, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateProjectVendorToVendorApproved()
        {
            bool allConfirmed = true;
            foreach (CRMModel.VendorQuestion vq in _questions.Where(z => z is CRMModel.VendorQuestion))
            {
                if (vq.Status != AccountQuestionStatuses.AccountConfirmed && vq.Status != AccountQuestionStatuses.PCI_Confirmed)
                {
                    allConfirmed = false;
                    break;
                }
            }

            if (allConfirmed)
            {
                var pvdl = new ProjectVendorDataLogic(_authRequest);
                var pv = pvdl.Retrieve(ProjectVendorId.Value);

                if (pv.Status == ProjectVendorStatuses.Pending)
                {
                    pv.Status = ProjectVendorStatuses.VendorApproved;
                    pvdl.Save(pv);
                }

                DisableAll = allConfirmed;
            }
        }

        private bool DisplayedQuestionsAreConfirmed(List<ServiceModel.IAccountQuestion> iaql)
        {
            bool allConfirmed = true;

            foreach (CRMModel.VendorQuestion vq in iaql.Where(z => z is CRMModel.VendorQuestion))
            {
                if (vq.Status != AccountQuestionStatuses.AccountConfirmed && vq.Status != AccountQuestionStatuses.PCI_Confirmed)
                {
                    allConfirmed = false;
                    break;
                }
            }

            return allConfirmed;
        }

        private int GetSortOrder(int? value = null, int? offset = null)
        {
            //int answer = int.MinValue + _offset;
            //_offset++;
            //return answer;
            return (value.HasValue ? value.Value : int.MinValue) + (offset.HasValue ? offset.Value : _offset++);
        }

        #endregion        
    }
}