<%@ Page Title="" Language="C#" MasterPageFile="~/SiteBase.Master" AutoEventWireup="true" CodeBehind="PCI_ClientQuestions.aspx.cs" Inherits="PCI.VSP.Web.CrmIFrames.PCI_ClientQuestions" ViewStateMode="Enabled" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/QAUserControl.ascx" TagName="QAUserControl" TagPrefix="uc1" %>
<%@ Register Src="../Controls/InvestmentAssumptionsControl.ascx" TagName="InvestmentAssumptionsControl" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <base target="_self" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
    </telerik:RadScriptManager>
    <asp:HiddenField ID="hfCategoryId" runat="server" />
    <asp:HiddenField ID="hfFunctionId" runat="server" />
    <asp:HiddenField ID="hfType" runat="server" />
    <div style="vertical-align: top;">
        <table>
            <tr>
                <td colspan="2">
                    <div class="contentheader" style="background-position: inherit; padding-left: 18px;">
                        <div class="contentheadertext" style="float: inherit;">Client Questions:</div>
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
                    <asp:Label ID="lblText" runat="server" Text="" ForeColor="Red" />
                    <uc1:QAUserControl ID="QAUserControl1" runat="server" Visible='false' />
                    <uc1:InvestmentAssumptionsControl ID="InvestmentAssumptionsControl1" runat="server" Visible='false' />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
