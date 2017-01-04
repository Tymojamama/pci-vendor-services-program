using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace PCI.VSP.Web.Account
{
    public partial class ChangeSecurityQuestion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void ChangePasswordQuestion_OnClick(object sender, EventArgs args)
        {
            try
            {
                MembershipUser u = Membership.GetUser(User.Identity.Name);
                Boolean result = u.ChangePasswordQuestionAndAnswer(PasswordTextbox.Text.Trim(),
                                                  QuestionTextbox.Text.Trim(),
                                                  AnswerTextbox.Text.Trim());

                if (result)
                    Msg.Text = "Password Question and Answer changed.";
                else
                    Msg.Text = "Password Question and Answer change failed.";
            }
            catch (Exception e)
            {
                Msg.Text = "Change failed. Please re-enter your values and try again.";
            }
        }

    }
}