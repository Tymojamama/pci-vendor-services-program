<%@ Page Title="" Language="C#" MasterPageFile="~/SiteBase.Master" AutoEventWireup="true" CodeBehind="ProjectInquiriesDialog.aspx.cs" Inherits="PCI.VSP.Web.Vendor.ProjectInquiriesDialog" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/QAUserControl.ascx" TagName="QAUserControl" TagPrefix="uc1" %>
<%@ Register Src="../Controls/InvestmentAssumptionsControl.ascx" TagName="InvestmentAssumptionsControl" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/RevenueSharingControl.ascx" TagName="RevenueSharingControl" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <base target="_self" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server"></telerik:RadScriptManager>
    <div style="vertical-align: top;">
        <table>
            <tr>
                <td colspan="2">
                    <div class="contentheader" style="background-position: inherit; padding-left: 18px;">
                        <div class="contentheadertext" style="float: inherit;">Project Inquiries:</div>
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
                            <tr><td style='<%# (int)Eval("SubItemLevel") > 0 ? "padding-left:" + (int)Eval("SubItemLevel") + "0px;" : "" %>'><asp:LinkButton ID='category' runat="server" CommandName='<%# (int)Eval("Type")%>' CommandArgument='<%# Eval("JSON") %>' Text='<%# Eval("DisplayName")%>' OnClick="category_Click" /></td></tr>
                        </ItemTemplate>
                        <FooterTemplate></table></FooterTemplate>
                    </asp:Repeater>
                </td>
                <td>
                    <uc1:QAUserControl ID="QAUserControl1" runat="server" Visible='false' />
                    <uc1:InvestmentAssumptionsControl ID="InvestmentAssumptionsControl1" runat="server" Visible='false' />
                    <uc1:RevenueSharingControl ID="RevenueSharingControl1" runat="server" Visible='false' />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="text-align: center;">
                    <asp:CheckBox ID="answers_confirm" runat="server" Text="Confirm answers above for this page? (if yes, check the box)" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hfCategoryId" runat="server" />
    <asp:HiddenField ID="hfFunctionId" runat="server" />
    <asp:HiddenField ID="hfType" runat="server" />
</asp:Content>
