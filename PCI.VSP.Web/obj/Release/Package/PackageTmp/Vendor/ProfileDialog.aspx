<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProfileDialog.aspx.cs" Inherits="PCI.VSP.Web.Vendor.ProfileDialog" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/QAUserControl.ascx" TagName="QAUserControl" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <base target="_self" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server"></telerik:RadScriptManager>
    <div style="vertical-align: top;">
        <table>
            <tr>
                <td colspan="2">
                    <div class="contentheader">
                        <div class="contentheadertext" style="float: inherit;">General Profile Questions:</div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="silverdivider"></div>
                </td>
            </tr>
            <tr>
                <td style='vertical-align:top;'>
                    <asp:Repeater ID="rptCategories" runat="server">
                        <HeaderTemplate><table></HeaderTemplate>
                        <ItemTemplate>
                            <tr><td style='<%# (int)Eval("SubItemLevel") > 0 ? "padding-left:" + (int)Eval("SubItemLevel") + "0px;" : "" %>'> <asp:LinkButton ID='category' runat="server" CommandName='<%# (int)Eval("Type")%>' CommandArgument='<%# Eval("JSON") %>' Text='<%# Eval("DisplayName")%>' OnClick="category_Click" /></td></tr>
                        </ItemTemplate>
                        <FooterTemplate></table></FooterTemplate>
                    </asp:Repeater>
                </td>
                <td>
                    <uc1:QAUserControl ID="QAUserControl1" runat="server" Visible='false' />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hfCategoryId" runat="server" />
    <asp:HiddenField ID="hfFunctionId" runat="server" />
</asp:Content>
