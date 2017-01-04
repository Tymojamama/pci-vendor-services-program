<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InvestmentAssumptionsControl.ascx.cs" Inherits="PCI.VSP.Web.Controls.InvestmentAssumptionsControl" %>

<style type="text/css">
    .inputwidth
    {
        width: 225px;
    }
    
    .tdwidth
    {
        width: 240px;
    }
</style>

<asp:ValidationSummary ID="InvestmentAssumptionValidationSummary" runat="server" ValidationGroup="InvestmentAssumptions" ShowSummary="true" HeaderText="To save the Investment Assumptions, please correct the following:" EnableClientScript="true" DisplayMode="List" ForeColor="Red" />
<table>
<asp:Repeater ID="rptQuestions" runat="server" Visible="true" OnItemDataBound="rptQuestions_ItemDataBound">
    <ItemTemplate>
        <tr><td colspan="7"><asp:Literal ID="litNumber" runat="server" /></td></tr><tr><td>Asset Class</td><td>&nbsp</td><td class="tdwidth"><asp:DropDownList ID="ddlAssetClass" runat="server" CssClass="inputwidth"></asp:DropDownList></td><td>&nbsp</td><td>Asset Fund</td><td>&nbsp</td><td class="tdwidth"><asp:TextBox ID="AssetFundTextBox" runat="server" Text='<%# Eval("AssetFund") %>' CssClass="inputwidth" ></asp:TextBox></td></tr><tr><td>Asset Symbol</td><td>&nbsp</td><td><asp:TextBox ID="AssetSymbolTextBox" runat="server" Text='<%# Eval("AssetSymbol") %>' CssClass="inputwidth"></asp:TextBox></td><td>&nbsp</td><td>Assets</td><td>$</td><td><asp:TextBox ID="AssetsTextBox" runat="server" Text='<%# Eval("Assets", "{0:n0}")%>' CssClass="inputwidth"></asp:TextBox><asp:RegularExpressionValidator ID="revAssets" runat="server" ValidationExpression="^[-]?[0-9]{1,3}(?:,?[0-9]{3})*(?:\.[0-9]{2})?$" ValidationGroup="InvestmentAssumptions" ForeColor="Red" Text="*" EnableClientScript="true" Display="Dynamic"></asp:RegularExpressionValidator></td></tr><tr><td>Annual Contributions</td><td>$</td><td><asp:TextBox ID="AnnualContributionsTextBox" runat="server" Text='<%# Eval("AnnualContributions", "{0:n0}") %>' CssClass="inputwidth"></asp:TextBox><asp:RegularExpressionValidator ID="revAnnualContributions" runat="server" ValidationExpression="^[-]?[0-9]{1,3}(?:,?[0-9]{3})*(?:\.[0-9]{2})?$" ValidationGroup="InvestmentAssumptions" ForeColor="Red" Text="*" EnableClientScript="true" Display="Dynamic"></asp:RegularExpressionValidator></td><td>&nbsp</td><td>Participants</td><td>&nbsp</td><td><asp:TextBox ID="ParticipantsTextBox" runat="server" Text='<%# Eval("Participants") %>' CssClass="inputwidth"></asp:TextBox><asp:RegularExpressionValidator ID="revParticipants" runat="server" ValidationExpression="\d+" ValidationGroup="InvestmentAssumptions" ForeColor="Red" Text="*" EnableClientScript="true" Display="Dynamic"></asp:RegularExpressionValidator></td></tr><tr><td colspan="7"><asp:HiddenField ID="hfId" runat="server" /></td></tr><tr id="trConfirm" runat="server"><td colspan="4">&nbsp</td><td>Confirm</td><td>&nbsp</td><td><asp:DropDownList ID="Confirmation" runat="server"><asp:ListItem></asp:ListItem><asp:ListItem Value="True">Confirm</asp:ListItem><asp:ListItem Value="False">Deny</asp:ListItem></asp:DropDownList></td></tr><tr id="trIsRequired" runat="server"><td colspan="4">&nbsp</td><td>Used in Vendor Filtering</td><td>&nbsp</td><td><asp:DropDownList ID="ddlRequired" runat="server"><asp:ListItem Value="False">No</asp:ListItem><asp:ListItem Value="True">Yes</asp:ListItem></asp:DropDownList></td></tr>
    </ItemTemplate>
</asp:Repeater>
        <tr id="trFooter" runat="server">
            <td colspan="7">
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
                <asp:Button ID="btnSaveAndClose" runat="server" Text="Save & Close" OnClick="btnSaveAndClose_Click" />
                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="parent.CloseIFrame();" />
                <asp:Label runat="server" ID="StatusLabel" EnableViewState="false"></asp:Label>
            </td>
        </tr>
    </table>
<asp:PlaceHolder ID="phBottom" runat="server"></asp:PlaceHolder>
