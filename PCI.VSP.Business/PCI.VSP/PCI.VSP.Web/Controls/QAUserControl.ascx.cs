using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PCI.VSP.Services.Model;
using PCI.VSP.Web.Enums;
using PCI.VSP.Data.Enums;
using System.Drawing;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;

namespace PCI.VSP.Web.Controls
{
    /// <summary>
    /// User control to display questions (Vendor or Client Questions in IAccountQuestion format)
    /// </summary>
    public partial class QAUserControl : System.Web.UI.UserControl
    {
        #region Constants

        private const int MaxLength = 255;
        private const string singleValueOutOfRangeMessage = "Question #{0}'s answer must be within {1} and {2} inclusive.";
        private const string rangeOutOfRangeMessage = "Question #{0}'s {1} value must be within {2} and {3} inclusive.";
        private const string choiceOutOfRangeMessage = "Question #{0}'s answers must be within {1} and {2} inclusive.";
        private const string invalidFileTypeMessage = "Question #{0}'s specified file is not an allowed file type.  The allowed file type(s): {1}.";
        private const string incompleteFeeTierMessage = "Fee #{0}'s tiers must be populated. Populate all tiers and ensure the Fee enter is a proper {1} value.";
        private const string validationGroup = "Questions";

        #endregion

        #region Private Properties

        /// <summary>
        /// List of IAccountQuestions
        /// </summary>
        private List<IAccountQuestion> _questions;

        /// <summary>
        /// The person that is viewing the contents of the control
        /// </summary>
        private UserType _userType = UserType.Unspecified;

        private UserType _viewAsType = UserType.Unspecified;

        /// <summary>
        /// Whether or not all questions have confirmed answers - TRUE by default
        /// </summary>
        private bool _allAnswersConfirmed = true;

        /// <summary>
        /// Whether or not the usercontrol is within a page that can be closed with CloseIFrame JavaScript method
        /// </summary>
        private bool _closeable = false;

        /// <summary>
        /// List of file types a user can upload
        /// </summary>
        private List<string> _allowedFileTypes;

        /// <summary>
        /// Whether or not to disable all questions, because of confirmed answers or closed project
        /// </summary>
        private bool _disableAll = false;

        #endregion

        #region Public Properties

        /// <summary>
        /// List of IAccountQuestions
        /// </summary>
        public List<IAccountQuestion> Questions
        {
            get
            {
                return (_questions == null ? new List<IAccountQuestion>() : _questions);
            }
            set
            {
                _questions = value;
            }
        }

        /// <summary>
        /// The person that is viewing the contents of the control
        /// </summary>
        public UserType UserType
        {
            get
            {
                return _userType;
            }
            set
            {
                _userType = value;
            }
        }

        /// <summary>
        /// How the control should treat the person viewing it, will use the UserType property's value if not set
        /// </summary>
        public UserType ViewAsType
        {
            get
            {
                if (_viewAsType == UserType.Unspecified) 
                    _viewAsType = UserType;

                return _viewAsType;
            }
            set
            {
                _viewAsType = value;
            }
        }

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

        public delegate void SaveRequestEventHandler(QAUserControl sender, SaveRequestEventArgs srea);
        public event SaveRequestEventHandler SaveRequest;

        /// <summary>
        /// List of file types a user can upload
        /// </summary>
        public List<string> AllowedFileTypes
        {
            get
            {
                return _allowedFileTypes;
            }
            set
            {
                _allowedFileTypes = value;
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

        protected void Page_Load(object sender, EventArgs e) { }

        public override void DataBind()
        {
            try
            {
                base.DataBind();
                rptQuestions.DataSource = _questions;
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
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    IAccountQuestion aq = (IAccountQuestion)e.Item.DataItem;
                    PlaceHolder ph = e.Item.FindControl("ph") as PlaceHolder;
                    (e.Item.FindControl("litNumber") as Literal).Text = (e.Item.ItemIndex + 1).ToString() + ")"; // Question number
                    bool isEnabled = IsQuestionEnabled(aq);
                    PlaceHolder phOther = e.Item.FindControl("phOther") as PlaceHolder; // Placeholder for file upload and comments
                    (e.Item.FindControl("hfId") as HiddenField).Value = aq.Id.ToString();

                    #region Create Comment Buttons

                    switch (ViewAsType)
                    {
                        case UserType.PCI:
                            if (aq is ClientQuestion)
                            {
                                phOther.Controls.Add(CreateCommentButton("clientCommentToPCI", "Client Comment To PCI", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.ClientToPci).ToString(), UserType, !string.IsNullOrEmpty(aq.CommentToPCI)));
                                phOther.Controls.Add(CreateCommentButton("clientCommentToVendor", "Client Comment To Vendor", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.ClientToVendor).ToString(), UserType, !string.IsNullOrEmpty(((ClientQuestion)aq).ClientCommentToVendor)));
                                phOther.Controls.Add(CreateCommentButton("pciCommentToClient", "PCI Comment To Client", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.PciToClient).ToString(), UserType, !string.IsNullOrEmpty(aq.CommentFromPCI)));
                            }
                            else if (aq is VendorQuestion)
                            {
                                phOther.Controls.Add(CreateCommentButton("vendorCommentToPCI", "Vendor Comment To PCI", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.VendorToPci).ToString(), UserType, !string.IsNullOrEmpty(aq.CommentToPCI)));
                                phOther.Controls.Add(CreateCommentButton("vendorCommentToClient", "Vendor Comment To Client", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.VendorToClient).ToString(), UserType, !string.IsNullOrEmpty(((VendorQuestion)aq).VendorCommentToClient)));
                                phOther.Controls.Add(CreateCommentButton("pciCommentToVendor", "PCI Comment To Vendor", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.PciToVendor).ToString(), UserType, !string.IsNullOrEmpty(aq.CommentFromPCI)));
                                phOther.Controls.Add(CreateCommentButton("clientCommentToVendor", "Client Comment To Vendor", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.ClientToVendor).ToString(), UserType, !string.IsNullOrEmpty(((VendorQuestion)aq).ClientCommentToVendor)));
                            }
                            break;
                        case UserType.Client:
                            phOther.Controls.Add(CreateCommentButton("clientCommentToPCI", "Client Comment To PCI", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.ClientToPci).ToString(), UserType, !string.IsNullOrEmpty(aq.CommentToPCI)));
                            phOther.Controls.Add(CreateCommentButton("clientCommentToVendor", "Client Comment To Vendor", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.ClientToVendor).ToString(), UserType, !string.IsNullOrEmpty(((ClientQuestion)aq).ClientCommentToVendor)));

                            if (aq is ClientQuestion && !string.IsNullOrEmpty((aq as ClientQuestion).PCICommentToClient))
                                phOther.Controls.Add(CreateCommentButton("pciCommentToClient", "PCI Comment To Client", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.PciToClient).ToString(), UserType, !string.IsNullOrEmpty(aq.CommentFromPCI)));

                            break;
                        case UserType.Vendor:
                            if (!(aq is ClientQuestion))
                            {
                                phOther.Controls.Add(CreateCommentButton("vendorCommentToPCI", "Vendor Comment To PCI", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.VendorToPci).ToString(), UserType, !string.IsNullOrEmpty(aq.CommentToPCI)));
                                phOther.Controls.Add(CreateCommentButton("vendorCommentToClient", "Vendor Comment To Client", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.VendorToClient).ToString(), UserType, !string.IsNullOrEmpty(((VendorQuestion)aq).VendorCommentToClient)));
                            }

                            if (aq is VendorQuestion)
                            {
                                if (!string.IsNullOrEmpty((aq as VendorQuestion).PCICommentToVendor))
                                    phOther.Controls.Add(CreateCommentButton("pciCommentToVendor", "PCI Comment To Vendor", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.PciToVendor).ToString(), UserType, true));
                                if (!string.IsNullOrEmpty((aq as VendorQuestion).ClientCommentToVendor))
                                    phOther.Controls.Add(CreateCommentButton("clientCommentToVendor", "Client Comment To Vendor", aq.Id.ToString(), Convert.ToInt32(Enums.CommentTypes.ClientToVendor).ToString(), UserType, true));
                            }

                            break;
                    }

                    #endregion

                    #region Uploaded File History

                    if (aq.Notes.Count > 0)
                    {
                        HtmlGenericControl divFiles = new HtmlGenericControl("div") { ID = "divFiles_" + aq.Id };
                        divFiles.Attributes.Add("class", "outerDiv divFiles_" + aq.Id);

                        HtmlGenericControl divBlackOut = new HtmlGenericControl("div");
                        divBlackOut.Attributes.Add("class", "blackOut2");
                        divFiles.Controls.Add(divBlackOut);

                        HtmlGenericControl divInner = new HtmlGenericControl("div");
                        divInner.Attributes.Add("class", "innerDiv ModalWindow2");

                        HtmlGenericControl table = new HtmlGenericControl("table");
                        table.Style.Add("width", "100%");
                        HtmlGenericControl headerRow = new HtmlGenericControl("tr");
                        HtmlGenericControl headerCell1 = new HtmlGenericControl("td") { InnerText = "File Name" };
                        HtmlGenericControl headerCell2 = new HtmlGenericControl("td") { InnerText = "Created Date" };

                        headerRow.Controls.Add(headerCell1);
                        headerRow.Controls.Add(headerCell2);
                        table.Controls.Add(headerRow);

                        foreach (var note in aq.Notes)
                        {
                            HtmlGenericControl tr = new HtmlGenericControl("tr");
                            HtmlGenericControl td1 = new HtmlGenericControl("td");
                            HtmlGenericControl td2 = new HtmlGenericControl("td") { InnerText = note.CreatedOn.Value.ToString() };

                            HyperLink hl = new HyperLink() { ID = "note_" + note.Id, Target = "_blank", Text = note.FileName, NavigateUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/Vendor/DownloadNote.ashx?id=" + note.Id };
                            td1.Controls.Add(hl);

                            tr.Controls.Add(td1);
                            tr.Controls.Add(td2);
                            table.Controls.Add(tr);
                        }
                        divInner.Controls.Add(table);

                        HtmlButton hbClose = new HtmlButton() { ID = "hideFiles_" + aq.Id, InnerText = "Close" };
                        hbClose.Attributes.Add("type", "button");
                        hbClose.Attributes.Add("onclick", "$('." + divFiles.ID + "').hide();");
                        divInner.Controls.Add(hbClose);

                        divFiles.Controls.Add(divInner);

                        HtmlButton hb = new HtmlButton() { ID = "showFiles_" + aq.Id };
                        hb.Attributes.Add("type", "button");
                        hb.Attributes.Add("onclick", "$('." + divFiles.ID + "').show();");
                        HtmlImage htmlImage = new HtmlImage() { Alt = "Uploaded Files" };
                        htmlImage.Attributes.Add("title", "Uploaded Files");
                        htmlImage.Src = "~/images/files.png";
                        hb.Controls.Add(htmlImage);

                        phOther.Controls.Add(hb);
                        phOther.Controls.Add(divFiles);
                    }

                    #endregion

                    #region Document Template Download

                    if (isEnabled && aq.DocumentTemplate != null && aq.DocumentTemplate.Id != Guid.Empty)
                    {
                        HtmlButton hb = new HtmlButton() { ID = "documenttemplate_" + aq.Id };
                        hb.Attributes.Add("type", "button");
                        HtmlImage htmlImage = new HtmlImage() { Alt = "Document Template" };
                        htmlImage.Attributes.Add("title", "Document Template");
                        htmlImage.Src = "~/images/documenttemplate.png";
                        hb.Controls.Add(htmlImage);
                        hb.Attributes.Add("onclick", "window.open('" + Request.Url.Scheme + "://" + Request.Url.Authority + "/Vendor/DownloadNote.ashx?id=" + aq.DocumentTemplate.Id + "');");
                        phOther.Controls.Add(hb);
                    }

                    #endregion

                    #region File Upload

                    if (isEnabled && AllowedFileTypes != null && AllowedFileTypes.Count > 0 && ViewAsType == UserType.Vendor)
                    {
                        StringBuilder sbMsgFileType = new StringBuilder();
                        StringBuilder sbRegex = new StringBuilder();

                        foreach (var s in AllowedFileTypes)
                        {
                            sbMsgFileType.Append(",").Append(s.ToUpper());
                            sbRegex.Append("|").Append(s.ToLower()).Append("|").Append(s.ToUpper());
                        }

                        if (sbMsgFileType.Length > 0) sbMsgFileType = sbMsgFileType.Remove(0, 1);
                        if (sbRegex.Length > 0) sbRegex = sbRegex.Remove(0, 1);

                        FileUpload fu = new FileUpload() { ID = "fileUpload" };
                        fu.Attributes.Add("runat", "server");
                        phOther.Controls.Add(fu);
                        phOther.Controls.Add(new RegularExpressionValidator() { ControlToValidate = fu.ID, ValidationGroup = validationGroup, ErrorMessage = String.Format(invalidFileTypeMessage, (e.Item.ItemIndex + 1), sbMsgFileType.ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red, ValidationExpression = @"^.*\.(" + sbRegex.ToString() + ")$" });
                    }

                    #endregion

                    #region Invalid Answer Code

                    Literal LiteralError = e.Item.FindControl("LiteralError") as Literal;

                    switch ((InvalidAnswerReasons)aq.InvalidAnswerReason)
                    {
                        case InvalidAnswerReasons.Expired:
                            LiteralError.Text = "<font color='red';>Answer Expired</font>";
                            break;
                        case InvalidAnswerReasons.Invalid:
                            LiteralError.Text = "<font color='red';>Answer Invalid</font>";
                            break;
                        case InvalidAnswerReasons.WordingChange:
                            LiteralError.Text = "<font color='red';>Answer Wording Change</font>";
                            break;
                    }

                    if (!string.IsNullOrEmpty(aq.AnswerRejectedReason)) LiteralError.Text += "<font color='red';> - " + aq.AnswerRejectedReason + "</font>";

                    #endregion

                    switch ((DataTypes)aq.QuestionDataType)
                    {
                        #region Integer / Text

                        case DataTypes.Integer:
                        case DataTypes.Text:

                            int intMin = 0, intMax = 0;
                            if (!int.TryParse(aq.MinimumAnswerAllowed, out intMin)) intMin = int.MinValue;
                            if (!int.TryParse(aq.MaximumAnswerAllowed, out intMax)) intMax = int.MaxValue;
                            switch ((AnswerTypes)aq.AnswerType)
                            {
                                case AnswerTypes.SingleValue:
                                    var tb = new TextBox() { ID = "tb" + ((DataTypes)aq.QuestionDataType).ToString(), Text = aq.Answer, CssClass = ((DataTypes)aq.QuestionDataType).ToString(), CausesValidation = true, MaxLength = MaxLength };
                                    tb.Enabled = isEnabled;
                                    ph.Controls.Add(tb);

                                    if ((DataTypes)aq.QuestionDataType == DataTypes.Integer)
                                        ph.Controls.Add(new RangeValidator() { Type = ValidationDataType.Integer, MinimumValue = intMin.ToString(), MaximumValue = intMax.ToString(), ControlToValidate = tb.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(singleValueOutOfRangeMessage, (e.Item.ItemIndex + 1), intMin.ToString(), intMax.ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });

                                    break;
                                case AnswerTypes.MultiValue:
                                    TextBox mvtb = new TextBox() { ID = "tb" + ((DataTypes)aq.QuestionDataType).ToString(), Text = aq.Answer, CssClass = ((DataTypes)aq.QuestionDataType).ToString(), TextMode = TextBoxMode.MultiLine, Rows = 4, MaxLength = MaxLength };
                                    mvtb.Enabled = isEnabled;
                                    ph.Controls.Add(mvtb);

                                    if ((DataTypes)aq.QuestionDataType == DataTypes.Integer)
                                    {
                                        var cv = new CustomValidator() { ValidateEmptyText = false, ClientValidationFunction = "MultiLineInteger_Validate", ControlToValidate = mvtb.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(choiceOutOfRangeMessage, (e.Item.ItemIndex + 1), intMin.ToString(), intMax.ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red };
                                        cv.Attributes.Add("minValue", intMin.ToString());
                                        cv.Attributes.Add("maxValue", intMax.ToString());
                                        cv.ServerValidate += new ServerValidateEventHandler(MultiLineInteger_Validate);
                                        ph.Controls.Add(cv);
                                    }

                                    break;
                                case AnswerTypes.Range:
                                    ph.Controls.Add(new Label() { Text = "Minimum: " });
                                    TextBox tbr1 = new TextBox() { ID = "tb" + ((DataTypes)aq.QuestionDataType).ToString(), Text = aq.Answer, CssClass = ((DataTypes)aq.QuestionDataType).ToString(), MaxLength = MaxLength };
                                    tbr1.Enabled = isEnabled;
                                    ph.Controls.Add(tbr1);

                                    ph.Controls.Add(new Label() { Text = "Maximum: " });
                                    TextBox tbr2 = new TextBox() { ID = "tbalt" + ((DataTypes)aq.QuestionDataType).ToString(), Text = aq.AlternateAnswer, CssClass = ((DataTypes)aq.QuestionDataType).ToString(), MaxLength = MaxLength };
                                    tbr2.Enabled = isEnabled;
                                    ph.Controls.Add(tbr2);

                                    if ((DataTypes)aq.QuestionDataType == DataTypes.Integer)
                                    {
                                        ph.Controls.Add(new CompareValidator() { ControlToValidate = tbr2.ID, ControlToCompare = tbr1.ID, Operator = ValidationCompareOperator.GreaterThanEqual, Type = ValidationDataType.Integer, ErrorMessage = "Question #" + (e.Item.ItemIndex + 1) + " contains an invalid integer range.", Text = "*", ValidationGroup = validationGroup, EnableClientScript = true, Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });
                                        ph.Controls.AddAt(2, new RangeValidator() { Type = ValidationDataType.Integer, MinimumValue = intMin.ToString(), MaximumValue = intMax.ToString(), ControlToValidate = tbr1.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(rangeOutOfRangeMessage, (e.Item.ItemIndex + 1), "minimum", intMin.ToString(), intMax.ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });
                                        ph.Controls.Add(new RangeValidator() { Type = ValidationDataType.Integer, MinimumValue = intMin.ToString(), MaximumValue = intMax.ToString(), ControlToValidate = tbr2.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(rangeOutOfRangeMessage, (e.Item.ItemIndex + 1), "maximum", intMin.ToString(), intMax.ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });
                                    }
                                    break;
                                case AnswerTypes.TieredFee:
                                    if ((DataTypes)aq.QuestionDataType == DataTypes.Integer)
                                    {
                                        HtmlGenericControl FeeTierDiv = new HtmlGenericControl("div") { ID = "FeeTierParent" };
                                        FeeTierDiv.Attributes.Add("class", "FeeTierParent");

                                        TextBox tbInteger = new TextBox() { ID = "tb" + ((DataTypes)aq.QuestionDataType).ToString(), CssClass = "FeeTierTotals", Text = aq.ChoiceAnswers, TextMode = TextBoxMode.MultiLine };
                                        FeeTierDiv.Controls.Add(tbInteger);

                                        var cvInteger = new CustomValidator() { ValidateEmptyText = false, ClientValidationFunction = "FeeTierInt_Validate", ControlToValidate = tbInteger.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(incompleteFeeTierMessage, (e.Item.ItemIndex + 1), ((DataTypes)aq.QuestionDataType).ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red };
                                        cvInteger.ServerValidate += new ServerValidateEventHandler(FeeTierInt_Validate);
                                        ph.Controls.Add(cvInteger);

                                        HtmlButton btnAddTier = new HtmlButton() { ID = "btn" + ((DataTypes)aq.QuestionDataType).ToString(), InnerText = "Add Tier" };
                                        btnAddTier.Attributes.Add("onclick", "addFeeTier(this);");
                                        btnAddTier.Attributes.Add("type", "button");
                                        btnAddTier.Attributes.Add("class", "AddTierButton");
                                        if (!isEnabled)
                                            btnAddTier.Attributes.Add("disabled", "disabled");

                                        FeeTierDiv.Controls.Add(btnAddTier);
                                        ph.Controls.Add(FeeTierDiv);
                                    }
                                    break;
                            }
                            break;

                        #endregion

                        #region Double

                        case DataTypes.Double:
                            double dbMin = 0.0, dbMax = 0.0;
                            if (!double.TryParse(aq.MinimumAnswerAllowed, out dbMin)) dbMin = int.MinValue;
                            if (!double.TryParse(aq.MaximumAnswerAllowed, out dbMax)) dbMax = int.MaxValue;
                            switch ((AnswerTypes)aq.AnswerType)
                            {
                                case AnswerTypes.SingleValue:
                                    var tb = new TextBox() { ID = "tb" + ((DataTypes)aq.QuestionDataType).ToString(), Text = aq.Answer, CssClass = ((DataTypes)aq.QuestionDataType).ToString(), CausesValidation = true, MaxLength = MaxLength };
                                    tb.Enabled = isEnabled;
                                    ph.Controls.Add(tb);
                                    ph.Controls.Add(new RangeValidator() { Type = ValidationDataType.Double, MinimumValue = dbMin.ToString(), MaximumValue = dbMax.ToString(), ControlToValidate = tb.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(singleValueOutOfRangeMessage, (e.Item.ItemIndex + 1), dbMin.ToString(), dbMax.ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });

                                    break;
                                case AnswerTypes.MultiValue:
                                    TextBox mvtb = new TextBox() { ID = "tb" + ((DataTypes)aq.QuestionDataType).ToString(), Text = aq.Answer, CssClass = ((DataTypes)aq.QuestionDataType).ToString(), TextMode = TextBoxMode.MultiLine, Rows = 4, MaxLength = MaxLength };
                                    mvtb.Enabled = isEnabled;
                                    ph.Controls.Add(mvtb);

                                    if ((DataTypes)aq.QuestionDataType == DataTypes.Integer)
                                    {
                                        var cv = new CustomValidator() { ValidateEmptyText = false, ClientValidationFunction = "MultiLineDouble_Validate", ControlToValidate = mvtb.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(choiceOutOfRangeMessage, (e.Item.ItemIndex + 1), dbMin.ToString(), dbMax.ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red };
                                        cv.Attributes.Add("minValue", dbMin.ToString());
                                        cv.Attributes.Add("maxValue", dbMax.ToString());
                                        cv.ServerValidate += new ServerValidateEventHandler(MultiLineDouble_Validate);
                                        ph.Controls.Add(cv);
                                    }

                                    break;
                                case AnswerTypes.Range:
                                    ph.Controls.Add(new Label() { Text = "Minimum: " });
                                    TextBox tbr1 = new TextBox() { ID = "tb" + ((DataTypes)aq.QuestionDataType).ToString(), Text = aq.Answer, CssClass = ((DataTypes)aq.QuestionDataType).ToString(), MaxLength = MaxLength };
                                    tbr1.Enabled = isEnabled;
                                    ph.Controls.Add(tbr1);

                                    ph.Controls.Add(new Label() { Text = "Maximum: " });
                                    TextBox tbr2 = new TextBox() { ID = "tbalt" + ((DataTypes)aq.QuestionDataType).ToString(), Text = aq.AlternateAnswer, CssClass = ((DataTypes)aq.QuestionDataType).ToString(), MaxLength = MaxLength };
                                    tbr2.Enabled = isEnabled;
                                    ph.Controls.Add(tbr2);
                                    ph.Controls.Add(new CompareValidator() { ControlToValidate = tbr2.ID, ControlToCompare = tbr1.ID, Operator = ValidationCompareOperator.GreaterThanEqual, Type = ValidationDataType.Double, ErrorMessage = "Question #" + (e.Item.ItemIndex + 1) + " contains an invalid double range.", Text = "*", ValidationGroup = validationGroup, EnableClientScript = true, Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });
                                    ph.Controls.AddAt(2, new RangeValidator() { Type = ValidationDataType.Double, MinimumValue = dbMin.ToString(), MaximumValue = dbMax.ToString(), ControlToValidate = tbr1.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(rangeOutOfRangeMessage, (e.Item.ItemIndex + 1), "minimum", dbMin.ToString(), dbMax.ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });
                                    ph.Controls.Add(new RangeValidator() { Type = ValidationDataType.Double, MinimumValue = dbMin.ToString(), MaximumValue = dbMax.ToString(), ControlToValidate = tbr2.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(rangeOutOfRangeMessage, (e.Item.ItemIndex + 1), "maximum", dbMin.ToString(), dbMax.ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });

                                    break;
                                case AnswerTypes.TieredFee:
                                    HtmlGenericControl FeeTierDiv = new HtmlGenericControl("div") { ID = "FeeTierParent" };
                                    FeeTierDiv.Attributes.Add("class", "FeeTierParent");

                                    TextBox tbDouble = new TextBox() { ID = "tb" + ((DataTypes)aq.QuestionDataType).ToString(), CssClass = "FeeTierTotals", Text = aq.ChoiceAnswers, TextMode = TextBoxMode.MultiLine };
                                    FeeTierDiv.Controls.Add(tbDouble);

                                    var cvDouble = new CustomValidator() { ValidateEmptyText = false, ClientValidationFunction = "FeeTierDouble_Validate", ControlToValidate = tbDouble.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(incompleteFeeTierMessage, (e.Item.ItemIndex + 1), ((DataTypes)aq.QuestionDataType).ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red };
                                    cvDouble.ServerValidate += new ServerValidateEventHandler(FeeTierDouble_Validate);
                                    ph.Controls.Add(cvDouble);

                                    HtmlButton btnAddTier = new HtmlButton() { ID = "btn" + ((DataTypes)aq.QuestionDataType).ToString(), InnerText = "Add Tier" };
                                    btnAddTier.Attributes.Add("onclick", "addFeeTier(this);");
                                    btnAddTier.Attributes.Add("type", "button");
                                    btnAddTier.Attributes.Add("class", "AddTierButton");
                                
                                    FeeTierDiv.Controls.Add(btnAddTier);
                                    ph.Controls.Add(FeeTierDiv);
                                break;
                            }
                            break;

                        #endregion

                        #region Money

                        case DataTypes.Money:

                            decimal moneyMin = 0, moneyMax = 0;
                            if (!decimal.TryParse(aq.MinimumAnswerAllowed, out moneyMin)) moneyMin = decimal.MinValue;
                            if (!decimal.TryParse(aq.MaximumAnswerAllowed, out moneyMax)) moneyMax = decimal.MaxValue;

                            if ((AnswerTypes)aq.AnswerType == AnswerTypes.SingleValue)
                            {
                                ph.Controls.Add(new Label() { Text = "$" });
                                var tb = new TextBox() { ID = "tb" + ((DataTypes)aq.QuestionDataType).ToString(), Text = aq.Answer, CssClass = ((DataTypes)aq.QuestionDataType).ToString(), CausesValidation = true, MaxLength = MaxLength };
                                tb.Enabled = isEnabled;
                                ph.Controls.Add(tb);
                                ph.Controls.Add(new RangeValidator() { Type = ValidationDataType.Currency, MinimumValue = moneyMin.ToString(), MaximumValue = moneyMax.ToString(), ControlToValidate = tb.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(singleValueOutOfRangeMessage, (e.Item.ItemIndex + 1), moneyMin.ToString(), moneyMax.ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });
                            }
                            else if ((AnswerTypes)aq.AnswerType == AnswerTypes.Range)
                            {
                                ph.Controls.Add(new Label() { Text = "Minimum: " });
                                ph.Controls.Add(new Label() { Text = "$" });
                                TextBox tbr1 = new TextBox() { ID = "tb" + ((DataTypes)aq.QuestionDataType).ToString(), Text = aq.Answer, CssClass = ((DataTypes)aq.QuestionDataType).ToString(), MaxLength = MaxLength };
                                tbr1.Enabled = isEnabled;
                                ph.Controls.Add(tbr1);
                                ph.Controls.Add(new RangeValidator() { Type = ValidationDataType.Currency, MinimumValue = moneyMin.ToString(), MaximumValue = moneyMax.ToString(), ControlToValidate = tbr1.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(rangeOutOfRangeMessage, (e.Item.ItemIndex + 1), "minimum", moneyMin.ToString(), moneyMax.ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });

                                ph.Controls.Add(new Label() { Text = "Maximum: " });
                                ph.Controls.Add(new Label() { Text = "$" });
                                TextBox tbr2 = new TextBox() { ID = "tbalt" + ((DataTypes)aq.QuestionDataType).ToString(), Text = aq.AlternateAnswer, CssClass = ((DataTypes)aq.QuestionDataType).ToString(), MaxLength = MaxLength };
                                tbr2.Enabled = isEnabled;
                                ph.Controls.Add(tbr2);
                                ph.Controls.Add(new RangeValidator() { Type = ValidationDataType.Currency, MinimumValue = moneyMin.ToString(), MaximumValue = moneyMax.ToString(), ControlToValidate = tbr2.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(rangeOutOfRangeMessage, (e.Item.ItemIndex + 1), "maximum", moneyMin.ToString(), moneyMax.ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });
                                ph.Controls.Add(new CompareValidator() { ControlToValidate = tbr2.ID, ControlToCompare = tbr1.ID, Operator = ValidationCompareOperator.GreaterThanEqual, Type = ValidationDataType.Currency, ErrorMessage = "Question #" + (e.Item.ItemIndex + 1) + " contains an invalid money range.", Text = "*", ValidationGroup = validationGroup, EnableClientScript = true, Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });
                            }
                            else if ((AnswerTypes)aq.AnswerType == AnswerTypes.TieredFee)
                            {
                                HtmlGenericControl FeeTierDiv = new HtmlGenericControl("div") { ID = "FeeTierParent" };
                                FeeTierDiv.Attributes.Add("class", "FeeTierParent");

                                TextBox tb = new TextBox() { ID = "tb" + ((DataTypes)aq.QuestionDataType).ToString(), CssClass = "FeeTierTotals", Text = aq.ChoiceAnswers, TextMode = TextBoxMode.MultiLine };
                                FeeTierDiv.Controls.Add(tb);

                                var cv = new CustomValidator() { ValidateEmptyText = false, ClientValidationFunction = "FeeTierMoney_Validate", ControlToValidate = tb.ID, ValidationGroup = validationGroup, ErrorMessage = string.Format(incompleteFeeTierMessage, (e.Item.ItemIndex + 1), ((DataTypes)aq.QuestionDataType).ToString()), EnableClientScript = true, Text = "*", Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red };
                                cv.ServerValidate += new ServerValidateEventHandler(FeeTierMoney_Validate);
                                ph.Controls.Add(cv);

                                HtmlButton btnAddTier = new HtmlButton() { ID = "btn" + ((DataTypes)aq.QuestionDataType).ToString(), InnerText = "Add Tier" };
                                btnAddTier.Attributes.Add("onclick", "addFeeTier(this);");
                                btnAddTier.Attributes.Add("type", "button");
                                btnAddTier.Attributes.Add("class", "AddTierButton");
                                
                                FeeTierDiv.Controls.Add(btnAddTier);
                                ph.Controls.Add(FeeTierDiv);
                            }
                            break;

                        #endregion

                        #region Yes_No

                        case DataTypes.Yes_No:
                            DropDownList ddl = new DropDownList() { ID = ID = "ddl" + ((DataTypes)aq.QuestionDataType).ToString() };
                            ddl.Items.Add(new ListItem("", ""));
                            ddl.Items.Add(new ListItem("Yes", "Yes"));
                            ddl.Items.Add(new ListItem("No", "No"));
                            ddl.SelectedValue = aq.Answer;
                            ddl.Enabled = isEnabled;
                            ph.Controls.Add(ddl);
                            break;

                        #endregion

                        #region Choice

                        case DataTypes.Choice:

                            if ((AnswerTypes)aq.AnswerType == AnswerTypes.SingleValue)
                            {
                                var choiceddl = new DropDownList() { ID = "ddl" + DataTypes.Choice.ToString() };
                                foreach (var answer in aq.ChoiceAnswers.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                                    choiceddl.Items.Add(new ListItem(answer, answer));

                                choiceddl.SelectedValue = aq.Answer;
                                choiceddl.Enabled = isEnabled;
                                choiceddl.Items.Insert(0, new ListItem("", ""));
                                ph.Controls.Add(choiceddl);
                            }
                            else if ((AnswerTypes)aq.AnswerType == AnswerTypes.MultiValue)
                            {
                                ListBox lb = new ListBox() { ID = "ddl" + DataTypes.Choice.ToString(), SelectionMode = ListSelectionMode.Multiple };
                                foreach (var answer in aq.ChoiceAnswers.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                                    lb.Items.Add(new ListItem(answer, answer));

                                foreach (var selectedanswer in aq.Answer.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var item = lb.Items.FindByValue(selectedanswer);
                                    if (item != null) item.Selected = true;
                                }
                                lb.Enabled = isEnabled;
                                ph.Controls.Add(lb);
                            }

                            break;

                        #endregion

                        #region Date

                        case DataTypes.Date:

                            RadDatePicker date = new RadDatePicker() { ID = "dt" + ((DataTypes)aq.QuestionDataType).ToString(), MinDate = SqlDateTime.MinValue.Value };
                            date.DateInput.DateFormat = "MM/dd/yyyy";
                            date.DateInput.DisplayDateFormat = "MM/dd/yyyy";
                            date.DateInput.ValidationGroup = validationGroup;
                            DateTime dt;
                            if (DateTime.TryParse(aq.Answer, out dt))
                                date.SelectedDate = dt.Date;
                            date.Enabled = isEnabled;
                            ph.Controls.Add(date);

                            if ((AnswerTypes)aq.AnswerType == AnswerTypes.Range)
                            {
                                ph.Controls.AddAt(0, new Label() { Text = "From: " });
                                ph.Controls.Add(new Label() { Text = "To: " });
                                RadDatePicker maxdate = new RadDatePicker() { ID = "dtalt" + ((DataTypes)aq.QuestionDataType).ToString(), MinDate = SqlDateTime.MinValue.Value };
                                maxdate.DateInput.DateFormat = "MM/dd/yyyy";
                                maxdate.DateInput.DisplayDateFormat = "MM/dd/yyyy";
                                maxdate.DateInput.ValidationGroup = validationGroup;
                                if (DateTime.TryParse(aq.AlternateAnswer, out dt))
                                    maxdate.SelectedDate = dt.Date;
                                maxdate.Enabled = isEnabled;
                                ph.Controls.Add(maxdate);
                                ph.Controls.Add(new CompareValidator() { ControlToValidate = maxdate.ID, ControlToCompare = date.ID, Operator = ValidationCompareOperator.GreaterThanEqual, Type = ValidationDataType.Date, ErrorMessage = "Question #" + (e.Item.ItemIndex + 1) + " contains an invalid date range.", Text = "*", ValidationGroup = validationGroup, EnableClientScript = true, Display = ValidatorDisplay.Dynamic, ForeColor = Color.Red });
                            }

                            break;

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Method to validate multi-line integer answers
        /// </summary>
        /// <param name="source">object</param>
        /// <param name="args">valdation event argument</param>
        void MultiLineInteger_Validate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            int intMin = 0, intMax = 0;
            if (!int.TryParse(((WebControl)(source)).Attributes["minValue"], out intMin)) intMin = int.MinValue;
            if (!int.TryParse(((WebControl)(source)).Attributes["maxValue"], out intMax)) intMax = int.MaxValue;

            string[] sa = args.Value.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < sa.Count(); i++)
            {
                int temp = 0;
                if (!int.TryParse(sa[i], out temp) || temp < intMin || temp > intMax) return;
            }

            args.IsValid = true;
        }

        /// <summary>
        /// Method to validate multi-line double answers
        /// </summary>
        /// <param name="source">object</param>
        /// <param name="args">valdation event argument</param>
        void MultiLineDouble_Validate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            double dbMin = 0, dbMax = 0;
            if (!double.TryParse(((WebControl)(source)).Attributes["minValue"], out dbMin)) dbMin = double.MinValue;
            if (!double.TryParse(((WebControl)(source)).Attributes["maxValue"], out dbMax)) dbMax = double.MaxValue;

            string[] sa = args.Value.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < sa.Count(); i++)
            {
                double temp = 0;
                if (!double.TryParse(sa[i], out temp) || temp < dbMin || temp > dbMax) return;
            }

            args.IsValid = true;
        }

        void FeeTierMoney_Validate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            string[] feeTiers = args.Value.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < feeTiers.Count(); i++)
            {
                string[] feeTier = feeTiers[i].Split('|');
                for (int j = 0; j < feeTier.Count(); j++)
                {
                    if (string.IsNullOrEmpty(feeTier[j])) return;

                    if (j == 0 || j == 1) // Minimum || Maximum Value
                    {
                        Regex r = new Regex(@"^[-+]?\d+(\.\d+)?$");
                        if (!r.IsMatch(feeTier[j])) return;
                    }

                    if (j == 2) // Fee Value
                    {
                        Regex r = new Regex(@"^[0-9]+(,[0-9]{3})*(\.[0-9]{1,5})?$");
                        if (!r.IsMatch(feeTier[j])) return;
                    }
                }
            }

            args.IsValid = true;
        }

        void FeeTierInt_Validate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            string[] feeTiers = args.Value.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < feeTiers.Count(); i++)
            {
                string[] feeTier = feeTiers[i].Split('|');
                for (int j = 0; j < feeTier.Count(); j++)
                {
                    if (string.IsNullOrEmpty(feeTier[j])) return;

                    if (j == 0 || j == 1) // Minimum || Maximum Value
                    {
                        Regex r = new Regex(@"^[-+]?\d+(\.\d+)?$");
                        if (!r.IsMatch(feeTier[j])) return;
                    }

                    if (j == 2) // Fee Value
                    {
                        Regex r = new Regex(@"^\s*\d+\s*$");
                        if (!r.IsMatch(feeTier[j])) return;
                    }
                }
            }

            args.IsValid = true;
        }

        void FeeTierDouble_Validate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            string[] feeTiers = args.Value.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < feeTiers.Count(); i++)
            {
                string[] feeTier = feeTiers[i].Split('|');
                for (int j = 0; j < feeTier.Count(); j++)
                {
                    if (string.IsNullOrEmpty(feeTier[j])) return;
                    Regex r = new Regex(@"^[-+]?\d+(\.\d+)?$");
                    if (!r.IsMatch(feeTier[j])) return;
                }
            }

            args.IsValid = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveAnswers();
        }

        protected void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            SaveAnswers();
            if (Closeable) Page.ClientScript.RegisterStartupScript(this.GetType(), "windowClose", "parent.CloseIFrame();", true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns whether or not the question is enabled
        /// </summary>
        /// <param name="question">IAccountQuestion object</param>
        /// <returns>boolean</returns>
        private bool IsQuestionEnabled(IAccountQuestion question)
        {
            bool isEnabled = true;
            if (question.Status == AccountQuestionStatuses.AccountConfirmed || question.Status == AccountQuestionStatuses.PCI_Confirmed || DisableAll) isEnabled = false;
            if (_allAnswersConfirmed) _allAnswersConfirmed = isEnabled;
            if (ViewAsType == UserType.Vendor && (question.QuestionType == QuestionTypes.PlanAssumption)) isEnabled = false;

            return isEnabled;
        }

        /// <summary>
        /// Save the answer(s) to the question(s)
        /// </summary>
        private void SaveAnswers()
        {
            try
            {
                Page.Validate(validationGroup);
                if (!Page.IsValid) return;

                List<IAccountQuestion> aql = new List<IAccountQuestion>();
                string errmsg = string.Empty;
                List<IAccountQuestion> dataSource = (List<IAccountQuestion>)rptQuestions.DataSource;

                for (int i = 0; i < rptQuestions.Items.Count; i++)
                {
                    Guid id = new Guid();
                    if (!Guid.TryParse((rptQuestions.Items[i].FindControl("hfId") as HiddenField).Value, out id)) continue;
                    IAccountQuestion aq = dataSource.Where(a => a.Id == id).FirstOrDefault();
                    if (aq == null) continue;

                    aq.Id = id;
                    var ph = rptQuestions.Items[i].FindControl("ph") as PlaceHolder;
                    
                    switch ((DataTypes)aq.QuestionDataType)
                    {
                        #region Integer / Text / Double

                        case DataTypes.Integer:
                        case DataTypes.Text:
                        case DataTypes.Double:

                            switch ((AnswerTypes)aq.AnswerType)
                            {
                                case AnswerTypes.SingleValue:
                                    aq.Answer = (ph.FindControl("tb" + ((DataTypes)aq.QuestionDataType).ToString()) as TextBox).Text;
                                    break;
                                case AnswerTypes.MultiValue:
                                    string answer = (ph.FindControl("tb" + ((DataTypes)aq.QuestionDataType).ToString()) as TextBox).Text;
                                    string a = string.Empty;
                                    foreach (var s in answer.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                                        a += s + "\n";
                                    aq.Answer = answer.Length > 0 ? a.Remove(a.Length - 1, 1) : string.Empty;
                                    break;
                                case AnswerTypes.Range:
                                    aq.Answer = (ph.FindControl("tb" + ((DataTypes)aq.QuestionDataType).ToString()) as TextBox).Text;
                                    aq.AlternateAnswer = (ph.FindControl("tbalt" + ((DataTypes)aq.QuestionDataType).ToString()) as TextBox).Text;
                                    break;
                                case AnswerTypes.TieredFee:
                                    string itdAnswer = (ph.FindControl("tb" + ((DataTypes)aq.QuestionDataType).ToString()) as TextBox).Text;
                                    if (!string.IsNullOrEmpty(itdAnswer) && itdAnswer.LastIndexOf("\r\n") == (itdAnswer.Length - 2))
                                        itdAnswer = itdAnswer.Remove(itdAnswer.LastIndexOf("\r\n"));
                                    else if (!string.IsNullOrEmpty(itdAnswer) && itdAnswer.LastIndexOf("\n") == (itdAnswer.Length - 1))
                                        itdAnswer = itdAnswer.Remove(itdAnswer.LastIndexOf("\n"));

                                    aq.ChoiceAnswers = itdAnswer;
                                    break;
                            }

                            break;

                        #endregion

                        #region Money

                        case DataTypes.Money:
                            switch ((AnswerTypes)aq.AnswerType)
                            {
                                case AnswerTypes.SingleValue:
                                    aq.Answer = (ph.FindControl("tb" + ((DataTypes)aq.QuestionDataType).ToString()) as TextBox).Text;
                                    break;
                                case AnswerTypes.Range:
                                    aq.Answer = (ph.FindControl("tb" + ((DataTypes)aq.QuestionDataType).ToString()) as TextBox).Text;
                                    aq.AlternateAnswer = (ph.FindControl("tbalt" + ((DataTypes)aq.QuestionDataType).ToString()) as TextBox).Text;
                                    break;
                                case AnswerTypes.TieredFee:
                                    string mAnswer = (ph.FindControl("tb" + ((DataTypes)aq.QuestionDataType).ToString()) as TextBox).Text;
                                    if (!string.IsNullOrEmpty(mAnswer) && mAnswer.LastIndexOf("\r\n") == (mAnswer.Length - 2))
                                        mAnswer = mAnswer.Remove(mAnswer.LastIndexOf("\r\n"));
                                    else if (!string.IsNullOrEmpty(mAnswer) && mAnswer.LastIndexOf("\n") == (mAnswer.Length - 1))
                                        mAnswer = mAnswer.Remove(mAnswer.LastIndexOf("\n"));

                                    aq.ChoiceAnswers = mAnswer;
                                    break;
                            }
                            break;

                        #endregion

                        case DataTypes.Yes_No:
                            aq.Answer = (ph.FindControl("ddl" + DataTypes.Yes_No.ToString()) as DropDownList).SelectedValue;
                            break;

                        #region Choice

                        case DataTypes.Choice:

                            if ((AnswerTypes)aq.AnswerType == AnswerTypes.SingleValue)
                                aq.Answer = (ph.FindControl("ddl" + DataTypes.Choice.ToString()) as DropDownList).SelectedValue;
                            else if ((AnswerTypes)aq.AnswerType == AnswerTypes.MultiValue)
                            {
                                var lb = ph.FindControl("ddl" + DataTypes.Choice.ToString()) as ListBox;
                                string answer = string.Empty;

                                foreach (var index in lb.GetSelectedIndices())
                                    answer += lb.Items[index].Value + "\n";

                                aq.Answer = answer.Length > 0 ? answer.Remove(answer.Length - 1, 1) : string.Empty;
                            }

                            break;

                        #endregion

                        #region Date

                        case DataTypes.Date:

                            var date = ph.FindControl("dt" + DataTypes.Date.ToString()) as RadDatePicker;
                            aq.Answer = date.SelectedDate.HasValue ? date.SelectedDate.Value.ToString() : string.Empty;

                            if ((AnswerTypes)aq.AnswerType == AnswerTypes.Range)
                            {
                                var maxdate = ph.FindControl("dtalt" + DataTypes.Date.ToString()) as RadDatePicker;
                                aq.AlternateAnswer = maxdate.SelectedDate.HasValue ? maxdate.SelectedDate.Value.ToString() : string.Empty;
                            }

                            break;

                        #endregion
                    }

                    var phOther = rptQuestions.Items[i].FindControl("phOther") as PlaceHolder;
                    FileUpload fu = phOther.FindControl("fileUpload") as FileUpload;
                    if (fu != null && fu.HasFile)
                    {
                        byte[] data = new byte[fu.PostedFile.ContentLength];
                        fu.FileContent.Read(data, 0, fu.PostedFile.ContentLength);
                        aq.NoteBase64Data = System.Convert.ToBase64String(data);
                        aq.NoteFileName = fu.FileName;
                    }
                    else
                        aq.NoteBase64Data = aq.NoteFileName = null;

                    aql.Add(aq);
                }

                if (aql.Count > 0) SaveRequest(this, new SaveRequestEventArgs(aql));
                SetStatusLabel(Statuses.SaveSucceeded);
            }
            catch
            {
                SetStatusLabel(Statuses.SaveFailed);
            }
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

        /// <summary>
        /// Creates a Comment Button
        /// </summary>
        /// <param name="name">Name/ID for the HTML Elements</param>
        /// <param name="displayName">Text to display to the user</param>
        /// <param name="questionId">Question ID (GUID)</param>
        /// <param name="type">Comment Type</param>
        /// <param name="uca">Audience viewing the comment</param>
        /// <returns>Comment HTML Button</returns>
        private HtmlButton CreateCommentButton(string name, string displayName, string questionId, string type, UserType uca, bool commentExists)
        {
            HtmlButton htmlButton = new HtmlButton() { ID = name };
            htmlButton.Attributes.Add("type", "button");

            if (uca == UserType.PCI)
                htmlButton.Attributes.Add("onclick", "launchPCICommentWindow('" + questionId + "', '" + type + "');");
            else
                htmlButton.Attributes.Add("onclick", "launchCommentWindow('" + questionId + "', '" + type + "');");

            HtmlImage htmlImage = new HtmlImage() { Alt = displayName, ID = name + "Image" };
            htmlImage.Attributes.Add("title", displayName);
            if (commentExists)
                htmlImage.Src = "~/images/comment.png";
            else
                htmlImage.Src = "~/images/no-comment.png";
            htmlButton.Controls.Add(htmlImage);
            
            return htmlButton;
        }

        #endregion

        /// <summary>
        /// Save Statuses
        /// </summary>
        private enum Statuses
        {
            Unspecified = 0,
            SaveSucceeded = 1,
            SaveFailed = 2
        }

        public class SaveRequestEventArgs
        {
            public SaveRequestEventArgs(List<IAccountQuestion> aql) { this.AccountQuestions = aql; }
            public List<IAccountQuestion> AccountQuestions { get; private set; }
        }
    }
}