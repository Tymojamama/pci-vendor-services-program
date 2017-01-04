using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PCI.VSP.Services;
using PCI.VSP.Data.CRM.Model;
using PCI.VSP.Data.CRM.DataLogic;
using ServiceModel = PCI.VSP.Services.Model;
using PCI.VSP.Data.Enums;
using PCI.VSP.Web.Controls;
using PCI.VSP.Web.Enums;
using System.Configuration;

namespace PCI.VSP.Web.CrmIFrames.VendorMonitoring
{
    public partial class VMMaster : System.Web.UI.MasterPage
    {
        public VendorMonitoringAnswerTypes VendorMonitoringAnswerType = VendorMonitoringAnswerTypes.None;

        private Guid _clientProjectId = Guid.Empty;
        private AuthenticationRequest _authRequest = new AuthenticationRequest() { Username = Data.Globals.CrmServiceSettings.Username, Password = Data.Globals.CrmServiceSettings.Password };
        private List<PlanAccountServiceProvider> _serviceProviders = null;
        private Dictionary<Guid, PlanAccountServiceProviderType> _serviceProviderTypes = null;
        private Dictionary<Guid, QuestionCategory> _usedQuestionCategories = null;
        private Dictionary<Guid, QuestionFunction> _usedquestionFunctions = null;
        private Guid _serviceProviderId = Guid.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Request.QueryString.AllKeys.Contains("id"))
                {
                    lblText.Text = "No Client Project ID Specified.";
#if DEBUG
                    _clientProjectId = Guid.Parse("64EBCAEA-6704-E311-ABFF-00155D016411");  // VM Test Project
#endif
                }
                else if (Guid.TryParse(Request.QueryString["id"], out _clientProjectId)) { }
                else
                    lblText.Text = "Invalid Client Project ID Specified.";


                if (_clientProjectId != Guid.Empty)
                {
                    ClientProject clientProject = new ClientProjectDataLogic(_authRequest).Retrieve(_clientProjectId);

                    _serviceProviders = new PlanAccountDataLogic(_authRequest).GetServiceProvidersForPlan(clientProject.PlanAccountId);
                    if (this.VendorMonitoringAnswerType == VendorMonitoringAnswerTypes.Estimate)
                    {
                        // on Estimates tab, exclude those with end dates
                        _serviceProviders = _serviceProviders.Where(sp => !sp.EndDate.HasValue).ToList();
                    }
                    else if (this.VendorMonitoringAnswerType == VendorMonitoringAnswerTypes.Actual)
                    {
                        // default start/end dates
                        DateTime start_date = new DateTime(1, 1, 1), end_date = new DateTime(9999, 12, 31);

                        Guid as_of_date_id;
                        if (!Guid.TryParse(ConfigurationManager.AppSettings["AsOfDateQuestionID"], out as_of_date_id))
                            as_of_date_id = Guid.Empty;

                        // find 'As Of Date' question
                        ClientQuestion asOfDateQ = new ClientQuestionDataLogic(_authRequest).FetchByClientProject(_clientProjectId, true).FirstOrDefault(cq => cq.QuestionId == as_of_date_id);
                        if (asOfDateQ != null && DateTime.TryParse(asOfDateQ.Answer, out end_date))
                            start_date = new DateTime(end_date.Year, 1, 1);
                            
                        // on Actuals tab, exlclude those that don't occur within the project year
                        _serviceProviders = _serviceProviders.Where(sp =>
                        {
                            DateTime sp_start_date = sp.StartDate ?? new DateTime(1, 1, 1),
                                     sp_end_date = sp.EndDate ?? new DateTime(9999, 12, 31);

                            return
                                sp_start_date <= end_date && sp_start_date >= start_date ||   // start date falls in the range
                                sp_end_date <= end_date && sp_end_date >= start_date ||   // end date falls in the range
                                sp_start_date <= start_date && sp_end_date >= end_date;        // service provider includes project range
                        })
                        .ToList();
                    }
                    var serviceProviderTypeIds = _serviceProviders.Select(z => z.ServiceProviderTypeId).Distinct().ToList();

                    _serviceProviderTypes = new PlanAccountServiceProviderTypeDataLogic(_authRequest).RetrieveAllIntoDictionary(serviceProviderTypeIds);
                    var serviceProviderCategories = _serviceProviderTypes.Values.Select(z => z.CategoryId).Distinct();
                    var serviceProviderFunctions = _serviceProviderTypes.Values.Select(z => z.FunctionId).Distinct();

                    _usedQuestionCategories = new QuestionCategoryDataLogic(_authRequest).RetrieveAllIntoDictionary().Where(z => serviceProviderCategories.Contains(z.Value.Id)).ToDictionary(z => z.Key, z => z.Value);
                    _usedquestionFunctions = new QuestionFunctionDataLogic(_authRequest).RetrieveAllIntoDictionary().Where(z => serviceProviderFunctions.Contains(z.Value.Id)).ToDictionary(z => z.Key, z => z.Value);

                    if (!IsPostBack)
                    {
                        rptCategories.DataSource = RetrieveNavigationMenu();
                        rptCategories.DataBind();
                    }
                    else
                    {
                        string ctrlname = Page.Request.Params.Get("__EVENTTARGET");
                        if (ctrlname == null || ctrlname == string.Empty)
                            reloadData();
                    }

                    switch (this.VendorMonitoringAnswerType)
                    {
                        case VendorMonitoringAnswerTypes.Actual :
                            btnCopyInvestments.Text = "Copy Selected to Estimates";
                            break;
                        case VendorMonitoringAnswerTypes.Estimate :
                            btnCopyInvestments.Text = "Copy Selected to Actuals";
                            break;
                        default :
                            btnCopyInvestments.Visible = false;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                lblText.Text = "Message: " + ex.Message + " Stack Trace: " + ex.StackTrace;
            }
        }

        private void reloadData()
        {
            try
            {
                if (string.IsNullOrEmpty(hfType.Value)) return;
                if (Guid.TryParse(hfType.Value, out _serviceProviderId))
                    PopulateQAControl();
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "reloadData", String.Format(Constants.AlertErrorMessage, "Reload category questions", ex.Message, ex.StackTrace), true);
            }

        }

        public void category_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton button = (LinkButton)sender;

                if (Guid.TryParse(button.CommandArgument, out _serviceProviderId))
                {
                    hfType.Value = _serviceProviderId.ToString();
                    PopulateQAControl();
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "category_Click", String.Format(Constants.AlertErrorMessage, "Retrieve category questions", ex.Message, ex.StackTrace), true);
            }
        }

        public List<ServiceModel.QuestionCategorizationItem> RetrieveNavigationMenu()
        {
            try
            {
                List<ServiceModel.QuestionCategorizationItem> qcil = new List<ServiceModel.QuestionCategorizationItem>();

                #region Generate Categories

                foreach (var questionCategory in _usedQuestionCategories)
                {
                    qcil.Add(new ServiceModel.QuestionCategorizationItem()
                    {
                        DisplayName = questionCategory.Value.Name,
                        CategorySortOrder = questionCategory.Value.SortOrder,
                        CategoryId = questionCategory.Key,
                        CategoryName = questionCategory.Value.Name,
                        Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.VendorMonitoring
                    });
                }

                #endregion

                #region Generate Functions

                var adl = new AccountDataLogic(_authRequest);

                foreach (var serviceProvider in _serviceProviders)
                {
                    var type = _serviceProviderTypes.Where(z => z.Key == serviceProvider.ServiceProviderTypeId).FirstOrDefault();
                    var category = _usedQuestionCategories.Where(z => z.Key == type.Value.CategoryId).FirstOrDefault();
                    var function = _usedquestionFunctions.Where(z => z.Key == type.Value.FunctionId).FirstOrDefault();

                    qcil.Add(new ServiceModel.QuestionCategorizationItem()
                    {
                        DisplayName = type.Value.Name + " - " + adl.Retrieve(serviceProvider.VendorAccountId).Name,
                        CategorySortOrder = (category.Value != null ? category.Value.SortOrder : 0),
                        CategoryId = category.Key,
                        CategoryName = (category.Value != null ? category.Value.Name : string.Empty),
                        FunctionId = serviceProvider.Id,
                        FunctionName = function.Value.Name,
                        FunctionSortOrder = function.Value.SortOrder,
                        SubItemLevel = 1,
                        Type = ServiceModel.QuestionCategorizationItem.CategorizationQuestionTypes.VendorMonitoring
                    });
                }

                #endregion

                return qcil.OrderBy(a => a.CategorySortOrder).ThenBy(a => a.FunctionSortOrder).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ServiceModel.IAccountQuestion> RetrieveVendorMonitoringQuestions(Guid serviceProviderId, VendorMonitoringAnswerTypes vendorMonitorAnswerType)
        {
            try
            {
                List<ServiceModel.IAccountQuestion> aql = new List<ServiceModel.IAccountQuestion>();
                var serviceProvider = _serviceProviders.Where(z => z.Id == serviceProviderId).FirstOrDefault();

                if (serviceProvider != null)
                {
                    var categoryId = _serviceProviderTypes.Where(z => z.Key == serviceProvider.ServiceProviderTypeId).Select(z => z.Value.CategoryId).FirstOrDefault();
                    var type = _serviceProviderTypes.Where(z => z.Key == serviceProvider.ServiceProviderTypeId).FirstOrDefault();
                    //List<VendorQuestion> vendorQuestions = new VendorQuestionDataLogic(_authRequest).RetrieveVendorMonitoringQuestions(serviceProvider.VendorAccountId, categoryId, type.Value.FunctionId, vendorMonitorAnswerType, true, type.Value.TemplateType, _clientProjectId, serviceProvider.Id);
                    List<VendorQuestion> vendorQuestions = new VendorQuestionDataLogic(_authRequest).FetchVendorMonitoringQuestions(serviceProvider.VendorAccountId, categoryId, type.Value.FunctionId, vendorMonitorAnswerType, true, type.Value.TemplateType, _clientProjectId, serviceProvider.Id);
                    aql = vendorQuestions.Select<VendorQuestion, Services.Model.VendorQuestion>(vq => new Services.Model.VendorQuestion(vq)).Cast<ServiceModel.IAccountQuestion>().ToList();
                }

                return aql;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateQuestions(Controls.QAUserControl qac, Controls.QAUserControl.SaveRequestEventArgs e)
        {
            try
            {
                new Services.VspService().UpdateVendorQuestions(e.AccountQuestions, Guid.Parse(Constants.CRMHouseAccountId), false);
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateQuestionsError", "alert('Message: " + ex.Message + "\\n\\rStack Trace: " + ex.StackTrace + "');", true);
            }
        }

        private void PopulateQAControl()
        {
            try
            {
                QAUserControl1.Visible = true;
                QAUserControl1.SaveRequest += UpdateQuestions;
                QAUserControl1.UserType = UserType.PCI;
                QAUserControl1.ViewAsType = UserType.Vendor;
                QAUserControl1.Questions = RetrieveVendorMonitoringQuestions(_serviceProviderId, VendorMonitoringAnswerType);
                QAUserControl1.AllowedFileTypes = new DocumentTypeDataLogic(_authRequest).RetrieveAllowedDocumentTypes();
                QAUserControl1.DataBind();
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "PopulateQAControl", String.Format(Constants.AlertErrorMessage, "Populate questions", ex.Message, ex.StackTrace), true);
            }
        }

        protected void btnCopyInvestments_Click(object sender, EventArgs e)
        {
            List<Services.Model.IAccountQuestion> updated_questions = new List<Services.Model.IAccountQuestion>();

            foreach (RepeaterItem rptItem in rptCategories.Items)
            {
                CheckBox chkCopy = rptItem.FindControl("chkCopy") as CheckBox;
                if (chkCopy != null && chkCopy.Visible && chkCopy.Checked)
                {
                    HiddenField hdnFunctionId = rptItem.FindControl("hdnFunctionId") as HiddenField;
                    HiddenField hdnCategoryId = rptItem.FindControl("hdnCategoryId") as HiddenField;

                    Guid functionId, categoryId;

                    if (hdnFunctionId != null && Guid.TryParse(hdnFunctionId.Value, out functionId) && functionId != Guid.Empty &&
                        hdnCategoryId != null && Guid.TryParse(hdnCategoryId.Value, out categoryId) && categoryId == new Guid(ConfigurationManager.AppSettings["InvestmentManagementId"]))
                    {
                        Services.Model.IAccountQuestion[] my_questions;
                        Services.Model.IAccountQuestion[] opposite_questions;
                        switch (VendorMonitoringAnswerType)
                        {
                            case VendorMonitoringAnswerTypes.Actual:
                                opposite_questions = RetrieveVendorMonitoringQuestions(functionId, VendorMonitoringAnswerTypes.Estimate)
                                    .OrderBy(q => q.FunctionId).ToArray();
                                my_questions = RetrieveVendorMonitoringQuestions(functionId, VendorMonitoringAnswerTypes.Actual)
                                    .OrderBy(q => q.FunctionId).ToArray();
                                break;
                            case VendorMonitoringAnswerTypes.Estimate:
                                opposite_questions = RetrieveVendorMonitoringQuestions(functionId, VendorMonitoringAnswerTypes.Actual)
                                    .OrderBy(q => q.FunctionId).ToArray();
                                my_questions = RetrieveVendorMonitoringQuestions(functionId, VendorMonitoringAnswerTypes.Estimate)
                                    .OrderBy(q => q.FunctionId).ToArray();
                                break;
                            default:
                                continue;
                        }

                        if (my_questions.Length != opposite_questions.Length)
                            continue;

                        for (int i = 0; i < my_questions.Length; i++)
                        {
                            ServiceModel.IAccountQuestion my_q = my_questions[i];
                            ServiceModel.IAccountQuestion opp_q = opposite_questions[i];

                            opp_q.Answer = my_q.Answer;
                            updated_questions.Add(opp_q);
                        }
                    }
                }
            }

            new Services.VspService().UpdateVendorQuestions(updated_questions.ToList(), Guid.Parse(Constants.CRMHouseAccountId), true);
        }
    }
}