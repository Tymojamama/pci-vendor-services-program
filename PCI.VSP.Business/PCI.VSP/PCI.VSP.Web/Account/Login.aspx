<%@ Page Title="Log In" Language="C#" MasterPageFile="~/AnonymousSite.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PCI.VSP.Web.Account.Login" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="small-width">
        <div class="contentheader">
            <div class="contentheadertext">
                Welcome to the Vendor Services Program
            </div>
            <div class="contentheadersubtext">
                Please enter your username and password.
            </div>
        </div>
        <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RenderOuterTable="false"
            MembershipProvider="VspMembershipProvider">
            <LayoutTemplate>
                <span class="failureNotification">
                    <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                </span>
                <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification"
                    ValidationGroup="LoginUserValidationGroup" />
                <div class="accountInfo">
                    <fieldset class="login">
                        <legend>Account Information</legend>
                        <div class="body-block-container">
                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">Username:</asp:Label>
                            <asp:TextBox ID="UserName" runat="server" CssClass="textEntry"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="User Name is required."
                                ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                        </div>
                        <div class="body-block-container">
                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                            <asp:TextBox ID="Password" runat="server" CssClass="textEntry" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required."
                                ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                        </div>
                        <div class="body-block-container">
                            <asp:CheckBox ID="RememberMe" runat="server" />
                            <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline">Keep me logged in</asp:Label>
                        </div>
                    </fieldset>
                    <div class="submitButton">
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Account/PasswordRecovery.aspx" CssClass="forgot-password">Forgot password</asp:HyperLink>
                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" ValidationGroup="LoginUserValidationGroup" />
                    </div>
                </div>
            </LayoutTemplate>
        </asp:Login>
    </div>
</asp:Content>
