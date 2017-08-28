using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ServiceModel = PCI.VSP.Services.Model;
using PCI.VSP.Web.Enums;
using System.Drawing;
using DataModel = PCI.VSP.Data.CRM.Model;

namespace PCI.VSP.Web.Controls
{
    public partial class InvestmentAssumptionsControl : System.Web.UI.UserControl
    {
        #region Private Variables

        private string _validationGroup = "InvestmentAssumptions";
        private string _errorMessage = "Investment Assumption #{0}'s {1} is not a valid {2}.";

        /// <summary>
        /// Whether or not the usercontrol is within a page that can be closed with CloseIFrame JavaScript method
        /// </summary>
        private bool _closeable = false;

        /// <summary>
        /// Whether or not to disable all questions, because of confirmed answers or closed project
        /// </summary>
        private bool _disableAll = false;

        #endregion

        #region Public Variables

        /// <summary>
        /// User currently viewing the control
        /// </summary>
        public UserType Audience = UserType.Unspecified;
        
        /// <summary>
        /// List of Investment Assumptions
        /// </summary>
        public List<ServiceModel.IAccountQuestion> InvestmentAssumptions = new List<ServiceModel.IAccountQuestion>();

        /// <summary>
        /// List of Investment (Assumption) Asset Classes
        /// </summary>
        public List<DataModel.InvestmentAssetClass> AssetClasses = new List<DataModel.InvestmentAssetClass>();

        public delegate void SaveRequestEventHandler(InvestmentAssumptionsControl sender, SaveRequestEventArgs srea);
        public event SaveRequestEventHandler SaveRequest;

        /// <summary>
        /// Whether or not the usercontrol is within a page that can be closed with CloseIFrame JavaScript method
        /// </summary>
        public bool Closeable
        {
            get
            {
                return _closeable;
            }
            set
            {
                _closeable = value;
            }
        }

        /// <summary>
        /// Whether or not to disable all questions, because of confirmed answers or closed project
        /// </summary>
        public bool DisableAll
        {
            get
            {
                return _disableAll;
            }
            set
            {
                _disableAll = value;
            }
        }

        #endregion

        #region Events

        public override void DataBind()
        {
            try
            {
                if (Audience == UserType.Unspecified) return;
                base.DataBind();
                rptQuestions.DataSource = InvestmentAssumptions;
                rptQuestions.DataBind();
                btnSave.Visible = !DisableAll;
                btnSaveAndClose.Visible = Closeable && !DisableAll;
                btnClose.Visible = Closeable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal List<ServiceModel.IAccountQuestion> GetFields()
        {
            List<ServiceModel.IAccountQuestion> dataSource = (List<ServiceModel.IAccountQuestion>)rptQuestions.DataSource;

            for (int i = 0; i < rptQuestions.Items.Count; i++)
            {
                if (Audience == UserType.Vendor)
                {
                    DropDownList ddl = (DropDownList)rptQuestions.Items[i].FindControl("Confirmation");
                    if (ddl.SelectedValue.ToUpper() == "TRUE")
                        ((DataModel.VendorQuestion)dataSource[i]).InvestmentAssumptionsConfirmed = true;
                    else
                        ((DataModel.VendorQuestion)dataSource[i]).InvestmentAssumptionsConfirmed = false;
                }
            }

            return dataSource;
        }

        protected void rptQuestions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var AssetClass = e.Item.FindControl("ddlAssetClass") as DropDownList;
                var AssetFund = e.Item.FindControl("AssetFundTextBox") as TextBox;
                var AssetSymbol = e.Item.FindControl("AssetSymbolTextBox") as TextBox;
                var Assets = e.Item.FindControl("AssetsTextBox") as TextBox;
                var AnnualContribution = e.Item.FindControl("AnnualContributionsTextBox") as TextBox;
                var Participants = e.Item.FindControl("ParticipantsTextBox") as TextBox;
                var Confirmation = e.Item.FindControl("Confirmation") as DropDownList;
                var ddlRequired = e.Item.FindControl("ddlRequired") as DropDownList;
                var hfId = e.Item.FindControl("hfId") as HiddenField;
                var iaq = (ServiceModel.IAccountQuestion)e.Item.DataItem;
                var Number = (e.Item.ItemIndex + 1).ToString(); // Investment Assumption Number, User Display Only
                (e.Item.FindControl("litNumber") as Literal).Text = Number + ")";

                hfId.Value = iaq.Id.ToString();

                AssetClass.Items.Clear();
                for (int i = 0; i < AssetClasses.Count; i++)
                    AssetClass.Items.Add(new ListItem(AssetClasses[i].Name, AssetClasses[i].Id.ToString()));

                if (Audience == UserType.Vendor) AssetClass.Items.Insert(0, new ListItem("Not Selected", "Not Selected"));

                AssetClass.SelectedValue = iaq.AssetClassId.ToString();

                if (DisableAll || iaq.Status == Data.Enums.AccountQuestionStatuses.AccountConfirmed || iaq.Status == Data.Enums.AccountQuestionStatuses.PCI_Confirmed)
                {
                    AssetClass.Attributes.Add("disabled", "disabled");
                    AssetFund.Attributes.Add("disabled", "disabled");
                    AssetSymbol.Attributes.Add("disabled", "disabled");
                    Assets.Attributes.Add("disabled", "disabled");
                    AnnualContribution.Attributes.Add("disabled", "disabled");
                    Participants.Attributes.Add("disabled", "disabled");
                    Confirmation.Attributes.Add("disabled", "disabled");
                    ddlRequired.Attributes.Add("disabled", "disabled");
                }

                switch (Audience)
                {
                    case UserType.PCI:
                        UpdateRegularExpressionValidator(e.Item, "revParticipants", Participants, Number, "Participants", "integer");
                        UpdateRegularExpressionValidator(e.Item, "revAssets", Assets, Number, "Assets", "currency value");
                        UpdateRegularExpressionValidator(e.Item, "revAnnualContributions", AnnualContribution, Number, "Annual Contribution", "currency value");

                        if (iaq is DataModel.VendorQuestion)
                        {
                            Confirmation.SelectedValue = ((DataModel.VendorQuestion)e.Item.DataItem).InvestmentAssumptionsConfirmed.ToString();
                            e.Item.FindControl("trIsRequired").Visible = false;
                        }
                        else if (iaq is DataModel.ClientQuestion)
                        {
                            ddlRequired.SelectedValue = ((DataModel.ClientQuestion)e.Item.DataItem).VendorMustOfferFund.ToString();
                            e.Item.FindControl("trConfirm").Visible = false;
                        }

                        break;
                    case UserType.Client:
                        UpdateRegularExpressionValidator(e.Item, "revParticipants", Participants, Number, "Participants", "integer");
                        UpdateRegularExpressionValidator(e.Item, "revAssets", Assets, Number, "Assets", "currency value");
                        UpdateRegularExpressionValidator(e.Item, "revAnnualContributions", AnnualContribution, Number, "Annual Contribution", "currency value");
                        ddlRequired.SelectedValue = ((DataModel.ClientQuestion)e.Item.DataItem).VendorMustOfferFund.ToString();
                        e.Item.FindControl("trConfirm").Visible = false;
                        break;
                    case UserType.Vendor:
                        AssetClass.Attributes.Add("disabled", "disabled");
                        AssetFund.Attributes.Add("disabled", "disabled");
                        AssetSymbol.Attributes.Add("disabled", "disabled");
                        Assets.Attributes.Add("disabled", "disabled");
                        AnnualContribution.Attributes.Add("disabled", "disabled");
                        Participants.Attributes.Add("disabled", "disabled");
                        (e.Item.FindControl("revParticipants") as RegularExpressionValidator).Visible = false;
                        (e.Item.FindControl("revAssets") as RegularExpressionValidator).Visible = false;
                        (e.Item.FindControl("revAnnualContributions") as RegularExpressionValidator).Visible = false;

                        DataModel.VendorQuestion vq = (DataModel.VendorQuestion)e.Item.DataItem;
                        Confirmation.SelectedValue = vq.InvestmentAssumptionsConfirmed.ToString();
                        e.Item.FindControl("trIsRequired").Visible = false;
                        break;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveInvestmentAssumptions();
        }

        protected void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            SaveInvestmentAssumptions();
            if (Closeable) Page.ClientScript.RegisterStartupScript(this.GetType(), "windowClose", "parent.CloseIFrame();", true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to save the Investment Assumptions
        /// </summary>
        private void SaveInvestmentAssumptions()
        {
            Page.Validate(_validationGroup);
            if (!Page.IsValid) return;
            if (rptQuestions.Items.Count == 0 || InvestmentAssumptions.Count == 0) return;
            List<ServiceModel.IAccountQuestion> aql = new List<ServiceModel.IAccountQuestion>();

            for (int i = 0; i < rptQuestions.Items.Count; i++)
            {
                Guid id = new Guid();
                if (!Guid.TryParse((rptQuestions.Items[i].FindControl("hfId") as HiddenField).Value, out id)) continue;
                ServiceModel.IAccountQuestion aq = InvestmentAssumptions.Where(a => a.Id == id).FirstOrDefault();
                if (aq == null) continue;

                switch (Audience)
                {
                    case UserType.PCI:
                        if (aq is ServiceModel.VendorQuestion)
                            UpdateVendorQuestion(ref aq, rptQuestions.Items[i]);
                        else if (aq is ServiceModel.ClientQuestion)
                            UpdateClientQuestion(ref aq, rptQuestions.Items[i]);
                        break;
                    case UserType.Client:
                        UpdateClientQuestion(ref aq, rptQuestions.Items[i]);
                        break;
                    case UserType.Vendor:
                        UpdateVendorQuestion(ref aq, rptQuestions.Items[i]);
                        break;
                }

                if (aq != null)
                    aql.Add(aq);
            }

            if (aql.Count > 0) SaveRequest(this, new SaveRequestEventArgs(aql));
            SetStatusLabel(Statuses.SaveSucceeded);
        }

        /// <summary>
        /// Assign the control to validate ID and error message to the Regular Expression Validator
        /// </summary>
        /// <param name="ri">Repeater Item</param>
        /// <param name="validatorId">ID of the Regular Expression Validator</param>
        /// <param name="control">Textbox control to validate</param>
        /// <param name="number">Investment Assumption Number that the user sees</param>
        /// <param name="displayName">Name of the field being validated that the user sees</param>
        /// <param name="dateType">Data type of the field being validated</param>
        private void UpdateRegularExpressionValidator(RepeaterItem ri, string validatorId, TextBox control, string number, string displayName, string dateType)
        {
            var rev = ri.FindControl(validatorId) as RegularExpressionValidator;
            rev.ControlToValidate = control.ID;
            rev.ErrorMessage = String.Format(_errorMessage, number, displayName, dateType);
        }

        /// <summary>
        /// Update Vendor Question with latest answers
        /// </summary>
        /// <param name="iaq">Vendor Question in IAccountQuestion Form</param>
        /// <param name="ri">Repeater Item</param>
        private void UpdateVendorQuestion(ref ServiceModel.IAccountQuestion iaq, RepeaterItem ri)
        {
            ((ServiceModel.VendorQuestion)iaq).InvestmentAssumptionsConfirmed = GetFieldBoolean(ri.FindControl("Confirmation") as DropDownList);
        }

        /// <summary>
        /// Update Client Question with latest Answers
        /// </summary>
        /// <param name="iaq">Client Question in IAccountQuestion Form</param>
        /// <param name="ri">Repeater Item</param>
        private void UpdateClientQuestion(ref ServiceModel.IAccountQuestion iaq, RepeaterItem ri)
        {
            Guid aci = new Guid();
            Guid.TryParse((ri.FindControl("ddlAssetClass") as DropDownList).SelectedValue, out aci);
            iaq.AssetClassId = aci;
            iaq.AssetFund = (ri.FindControl("AssetFundTextBox") as TextBox).Text;
            iaq.AssetSymbol = (ri.FindControl("AssetSymbolTextBox") as TextBox).Text;

            decimal assets = 0;
            decimal.TryParse((ri.FindControl("AssetsTextBox") as TextBox).Text, out assets);
            iaq.Assets = assets;

            decimal contributions = 0;
            decimal.TryParse((ri.FindControl("AnnualContributionsTextBox") as TextBox).Text, out contributions);
            iaq.AnnualContributions = contributions;

            int participants = 0;
            if (int.TryParse((ri.FindControl("ParticipantsTextBox") as TextBox).Text, out participants))
            iaq.Participants = participants;

            ((DataModel.ClientQuestion)iaq).VendorMustOfferFund = GetFieldBoolean((ri.FindControl("ddlRequired") as DropDownList));
        }

        private Boolean GetFieldBoolean(DropDownList ddl)
        {
            if (String.IsNullOrWhiteSpace(ddl.SelectedValue))
                return false;
            return Convert.ToBoolean(ddl.SelectedValue);
        }

        /// <summary>
        /// Sets the status label that tells the user whether or not the save was successful
        /// </summary>
        /// <param name="status">Save Statuses</param>
        private void SetStatusLabel(Statuses status)
        {
            Label sl = StatusLabel;
            switch (status)
            {
                case Statuses.SaveFailed:
                    sl.Text = "An error occurred during the save operation.";
                    sl.ForeColor = Color.Red;
                    sl.Font.Bold = true;
                    break;
                case Statuses.SaveSucceeded:
                    sl.Text = "The save operation succeeded.";
                    sl.ForeColor = Color.Black;
                    sl.Font.Bold = false;
                    break;
                case Statuses.Unspecified:
                default:
                    sl.Text = String.Empty;
                    break;
            }
        }

        #endregion

        public class SaveRequestEventArgs
        {
            public SaveRequestEventArgs(List<ServiceModel.IAccountQuestion> aql) { this.AccountQuestions = aql; }
            public List<ServiceModel.IAccountQuestion> AccountQuestions { get; private set; }
        }

        /// <summary>
        /// Save Statuses
        /// </summary>
        private enum Statuses
        {
            Unspecified = 0,
            SaveSucceeded = 1,
            SaveFailed = 2
        }
    }
}