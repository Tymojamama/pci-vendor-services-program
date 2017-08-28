using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ServiceModel = PCI.VSP.Services.Model;
using CRMModel = PCI.VSP.Data.CRM.Model;
using System.Drawing;
using PCI.VSP.Web.Enums;


namespace PCI.VSP.Web.Controls
{
    public partial class RevenueSharingControl : System.Web.UI.UserControl
    {
        #region Private Variables

        private ServiceModel.IUser _user = null;
        private Boolean _isReadOnly = false;

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
        /// List of Investment (Assumption) Asset Classes
        /// </summary>
        public List<CRMModel.InvestmentAssetClass> AssetClasses = new List<CRMModel.InvestmentAssetClass>();

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

        public delegate void SaveRequestEventHandler(RevenueSharingControl sender, SaveRequestEventArgs srea);
        public event SaveRequestEventHandler SaveRequest;

        /// <summary>
        /// List of Investment Assumptions
        /// </summary>
        public List<ServiceModel.IAccountQuestion> InvestmentAssumptions = new List<ServiceModel.IAccountQuestion>();

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

        protected void rptQuestions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    ServiceModel.VendorQuestion vq = (ServiceModel.VendorQuestion)e.Item.DataItem;
                    DropDownList ddl = (DropDownList)e.Item.FindControl("CalculationTypeDropDownList");
                    ddl.SelectedValue = Convert.ToInt32(vq.RevenueSharingCalculationType).ToString();
                    ddl.Enabled = !_isReadOnly;

                    TextBox totalRevenueSharingTextBox = (TextBox)e.Item.FindControl("TotalRevenueSharingTextBox");
                    totalRevenueSharingTextBox.Text = (vq.CurrentBps + vq.CurrentServiceFee + vq.OtherRevenueSharing).ToString("N5");

                    TextBox currentBpsTextBox = (TextBox)e.Item.FindControl("CurrentBpsTextBox");
                    TextBox currentServiceFeeTextBox = (TextBox)e.Item.FindControl("CurrentServiceFeeTextBox");
                    TextBox currentFindersFeeTextBox = (TextBox)e.Item.FindControl("CurrentFindersFeeTextBox");
                    TextBox otherRevenueSharingTextBox = (TextBox)e.Item.FindControl("OtherRevenueSharingTextBox");
                    TextBox txtCurrentPerHeadSubta = (TextBox)e.Item.FindControl("txtCurrentPerHeadSubta");
                    TextBox txtAssetBasedAdministrativeFee = (TextBox)e.Item.FindControl("txtAssetBasedAdministrativeFee");

                    TextBox[] textboxes = new TextBox[] { currentBpsTextBox, currentServiceFeeTextBox, currentFindersFeeTextBox,
                                                        otherRevenueSharingTextBox, txtCurrentPerHeadSubta, txtAssetBasedAdministrativeFee };

                    (e.Item.FindControl("hfId") as HiddenField).Value = vq.Id.ToString();

                    if (DisableAll)
                    {
                        ddl.Enabled = false;
                        totalRevenueSharingTextBox.Enabled = false;
                        currentBpsTextBox.Enabled = false;
                        currentServiceFeeTextBox.Enabled = false;
                        currentFindersFeeTextBox.Enabled = false;
                        otherRevenueSharingTextBox.Enabled = false;
                        txtCurrentPerHeadSubta.Enabled = false;
                        txtAssetBasedAdministrativeFee.Enabled = false;
                        ((TextBox)e.Item.FindControl("VendorCommentTextBox")).Enabled = false;
                    }

                    // add client-side onchange event handlers
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (TextBox textbox in textboxes)
                    {
                        sb.Clear();
                        sb.Append("RevenueSharingControl_OnChanged(");
                        sb.Append("\"" + totalRevenueSharingTextBox.ClientID + "\",");
                        sb.Append("\"" + currentBpsTextBox.ClientID + "\",");
                        sb.Append("\"" + currentServiceFeeTextBox.ClientID + "\",");
                        sb.Append("\"" + currentFindersFeeTextBox.ClientID + "\",");
                        sb.Append("\"" + otherRevenueSharingTextBox.ClientID + "\");");
                        textbox.Attributes.Add("onchange", sb.ToString());
                    }

                    break;
            }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveRevenueSharing();
        }

        protected void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            SaveRevenueSharing();
            if (Closeable) Page.ClientScript.RegisterStartupScript(this.GetType(), "windowClose", "parent.CloseIFrame();", true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to save the Investment Assumptions
        /// </summary>
        private void SaveRevenueSharing()
        {
            if (IsPageValid)
            {
                List<ServiceModel.IAccountQuestion> aql = GetFields();
                if (aql.Count > 0)
                    SaveRequest(this, new SaveRequestEventArgs(aql));
                SetStatusLabel(Statuses.SaveSucceeded);
            }
        }

        internal bool IsPageValid
        {
            get
            {
                Page.Validate("RevenueSharing");
                return Page.IsValid;
            }
        }

        internal List<ServiceModel.IAccountQuestion> GetFields()
        {
            List<ServiceModel.IAccountQuestion> dataSource = InvestmentAssumptions;//(List<ServiceModel.IAccountQuestion>)rptQuestions.DataSource;

            for (int i = 0; i < rptQuestions.Items.Count; i++)
            {
                Guid id = new Guid();
                if (!Guid.TryParse((rptQuestions.Items[i].FindControl("hfId") as HiddenField).Value, out id)) continue;
                ServiceModel.VendorQuestion vq = (ServiceModel.VendorQuestion)dataSource.Where(a => a.Id == id).FirstOrDefault();
                if (vq == null) continue;

                RepeaterItem ri = rptQuestions.Items[i];

                Decimal pct = 0m;
                TextBox tbx = (TextBox)ri.FindControl("CurrentBpsTextBox");
                vq.CurrentBps = Decimal.TryParse(tbx.Text.Trim(), out pct) ? pct : 0m;

                tbx = (TextBox)ri.FindControl("CurrentServiceFeeTextBox");
                vq.CurrentServiceFee = Decimal.TryParse(tbx.Text.Trim(), out pct) ? pct : 0m;

                tbx = (TextBox)ri.FindControl("CurrentFindersFeeTextBox");
                vq.CurrentFindersFee = Decimal.TryParse(tbx.Text.Trim(), out pct) ? pct : 0m;

                tbx = (TextBox)ri.FindControl("OtherRevenueSharingTextBox");
                vq.OtherRevenueSharing = Decimal.TryParse(tbx.Text.Trim(), out pct) ? pct : 0m;

                tbx = (TextBox)ri.FindControl("VendorCommentTextBox");
                vq.RevenueSharingVendorComment = tbx.Text.Trim();

                tbx = (TextBox)ri.FindControl("TotalRevenueSharingTextBox");
                vq.TotalRevenueSharing = (vq.CurrentBps + vq.CurrentServiceFee + vq.OtherRevenueSharing);// Decimal.TryParse(tbx.Text.Trim(), out pct) ? pct : 0m;

                tbx = (TextBox)ri.FindControl("txtCurrentPerHeadSubta");
                vq.CurrentPerHeadSubta = Decimal.TryParse(tbx.Text.Trim(), out pct) ? pct : 0m;

                tbx = (TextBox)ri.FindControl("txtAssetBasedAdministrativeFee");
                vq.AssetBasedAdministrativeFee = Decimal.TryParse(tbx.Text.Trim(), out pct) ? pct : 0m;

                DropDownList ddl = (DropDownList)ri.FindControl("CalculationTypeDropDownList");
                if (ddl.SelectedIndex == -1)
                    vq.RevenueSharingCalculationType = Data.Enums.RevenueSharingCalculationTypes.BasisPoints;
                else
                    vq.RevenueSharingCalculationType = (Data.Enums.RevenueSharingCalculationTypes)Convert.ToInt32(ddl.SelectedValue);
            }

            return dataSource;
        }

        private void MakeFieldsReadOnly()
        {

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