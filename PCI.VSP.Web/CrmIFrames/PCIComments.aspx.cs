using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PCI.VSP.Data.Enums;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Services;
using PCI.VSP.Web.Enums;

namespace PCI.VSP.Web.CrmIFrames
{
    public partial class PCIComments : System.Web.UI.Page
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
            Comment1.Audience = UserType.PCI;

            switch (CommentType)
            {
                case CommentTypes.VendorToPci:
                case CommentTypes.VendorToClient:
                case CommentTypes.PciToVendor:
                    Comment1.Question = new VspService().GetIAccountQuestion(EntityId, QuestionEntity.VendorQuestion);
                    break;
                case CommentTypes.PciToClient:
                case CommentTypes.ClientToVendor:
                case CommentTypes.ClientToPci:
                    Comment1.Question = new VspService().GetIAccountQuestion(EntityId, QuestionEntity.ClientQuestion);
                    break;
                case CommentTypes.Unspecified:
                default:
                    throw new Exception("Invalid Comment Type");
                    break;
            }

            //Comment1.Question = new VspService().GetIAccountQuestion(EntityId, QuestionEntity.ClientQuestion);
            Comment1.CommentType = CommentType;
            Comment1.SaveRequest += UpdateComment;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to save the client questions
        /// </summary>
        /// <param name="qac">QA User Control</param>
        /// <param name="e">QA User Control event arguments</param>
        private void UpdateComment(Controls.Comment qac, Controls.Comment.SaveRequestEventArgs e)
        {
            try
            {
                switch (CommentType)
                {
                    case CommentTypes.VendorToPci:
                    case CommentTypes.VendorToClient:
                    case CommentTypes.PciToVendor:
                        new Services.VspService().UpdateVendorQuestionComment(e.AccountQuestion, null);
                        break;
                    case CommentTypes.PciToClient:
                    case CommentTypes.ClientToVendor:
                    case CommentTypes.ClientToPci:
                        new Services.VspService().UpdateClientQuestion(e.AccountQuestion);
                        break;
                    case CommentTypes.Unspecified:
                    default:
                        throw new Exception("Invalid Comment Type");
                        break;
                }

                //new Services.VspService().UpdateClientQuestion(e.AccountQuestion);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}