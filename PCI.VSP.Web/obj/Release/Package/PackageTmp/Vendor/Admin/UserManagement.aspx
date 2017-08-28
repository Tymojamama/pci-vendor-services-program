<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="UserManagement.aspx.cs" Inherits="PCI.VSP.Web.VendorUserManagement"
    EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function refreshVendorProducts(contactId) {
            var hfContactId = document.getElementById('<%=ContactIdHiddenField.ClientID %>');
            if (hfContactId == null) { return; }
            hfContactId.value = contactId;

            __doPostBack('<%=VendorProductsUpdatePanel.UniqueID %>', '');
            document.getElementById('divVendorProducts').style.visibility = 'visible';
            var createUserButton = document.getElementById('CreateUserButton');
            if (createUserButton != null && createUserButton != undefined)
                createUserButton.style.visibility = 'hidden';

            document.getElementById('<%=VendorProductsStatusLabel.ClientID %>').value = '';
        }

        function vendorProductsCancelButton_Click() {
            document.getElementById('divVendorProducts').style.visibility = 'collapse';

            var createUserButton = document.getElementById('<%=CreateUserButton.ClientID %>');
            if (createUserButton != null && createUserButton != undefined)
                createUserButton.style.visibility = 'visible';
        }
    </script>
</asp:Content>
<asp:Content ID="Navigation" runat="server" ContentPlaceHolderID="NavContent">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div style="float: right; vertical-align: top;">
        <div class="contentheader">
            <div class="contentheadertext">
                User Management</div>
        </div>
        <div style="float: right; width: 539px;">
            <div class="silverdivider" style="width:539px; float: right;">
            </div>
            <asp:Panel runat="server" HorizontalAlign="Left">
                <div class="contentgridviewcontainer">
                    <asp:GridView ID="ContactsGridView" runat="server" EnableViewState="False" AutoGenerateColumns="False"
                        OnRowDataBound="ContactsGridView_RowDataBound" Width="539px" CellPadding="4"
                        ForeColor="Black" BackColor="White" BorderColor="#E0DCD9">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdContactId" runat="server" />
                                    <asp:HyperLink ID="hlCreateUserLink" runat="server" Text="Edit" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Username" HeaderText="Username" />
                            <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                            <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                        </Columns>
                    </asp:GridView>
                </div>
                <br />
                <input id="CreateUserButton" type="button" runat="server" value="New User" onclick="refreshVendorProducts('');" />
            </asp:Panel>
            <div id="divVendorProducts" style="text-align: left; visibility: hidden;">
                <asp:UpdatePanel ID="VendorProductsUpdatePanel" runat="server" UpdateMode="Conditional"
                    EnableViewState="true" ChildrenAsTriggers="false">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="SaveUserButton" />
                    </Triggers>
                    <ContentTemplate>
                        <table style="background-color: White; color: Black; width: 539px;">
                            <tr>
                                <td colspan="2">
                                    <asp:ValidationSummary ID="CreateUserValidationSummary" ValidationGroup="CreateUserValidationGroup"
                                        runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: top;">
                                    <table style="width: 275px;">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label1" runat="server" Text="Username: "></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="UsernameTextbox" runat="server" Width="128px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="UsernameFieldValidator" runat="server" ErrorMessage="Username is required."
                                                    ControlToValidate="UsernameTextbox" ValidationGroup="CreateUserValidationGroup">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label5" runat="server" Text="Password: "></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="PasswordTextbox" runat="server" TextMode="Password" Width="128px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="PasswordFieldValidator" runat="server" ErrorMessage="Password is required."
                                                    ControlToValidate="PasswordTextbox" ValidationGroup="CreateUserValidationGroup">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label6" runat="server" Text="Confirm Password: "></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="ConfirmPasswordTextbox" runat="server" TextMode="Password" Width="128px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="ConfirmPasswordFieldValidator" runat="server" ErrorMessage="Confirm Password is required."
                                                    ControlToValidate="ConfirmPasswordTextbox" ValidationGroup="CreateUserValidationGroup">*</asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="PasswordCompareValidator" runat="server" ErrorMessage="Passwords do not match."
                                                    ControlToCompare="PasswordTextbox" ControlToValidate="ConfirmPasswordTextbox"
                                                    ValidationGroup="CreateUserValidationGroup"></asp:CompareValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label2" runat="server" Text="Email: "></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="EmailTextbox" runat="server" Width="128px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="EmailFieldValidator" runat="server" ErrorMessage="Email is required."
                                                    ControlToValidate="EmailTextbox" ValidationGroup="CreateUserValidationGroup">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label3" runat="server" Text="First Name: "></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="FirstNameTextbox" runat="server" Width="128px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="FirstNameFieldValidator" runat="server" ErrorMessage="First Name is required."
                                                    ControlToValidate="FirstNameTextbox" ValidationGroup="CreateUserValidationGroup">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label4" runat="server" Text="Last Name: "></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="LastNameTextbox" runat="server" Width="128px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="LastNameFieldValidator" runat="server" ErrorMessage="Last Name is required."
                                                    ControlToValidate="LastNameTextbox" ValidationGroup="CreateUserValidationGroup">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="vertical-align: top;">
                                    <div id="divVendorProductsScroller" style="overflow: auto;">
                                        <asp:Label ID="VendorProductsLabel" runat="server"></asp:Label>
                                        <asp:CheckBoxList ID="VendorProductsCheckBoxList" runat="server" DataValueField="VendorProductId"
                                            DataTextField="VendorProductName" EnableViewState="true">
                                        </asp:CheckBoxList>
                                        <asp:HiddenField ID="ContactIdHiddenField" runat="server" />
                                        <asp:Label ID="VendorProductsStatusLabel" runat="server"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Button ID="SaveUserButton" runat="server" Text="Save User" OnClick="SaveUserButton_Click"
                                                    ValidationGroup="CreateUserValidationGroup" />
                                            </td>
                                            <td>
                                                <input id="CancelUserButton" type="button" value="Cancel" onclick="vendorProductsCancelButton_Click();" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    <script type="text/javascript">
                $(document).ready(function () {
                    $("#<%=ContactsGridView.ClientID%>").find("tr").hover(function () {
                        $(this).css('cursor', 'pointer');
                    }, function () {
                        $(this).css('cursor', 'default');
                    });

                    $("#UserManagementNavImg").css('background', 'url(<%= ResolveClientUrl("~/images/selectedbutton.png") %>)');
                });
    </script>
</asp:Content>
