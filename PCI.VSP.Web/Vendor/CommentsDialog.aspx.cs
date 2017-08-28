using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PCI.VSP.Services;
using PCI.VSP.Data.Enums;
using PCI.VSP.Services.Model;

namespace PCI.VSP.Web.Vendor
{
    public partial class CommentsDialog : System.Web.UI.Page
    {
        #region Properties

        private Guid EntityId
        {
            get
            {
                return Guid.Parse(Request.QueryString["EntityId"]);
            }
        }

        private Enums.CommentTypes CommentType
        {
            get
            {
                if (!Request.QueryString.AllKeys.Contains("CommentType")) { return Enums.CommentTypes.Unspecified; }
                return (Enums.CommentTypes)Convert.ToInt32(Request.QueryString["CommentType"]);
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Comment1.Audience = Enums.UserType.Vendor;
            Comment1.Question = new VspService().GetIAccountQuestion(EntityId, QuestionEntity.VendorQuestion);
            Comment1.CommentType = CommentType;
            Comment1.SaveRequest += UpdateComment;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to save the vendor questions
        /// </summary>
        /// <param name="qac">QA User Control</param>
        /// <param name="e">QA User Control event arguments</param>
        private void UpdateComment(Controls.Comment qac, Controls.Comment.SaveRequestEventArgs e)
        {
            try
            {
                IUser user = new Security.Utility().GetUser();
                new VspService().UpdateVendorQuestionComment(e.AccountQuestion, user.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}