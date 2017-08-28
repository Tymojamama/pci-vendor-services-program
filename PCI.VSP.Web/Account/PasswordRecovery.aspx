<%@ Page Title="" Language="C#" MasterPageFile="~/AnonymousSite.master" AutoEventWireup="true" CodeBehind="PasswordRecovery.aspx.cs" Inherits="PCI.VSP.Web.Account.PasswordRecovery" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:200' rel='stylesheet' type='text/css'>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="NavContent" runat="server"></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="small-width">
        <div class="contentheader" style="margin-top: 25px;">
            <div class="contentheadertext" style="color: #0e2e47;">
                Forget your password?
            </div>
            <div class="contentheadersubtext">
                you can use this form to reset your password
            </div>
        </div>
        <div class="login">
            <div class="login-header"><b>Password Reset</b></div>
            <div class="login-body">
                <div class="login-fieldset">
                        <b>
                            Username:
                        </b>
                    <asp:PasswordRecovery ID="PasswordRecovery1" runat="server" MembershipProvider="VspMembershipProvider"
                        MailDefinition-From="noreply@pension-consultants.com" MailDefinition-Subject="VSP Password Reset Test" UserNameInstructionText="" UserNameTitleText="">
                    </asp:PasswordRecovery>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
