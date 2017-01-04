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
    <div class="small-width">
        <div class="contentheader">
            <div class="contentheadertext">
                Client Questions
            </div>
            <div class="contentheadersubtext">
                please answer the questions from the selected plan/account
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
                <%--<asp:Repeater ID="rptCategories" runat="server">
                    <ItemTemplate>
                        <div style='<%# (int)Eval("SubItemLevel") > 0 ? "padding-left:" + (int)Eval("SubItemLevel") + "0px;" : "" %>'><asp:LinkButton ID='category' runat="server" CommandName='<%# (int)Eval("Type")%>' CommandArgument='<%# Eval("JSON") %>' Text='<%# Eval("DisplayName")%>' OnClick="category_Click" /></div></>
                    </ItemTemplate>
                </asp:Repeater>--%>
            </div>
        </div>
    </div>
    <div class="small-width">
        <asp:Label ID="lblText" runat="server" Text="" ForeColor="Red" />
        <uc1:QAUserControl ID="QAUserControl1" runat="server" Visible='false' />
        <uc1:InvestmentAssumptionsControl ID="InvestmentAssumptionsControl1" runat="server" Visible='false' />
    </div>
</asp:Content>
