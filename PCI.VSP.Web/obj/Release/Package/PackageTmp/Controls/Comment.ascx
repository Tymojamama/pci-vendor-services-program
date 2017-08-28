<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Comment.ascx.cs" Inherits="PCI.VSP.Web.Controls.Comment" %>
<table>
    <tr>
        <td>
            <asp:Label ID="CommentsLabel" runat="server" Text="Comments"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <asp:TextBox ID="CommentsTextBox" runat="server" Columns="50" Rows="10" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <span style="float:right;">
                <input id="btnCancel" type="button" value="Cancel" onclick="parent.HideCommentsModalWindow();" />
                <asp:Button ID="CloseButton" runat="server" Text="Save And Close" OnClick="CloseButton_Click" />
            </span>
        </td>
    </tr>
</table>