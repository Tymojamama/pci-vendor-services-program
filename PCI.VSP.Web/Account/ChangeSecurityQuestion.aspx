<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="ChangeSecurityQuestion.aspx.cs" Inherits="PCI.VSP.Web.Account.ChangeSecurityQuestion" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <table>
        <tr>
            <td>
                <div class="contentheader">
                    <div class="contentheadertext">
                        Change Password Question and Answer for
                        <%=User.Identity.Name%></div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Msg" ForeColor="maroon" runat="server" />
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
                <table cellpadding="3" border="0">
                    <tr>
                        <td>
                            Password:
                        </td>
                        <td>
                            <asp:TextBox ID="PasswordTextbox" runat="server" TextMode="Password" />
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="OldPasswordRequiredValidator" runat="server" ControlToValidate="PasswordTextbox"
                                ForeColor="red" Display="Static" ErrorMessage="Required" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            New Password Question:
                        </td>
                        <td>
                            <asp:TextBox ID="QuestionTextbox" MaxLength="256" Columns="60" runat="server" />
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="QuestionRequiredValidator" runat="server" ControlToValidate="QuestionTextbox"
                                ForeColor="red" Display="Static" ErrorMessage="Required" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            New Password Answer:
                        </td>
                        <td>
                            <asp:TextBox ID="AnswerTextbox" MaxLength="128" Columns="60" runat="server" />
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="AnswerRequiredValidator" runat="server" ControlToValidate="AnswerTextbox"
                                ForeColor="red" Display="Static" ErrorMessage="Required" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <asp:Button ID="ChangePasswordQuestionButton" Text="Change Password Question and Answer"
                                OnClick="ChangePasswordQuestion_OnClick" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
