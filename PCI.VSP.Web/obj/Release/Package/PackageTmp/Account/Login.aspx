<%@ Page Title="Log In" Language="C#" MasterPageFile="~/AnonymousSite.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PCI.VSP.Web.Account.Login" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:200' rel='stylesheet' type='text/css'>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="small-width">
        <div class="contentheader">
            <div class="contentheadertext">
                Welcome to the Vendor Portal
            </div>
            <div class="contentheadersubtext">
                please sign in to access the site
            </div>
        </div>
        <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RenderOuterTable="false" MembershipProvider="VspMembershipProvider">
            <LayoutTemplate>
                <div class="login">
                    <div class="login-header"><b>Sign in</b></div>
                    <div class="login-body">
                        <div class="login-fieldset">
                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                                <b>
                                    Username:
                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                    CssClass="failureNotification" ErrorMessage="Username is required." ToolTip="Username is required."
                                    ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                                </b>
                            </asp:Label>
                            <asp:TextBox ID="UserName" runat="server" CssClass="login-text" TabIndex="1"></asp:TextBox>
                            
                        </div>
                        <div class="login-fieldset">
                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
                                <b>
                                    Password (<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Account/PasswordRecovery.aspx">forgot</asp:HyperLink>):
                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                        CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required."
                                        ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                                </b>
                            </asp:Label>
                            <asp:TextBox ID="Password" runat="server" CssClass="login-text" TextMode="Password" TabIndex="2"></asp:TextBox>
                        </div>
                        <div class="login-fieldset" style="margin-bottom: 15px; margin-top: 10px;">
                            <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Sign in" ValidationGroup="LoginUserValidationGroup" CssClass="login-button" />
                        </div>
                        <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification login-notification-area" ValidationGroup="LoginUserValidationGroup" />
                    <%--    <div class="body-block-container">
                            <asp:CheckBox ID="RememberMe" runat="server" />
                            <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline">Keep me logged in</asp:Label>
                        </div>--%>
                        <%--<div class="submit-button">
                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Account/PasswordRecovery.aspx" CssClass="forgot-password">Forgot password</asp:HyperLink>
                        </div>--%>
                    </div>
                </div>
                <div class="failureNotification login-failure-area">
                    <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                </div>
            </LayoutTemplate>
        </asp:Login>
    </div>
</asp:Content>
