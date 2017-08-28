<%@ Page Title="" Language="C#" MasterPageFile="~/SiteBase.Master" AutoEventWireup="true" CodeBehind="ProjectInquiriesDialog.aspx.cs" Inherits="PCI.VSP.Web.Vendor.ProjectInquiriesDialog" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../Controls/QAUserControl.ascx" TagName="QAUserControl" TagPrefix="uc1" %>
<%@ Register Src="../Controls/InvestmentAssumptionsControl.ascx" TagName="InvestmentAssumptionsControl" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/RevenueSharingControl.ascx" TagName="RevenueSharingControl" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <base target="_self" />
    <link href="<%= ResolveClientUrl("~/Styles/QAUserControl.css") %>" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="small-width">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server"></telerik:RadScriptManager>
        <div class="contentheader">
            <div class="contentheadertext">
                Project Inquiries
            </div>
            <div class="contentheadersubtext">
                please answer the questions from the selected project
            </div>
        </div>
    </div>
    <div class="content-options">
        <div class="small-width">
            <div class="contentsubheader">
                <span style="margin-right: 15px; color: #f1f4f6;">Question Categories</span>
                <asp:DropDownList ID="ddlCategories" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCategories_SelectedIndexChanged" CssClass="drop-down-list" ToolTip="Question Categories">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <div class="small-width">
        <uc1:QAUserControl ID="QAUserControl1" runat="server" Visible='false' />
        <uc1:InvestmentAssumptionsControl ID="InvestmentAssumptionsControl1" runat="server" Visible='false' />
        <uc1:RevenueSharingControl ID="RevenueSharingControl1" runat="server" Visible='false' />
        <asp:CheckBox ID="answers_confirm" runat="server" Text="The above answers have been confirmed." />
        <asp:HiddenField ID="hfCategoryId" runat="server" />
        <asp:HiddenField ID="hfFunctionId" runat="server" />
        <asp:HiddenField ID="hfType" runat="server" />
    </div>
</asp:Content>
