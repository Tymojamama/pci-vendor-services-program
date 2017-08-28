<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PasswordRecovery.aspx.cs" Inherits="PCI.VSP.Web.Account.PasswordRecovery" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="NavContent" runat="server"></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table style="margin: 0 auto;">
        <tr>
            <td>
                <div class="contentheader">
                    <div class="contentheadertext">
                        Password Recovery
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="silverdivider">
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div style="width: 100%;">
                    <div style="width: 50%; margin: 0px auto;">
                        <asp:PasswordRecovery ID="PasswordRecovery1" runat="server" MembershipProvider="VspMembershipProvider"
                            MailDefinition-From="noreply@tricension.com" MailDefinition-Subject="VSP Password Reset Test">
                        </asp:PasswordRecovery>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
