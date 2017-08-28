using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PCI.VSP.Web
{
    public partial class VendorUserManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // get contacts for this person's account
                RefreshVendorAgents();
                RefreshVendorProducts();
                DataBindVendorProducts();
            }

            if (!ScriptManager1.IsInAsyncPostBack)
                BindData();
            else if (Request.Form["__EVENTTARGET"] == VendorProductsUpdatePanel.UniqueID)
            {
                VendorProductsUpdatePanel.Visible = true;
                RefreshVendorAgentVendorProducts();
                BindUserInfo();
                VendorProductsUpdatePanel.Update();
            }
        }

        private void BindUserInfo()
        {
            Services.VspService service = new Services.VspService();

            bool isNewUser = String.IsNullOrWhiteSpace(ContactIdHiddenField.Value);
            PasswordTextbox.Visible = isNewUser;
            ConfirmPasswordTextbox.Visible = isNewUser;
            PasswordCompareValidator.Enabled = isNewUser;
            PasswordFieldValidator.Enabled = isNewUser;
            ConfirmPasswordFieldValidator.Enabled = isNewUser;

            if (isNewUser) {
                ClearControls();
                return;             
            }
            
            Guid? contactId = new Guid?();
            contactId = Guid.Parse(ContactIdHiddenField.Value);

            Services.Model.IUser user = service.GetUser(contactId.Value);
            EmailTextbox.Text = user.Email;
            FirstNameTextbox.Text = user.FirstName;
            LastNameTextbox.Text = user.LastName;
            UsernameTextbox.Text = user.Username;
            PasswordTextbox.Visible = false;
            ConfirmPasswordTextbox.Visible = false;
        }

        private void SaveVendorProductSelections(Guid contactId)
        {
            // get the contactId
            try
            {
                List<Guid> vendorProductIds = new List<Guid>();

                // get the list of selected vendor products
                foreach (ListItem li in VendorProductsCheckBoxList.Items)
                    if (li.Selected)
                        vendorProductIds.Add(Guid.Parse(li.Value));

                // make the update request
                Services.Model.IUser user = new Security.Utility().GetUser();
                Services.VspService service = new Services.VspService();
                service.UpdateAgentProducts(user.AccountId, contactId, vendorProductIds);

                // if the contactId belongs to the current user, refresh the vendorProduct cache
                if (user.Id == contactId)
                {
                    RefreshVendorProducts();
                    DataBindVendorProducts();
                }
                
                VendorProductsStatusLabel.Text = "The save operation succeeded.";
            }
            catch (Exception ex)
            {
                VendorProductsStatusLabel.Text = "The save operation failed.";
            }
        }

        private void ClearControls()
        {
            TextBox[] textboxes = new TextBox[] { ConfirmPasswordTextbox, EmailTextbox, FirstNameTextbox, LastNameTextbox, PasswordTextbox, UsernameTextbox };
            foreach (TextBox textbox in textboxes)
                textbox.Text = String.Empty;
            foreach (ListItem li in VendorProductsCheckBoxList.Items)
                li.Selected = false;
        }

        private void RefreshVendorAgentVendorProducts()
        {
            foreach(ListItem li in VendorProductsCheckBoxList.Items)
                li.Selected = false;

            // get the contactId
            if (String.IsNullOrWhiteSpace(ContactIdHiddenField.Value)) { return; }
            
            Guid? contactId = new Guid?();
            contactId = Guid.Parse(ContactIdHiddenField.Value);

            // get this contact's assigned vendorProducts
            Services.Model.IUser user = new Security.Utility().GetUser();
            Services.VspService service = new Services.VspService();
            List<Data.CRM.Model.VendorProduct> vendorProducts = service.GetAgentProducts(user.AccountId, contactId.Value);

            // check the checkboxes
            foreach(Data.CRM.Model.VendorProduct vp in vendorProducts)
                foreach (ListItem li in VendorProductsCheckBoxList.Items)
                    if (vp.VendorProductId == Guid.Parse(li.Value))
                    {
                        li.Selected = true;
                        continue;
                    }

            // update the vendor products label with the contact's name
            List<Services.Model.VendorAgent> vendorAgents = (List<Services.Model.VendorAgent>)Session[Constants.VendorAgents];
            if (vendorAgents != null)
            {
                foreach (Services.Model.VendorAgent vendorAgent in vendorAgents)
                    if (vendorAgent.Id == contactId)
                    {
                        VendorProductsLabel.Text = "Products assigned to " + vendorAgent.FirstName + " " + vendorAgent.LastName + ":";
                        break;
                    }
            }
        }

        private void DataBindVendorProducts()
        {
            List<Data.CRM.Model.VendorProduct> vendorProducts = (List<Data.CRM.Model.VendorProduct>)Session[Constants.VendorProducts];
            VendorProductsCheckBoxList.DataSource = vendorProducts;
            VendorProductsCheckBoxList.DataBind();
        }

        private void RefreshVendorAgents()
        {
            Security.Utility sutility = new Security.Utility();
            PCI.VSP.Services.VspService service = new Services.VspService();

            List<PCI.VSP.Services.Model.VendorAgent> vendorAgents = service.GetAgentSummary(sutility.GetUser().AccountId);
            Session[Constants.VendorAgents] = vendorAgents;
        }

        private void RefreshVendorProducts()
        {
            Services.Model.IUser user = new Security.Utility().GetUser();
            PCI.VSP.Services.VspService service = new Services.VspService();
            List<Data.CRM.Model.VendorProduct> vendorProducts;

            if (user is PCI.VSP.Services.Model.VendorAdmin)
                vendorProducts = service.GetVendorProducts(user.AccountId);
            else
                vendorProducts = service.GetAgentProducts(user.AccountId, user.Id);

            Session[Constants.VendorProducts] = vendorProducts;
        }

        private void BindData()
        {
            List<PCI.VSP.Services.Model.VendorAgent> vendorAgents = (List<PCI.VSP.Services.Model.VendorAgent>)Session[Constants.VendorAgents];
            ContactsGridView.DataSource = vendorAgents;
            ContactsGridView.DataBind();
        }

        protected void SaveUserButton_Click(object sender, EventArgs e)
        {

            //if (String.IsNullOrWhiteSpace(ContactIdHiddenField.Value)) { return; }

            Guid contactId = new Guid();
            Services.Model.VendorAgent va = ReadVendorAgent();
            Services.VspService service = new Services.VspService();

            if (Guid.TryParse(ContactIdHiddenField.Value, out contactId))
            {
                va.Id = contactId;
                service.UpdateAgent(va);
            }
            else
            {
                contactId = service.CreateAgent(va);
            }

            SaveVendorProductSelections(contactId);
            RefreshVendorAgents();
            BindData();
            ClearControls();
        }

        private Services.Model.VendorAgent ReadVendorAgent()
        {
            Services.Model.VendorAgent va = new Services.Model.VendorAgent()
            {
                Email = EmailTextbox.Text.Trim(),
                FirstName = FirstNameTextbox.Text.Trim(),
                LastName = LastNameTextbox.Text.Trim(),
                Username = UsernameTextbox.Text.Trim(),
                AccountId = new Security.Utility().GetUser().AccountId
            };
            if (!String.IsNullOrWhiteSpace(PasswordTextbox.Text.Trim()))
                va.Password = PasswordTextbox.Text.Trim();
            return va;
        }

        protected void ContactsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HiddenField hf = (HiddenField)e.Row.FindControl("hdContactId");
                if (hf != null) { hf.Value = DataBinder.Eval(e.Row.DataItem, "Id").ToString(); }

                HyperLink h = (HyperLink)e.Row.FindControl("hlCreateUserLink");
                h.Attributes.Add("onclick", "refreshVendorProducts('" + hf.Value + "');");
            }
        }

    }

    
}