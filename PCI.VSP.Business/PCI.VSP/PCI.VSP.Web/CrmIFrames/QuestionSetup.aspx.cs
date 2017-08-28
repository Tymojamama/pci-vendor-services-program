using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace PCI.VSP.Web.CrmIFrames
{
    public partial class QuestionSetup : System.Web.UI.Page
    {
        private Guid QuestionId
        {
            get
            {
                if (!Request.QueryString.AllKeys.Contains("id")) { return Guid.Empty; }
                return Guid.Parse(Request.QueryString["id"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindQuestionDataTypes();

                // get the question type, select the appropriate values
                Guid questionId = this.QuestionId;
                Data.CRM.Model.Question question = null;

                if (questionId != Guid.Empty)
                {
                    Services.VspService service = new Services.VspService();
                    question = service.GetQuestion(questionId);

                    TrySelectListItem(Convert.ToInt32(question.QuestionDataType), QuestionTypeDropDownList);
                }

                Model.FilterCriteria fc = new Model.FilterCriteria() { QuestionDataType = QuestionDataType };
                BindClientResponseTypes(fc);

                if (question != null)
                    TrySelectListItem(Convert.ToInt32(question.ClientAnswerType), ClientResponseTypeDropDownList);

                fc.ClientResponseType = ClientResponseType;
                BindVendorResponseTypes(fc);

                if (question != null)
                    TrySelectListItem(Convert.ToInt32(question.VendorAnswerType), VendorResponseTypeDropDownList);
                fc.VendorResponseType = VendorResponseType;
                BindComparisonTypes(fc);

                if (question != null)
                    TrySelectListItem(Convert.ToInt32(question.ComparisonType), ComparisonTypeDropDownList);
            }
            CreateSummary();
            //else
            //    NotifyParent();
        }

        private void CreateSummary()
        {
            QuestionTypeHiddenField.Value = Convert.ToInt32(QuestionDataType).ToString();
            ClientResponseTypeHiddenField.Value = Convert.ToInt32(ClientResponseType).ToString();
            VendorResponseTypeHiddenField.Value = Convert.ToInt32(VendorResponseType).ToString();
            ComparisonTypeHiddenField.Value = Convert.ToInt32(ComparisonType).ToString();
        }

        private void NotifyParent()
        {
            //String questionDataType = Convert.ToInt32(QuestionDataType).ToString();
            //String clientAnswerType = Convert.ToInt32(ClientResponseType).ToString();
            //String vendorAnswerType = Convert.ToInt32(VendorResponseType).ToString();
            //String comparisonType = Convert.ToInt32(ComparisonType).ToString();

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //sb.AppendLine("alert($(parent.document).find('#vsp_clientanswertype').val());");
            //sb.AppendLine("$(parent.document).find('#vsp_clientanswertype').val('" + clientAnswerType + "');");
            //sb.AppendLine("$(parent.document).find('#vsp_comparisontype').val('" + comparisonType + "');");
            //sb.AppendLine("$(parent.document).find('#vsp_vendoranswertype').val('" + vendorAnswerType + "');");
            //sb.AppendLine("$(parent.document).find('#vsp_questiondatatype').val('" + questionDataType + "');");

            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "NotifyParentOfQuestionSetup", sb.ToString(), true);

            //sb.AppendLine("foreach (var o in parent.document.forms[0].all) {");
            //sb.AppendLine("   alert(o.toString()); }");
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "parentPolling", sb.ToString(), true);

            //Page.ClientScript.RegisterStartupScript(this.GetType(), "NotifyParentOfQuestionType",
            //    "alert('NotifyParentOfQuestionType: " + questionDataType + "'); parent.document.forms[0].all.vsp_questiondatatype.DataValue = " + questionDataType + ";", true);
        }

        private Data.Enums.DataTypes QuestionDataType
        {
            get
            {
                if (QuestionTypeDropDownList.SelectedIndex == -1) { return Data.Enums.DataTypes.Unspecified; }
                return (Data.Enums.DataTypes)Convert.ToInt32(QuestionTypeDropDownList.SelectedItem.Value);
            }
        }

        private Data.Enums.AnswerTypes ClientResponseType
        {
            get
            {
                if (ClientResponseTypeDropDownList.SelectedIndex == -1) { return Data.Enums.AnswerTypes.Unspecified; }
                return (Data.Enums.AnswerTypes)Convert.ToInt32(ClientResponseTypeDropDownList.SelectedItem.Value);
            }
        }

        private Data.Enums.AnswerTypes VendorResponseType
        {
            get
            {
                if (VendorResponseTypeDropDownList.SelectedIndex == -1) { return Data.Enums.AnswerTypes.Unspecified; }
                return (Data.Enums.AnswerTypes)Convert.ToInt32(VendorResponseTypeDropDownList.SelectedItem.Value);
            }
        }

        private Data.CRM.Model.Question.ComparisonTypes ComparisonType
        {
            get
            {
                if (ComparisonTypeDropDownList.SelectedIndex == -1) { return Data.CRM.Model.Question.ComparisonTypes.Unspecified; }
                return (Data.CRM.Model.Question.ComparisonTypes)Convert.ToInt32(ComparisonTypeDropDownList.SelectedItem.Value);
            }
        }

        private Model.FilterCriteria GetFilterCriteria()
        {
            return new Model.FilterCriteria()
            {
                ClientResponseType = ClientResponseType,
                VendorResponseType = VendorResponseType,
                QuestionDataType = QuestionDataType,
                ComparisonType = ComparisonType
            };
        }

        private void BindQuestionDataTypes()
        {
            QuestionTypeDropDownList.DataSource = new Model.QuestionDataTypes();
            QuestionTypeDropDownList.DataBind();
        }

        private void BindClientResponseTypes(Model.FilterCriteria fc)
        {
            Int32? selectedValue = new Int32?();
            if (ClientResponseTypeDropDownList.SelectedIndex != -1)
                selectedValue = Convert.ToInt32(ClientResponseTypeDropDownList.SelectedItem.Value);

            ClientResponseTypeDropDownList.DataSource = new Model.ClientResponseTypes(fc);
            ClientResponseTypeDropDownList.DataBind();

            TrySelectListItem(selectedValue, ClientResponseTypeDropDownList);
        }

        private void TrySelectListItem(Int32? selectedValue, DropDownList ddl)
        {
            if (!selectedValue.HasValue || ddl == null) { return; }
            foreach (ListItem li in ddl.Items)
            {
                if (Convert.ToInt32(li.Value) == selectedValue.Value)
                {
                    ddl.SelectedValue = li.Value;
                    break;
                }
            }
        }

        private void BindVendorResponseTypes(Model.FilterCriteria fc)
        {
            Int32? selectedValue = new Int32?();
            if (VendorResponseTypeDropDownList.SelectedIndex != -1)
                selectedValue = Convert.ToInt32(VendorResponseTypeDropDownList.SelectedItem.Value);

            VendorResponseTypeDropDownList.DataSource = new Model.VendorResponseTypes(fc);
            VendorResponseTypeDropDownList.DataBind();

            TrySelectListItem(selectedValue, VendorResponseTypeDropDownList);
        }

        private void BindComparisonTypes(Model.FilterCriteria fc)
        {
            Int32? selectedValue = new Int32?();
            if (ComparisonTypeDropDownList.SelectedIndex != -1)
                selectedValue = Convert.ToInt32(ComparisonTypeDropDownList.SelectedItem.Value);

            ComparisonTypeDropDownList.DataSource = new Model.ComparisonTypes(fc);
            ComparisonTypeDropDownList.DataBind();

            TrySelectListItem(selectedValue, ComparisonTypeDropDownList);
        }

        protected void QuestionTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Model.FilterCriteria fc = new Model.FilterCriteria() { QuestionDataType = QuestionDataType };
            BindClientResponseTypes(fc);
            fc.ClientResponseType = ClientResponseType;
            BindVendorResponseTypes(fc);
            fc.VendorResponseType = VendorResponseType;
            BindComparisonTypes(fc);
        }

        protected void ClientResponseTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Model.FilterCriteria fc = new Model.FilterCriteria() { QuestionDataType = QuestionDataType, ClientResponseType = ClientResponseType };
            BindVendorResponseTypes(fc);
            fc.VendorResponseType = VendorResponseType;
            BindComparisonTypes(fc);
        }

        protected void VendorResponseTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Model.FilterCriteria fc = new Model.FilterCriteria() { QuestionDataType = QuestionDataType, VendorResponseType = VendorResponseType };
            BindClientResponseTypes(fc);
            fc.ClientResponseType = ClientResponseType;
            BindComparisonTypes(fc);
        }

        private void Save()
        {
            if (this.QuestionId == Guid.Empty) { return; }
            Data.CRM.Model.Question question = new Data.CRM.Model.Question()
            {
                QuestionId = this.QuestionId,
                QuestionDataType = this.QuestionDataType,
                ClientAnswerType = this.ClientResponseType,
                VendorAnswerType = this.VendorResponseType,
                ComparisonType = this.ComparisonType
            };

            Services.VspService service = new Services.VspService();
            service.SaveQuestion(question);
        }
    }
}