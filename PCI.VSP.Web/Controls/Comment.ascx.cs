using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PCI.VSP.Web.Enums;
using PCI.VSP.Data.Enums;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Services;
using PCI.VSP.Services.Model;

namespace PCI.VSP.Web.Controls
{
    public partial class Comment : System.Web.UI.UserControl
    {
        #region Private Properties

        private UserType _audience = UserType.Unspecified;

        private IAccountQuestion _question;

        private Enums.CommentTypes _commentType = CommentTypes.Unspecified;

        #endregion

        #region Public Properties

        /// <summary>
        /// The person that is viewing the contents of the control
        /// </summary>
        public UserType Audience
        {
            get
            {
                return _audience;
            }
            set
            {
                _audience = value;
            }
        }

        public IAccountQuestion Question
        {
            get
            {
                return _question;
            }
            set
            {
                _question = value;
            }
        }

        /// <summary>
        /// Comment Type
        /// </summary>
        public CommentTypes CommentType
        {
            get
            {
                return _commentType;
            }
            set
            {
                _commentType = value;
            }
        }

        public delegate void SaveRequestEventHandler(Comment sender, SaveRequestEventArgs srea);
        public event SaveRequestEventHandler SaveRequest;

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Audience == UserType.Unspecified || Question == null || CommentType == CommentTypes.Unspecified) return;

            if (!IsPostBack)
            {
                #region Set Comment Textbox Text

                switch (CommentType)
                {
                    case CommentTypes.VendorToClient:
                        if (Question is VendorQuestion)
                            CommentsTextBox.Text = (Question as VendorQuestion).VendorCommentToClient;
                        break;
                    case CommentTypes.ClientToVendor:
                        if (Question is VendorQuestion)
                            CommentsTextBox.Text = (Question as VendorQuestion).ClientCommentToVendor;
                        else if (Question is ClientQuestion)
                            CommentsTextBox.Text = (Question as ClientQuestion).ClientCommentToVendor;
                        break;
                    case CommentTypes.PciToClient:
                    case CommentTypes.PciToVendor:
                        CommentsTextBox.Text = Question.CommentFromPCI;
                        break;
                    case CommentTypes.VendorToPci:
                    case CommentTypes.ClientToPci:
                        CommentsTextBox.Text = Question.CommentToPCI;
                        break;
                    default:
                        CommentsTextBox.Text = string.Empty;
                        break;
                }

                #endregion

                #region Set Comment Label Text

                switch (CommentType)
                {
                    case Enums.CommentTypes.ClientToVendor:
                        CommentsLabel.Text = "Client Comment to Vendor:";
                        break;
                    case Enums.CommentTypes.PciToVendor:
                        CommentsLabel.Text = "PCI Comment to Vendor:";
                        break;
                    case Enums.CommentTypes.VendorToClient:
                        CommentsLabel.Text = "Vendor Comment to Client:";
                        break;
                    case Enums.CommentTypes.ClientToPci:
                        CommentsLabel.Text = "Client Comment To PCI";
                        break;
                    case Enums.CommentTypes.PciToClient:
                        CommentsLabel.Text = "PCI Comment To Client";
                        break;
                    case Enums.CommentTypes.VendorToPci:
                        CommentsLabel.Text = "Vendor Comment To PCI";
                        break;
                }

                #endregion

                if (Question.Status == AccountQuestionStatuses.AccountConfirmed || Question.Status == AccountQuestionStatuses.PCI_Confirmed)
                    DisableTextEntry();
                else if (Audience == UserType.Client && Question is ClientQuestion && (CommentType == CommentTypes.VendorToClient || CommentType == CommentTypes.PciToClient))
                    DisableTextEntry();
                else if (Audience == UserType.Vendor && Question is VendorQuestion && (CommentType == CommentTypes.ClientToVendor || CommentType == CommentTypes.PciToVendor))
                    DisableTextEntry();
            }
        }

        protected void CloseButton_Click(object sender, EventArgs e)
        {
            if (Question != null)
            {
                switch (CommentType)
                {
                    case CommentTypes.VendorToClient:
                        if (Question is VendorQuestion)
                            (Question as VendorQuestion).VendorCommentToClient = CommentsTextBox.Text;
                        break;
                    case CommentTypes.ClientToVendor:
                        if (Question is VendorQuestion)
                            (Question as VendorQuestion).ClientCommentToVendor = CommentsTextBox.Text;
                        else if (Question is ClientQuestion)
                            (Question as ClientQuestion).ClientCommentToVendor = CommentsTextBox.Text;
                        break;
                    case CommentTypes.PciToClient:
                    case CommentTypes.PciToVendor:
                        Question.CommentFromPCI = CommentsTextBox.Text;
                        break;
                    case CommentTypes.VendorToPci:
                    case CommentTypes.ClientToPci:
                        Question.CommentToPCI = CommentsTextBox.Text;
                        break;
                }

                SaveRequest(this, new SaveRequestEventArgs(Question));
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "CloseWindow", "parent.HideCommentsModalWindow();", true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Make the Comments Textbox readonly and hide the Close And Save button
        /// </summary>
        private void DisableTextEntry()
        {
            CommentsTextBox.ReadOnly = true;
            CommentsTextBox.Enabled = false;
            CloseButton.Visible = false;
        }

        #endregion

        public class SaveRequestEventArgs
        {
            public SaveRequestEventArgs(IAccountQuestion aql) { this.AccountQuestion = aql; }
            public IAccountQuestion AccountQuestion { get; private set; }
        }
    }
}