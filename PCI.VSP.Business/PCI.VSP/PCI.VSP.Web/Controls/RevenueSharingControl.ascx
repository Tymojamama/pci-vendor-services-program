<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RevenueSharingControl.ascx.cs"
    Inherits="PCI.VSP.Web.Controls.RevenueSharingControl" %>
<script type="text/javascript">
    function RevenueSharingControl_OnChanged(totalControl, bpsControl, serviceFeeControl, findersFeeControl, otherControl) {
        var total = 0.0;
        var bps = $('#' + bpsControl).val();
        var serviceFee = $('#' + serviceFeeControl).val();
        var findersFee = $('#' + findersFeeControl).val();
        var otherRevenue = $('#' + otherControl).val();

        if (parseFloat(bps) != NaN)
            total += parseFloat(bps);
        if (parseFloat(serviceFee) != NaN)
            total += parseFloat(serviceFee);
//        if (parseFloat(findersFee) != NaN)
//            total += parseFloat(findersFee);
        if (parseFloat(otherRevenue) != NaN)
            total += parseFloat(otherRevenue);

        if (parseFloat(total) != NaN)
            $('#' + totalControl).val(total.toFixed(5));
        else
            $('#' + totalControl).val('NaN');
    }
</script>
<table>
<asp:Repeater ID="rptQuestions" runat="server" Visible="true" OnItemDataBound="rptQuestions_ItemDataBound">
    <ItemTemplate>
        <tr>
            <td>Investment Name</td>
            <td>
                <asp:TextBox ID="AssetFundTextBox" runat="server" Text='<%# Eval("AssetFund") %>' ReadOnly="true" Enabled="false" Style="width: 225px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Investment ID</td>
            <td>
                <asp:TextBox ID="AssetSymbolTextBox" runat="server" Text='<%# Eval("AssetSymbol") %>' ReadOnly="true" Enabled="false" Style="width: 85px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Sub-ta Calculation Method</td>
            <td>
                <asp:DropDownList ID="CalculationTypeDropDownList" runat="server">
                    <asp:ListItem Text="Basis Points" Value="1" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Both" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Greater Than" Value="3"></asp:ListItem>
                    <asp:ListItem Text="Lesser Of" Value="4"></asp:ListItem>
                    <asp:ListItem Text="N/A" Value="5"></asp:ListItem>
                    <asp:ListItem Text="Per Head" Value="6"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>Sub-ta stated in basis points</td>
            <td>
                <asp:TextBox ID="CurrentBpsTextBox" runat="server" Text='<%# Eval("CurrentBps", "{0:n5}") %>'></asp:TextBox>
                <asp:RangeValidator ID="CurrentBpsRangeValidator" runat="server" ErrorMessage="Please enter a value 0 to 100." ControlToValidate="CurrentBpsTextBox" MaximumValue="100" MinimumValue="0" SetFocusOnError="True" Type="Double" ValidationGroup="RevenueSharing"></asp:RangeValidator>
                <asp:CompareValidator ID="CurrentBpsCompareValidator" runat="server" ErrorMessage="Please enter a number." Operator="DataTypeCheck" Type="Double" ValidationGroup="RevenueSharing" ControlToValidate="CurrentBpsTextBox"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td>Sub-ta stated as per head</td>
            <td>
                <asp:TextBox ID="txtCurrentPerHeadSubta" runat="server" Text='<%# Eval("CurrentPerHeadSubta", "{0:n2}") %>'></asp:TextBox>
                <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="Please enter a value -1000000000.00 to 1000000000.00." ControlToValidate="txtCurrentPerHeadSubta" SetFocusOnError="True" ValidationGroup="RevenueSharing" MaximumValue="1000000000.00" MinimumValue="-1000000000.00" Type="Double"></asp:RangeValidator>
            </td>
        </tr>
        <tr>
            <td>Service Fee or 12b-1 stated in basis points</td>
            <td>
                <asp:TextBox ID="CurrentServiceFeeTextBox" runat="server" Text='<%# Eval("CurrentServiceFee", "{0:n5}") %>'></asp:TextBox>
                <asp:RangeValidator ID="CurrentServiceFeeRangeValidator" runat="server" ErrorMessage="Please enter a value 0 to 100." ControlToValidate="CurrentServiceFeeTextBox" SetFocusOnError="True" ValidationGroup="RevenueSharing" MaximumValue="100" MinimumValue="0" Type="Double"></asp:RangeValidator>
                <asp:CompareValidator ID="CurrentServiceFeeCompareValidator" runat="server" ErrorMessage="Please enter a number." Operator="DataTypeCheck" Type="Double" ValidationGroup="RevenueSharing" ControlToValidate="CurrentServiceFeeTextBox"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td>Other asset-based revenue sharing stated in basis points</td>
            <td>
                <asp:TextBox ID="OtherRevenueSharingTextBox" runat="server" Text='<%# Eval("OtherRevenueSharing", "{0:n5}") %>'></asp:TextBox>
                <asp:RangeValidator ID="OtherRevenueSharingRangeValidator" runat="server" ErrorMessage="Please enter a value 0 to 100." ControlToValidate="OtherRevenueSharingTextBox" SetFocusOnError="True" ValidationGroup="RevenueSharing" MaximumValue="100" MinimumValue="0" Type="Double"></asp:RangeValidator>
                <asp:CompareValidator ID="OtherRevenueSharingCompareValidator" runat="server" ErrorMessage="Please enter a number." ControlToValidate="OtherRevenueSharingTextBox" Operator="DataTypeCheck" Type="Double" ValidationGroup="RevenueSharing"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td>Total Asset-Based Revenue Sharing</td>
            <td>
                <asp:TextBox ID="TotalRevenueSharingTextBox" runat="server" ReadOnly="true" Enabled="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Wrap fee (charged at investment level by plan)</td>
            <td>
                <asp:TextBox ID="txtAssetBasedAdministrativeFee" runat="server" Text='<%# Eval("AssetBasedAdministrativeFee", "{0:n5}") %>'></asp:TextBox>
                <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="Please enter a value 0.00000 to 1,000,000,000.00000." ControlToValidate="txtAssetBasedAdministrativeFee" SetFocusOnError="True" ValidationGroup="RevenueSharing" MaximumValue="1000000000.00000" MinimumValue="0.00000" Type="Double"></asp:RangeValidator>
            </td>
        </tr>
        <tr>
            <td>Deposit-based commission</td>
            <td>
                <asp:TextBox ID="CurrentFindersFeeTextBox" runat="server" Text='<%# Eval("CurrentFindersFee", "{0:n5}") %>'></asp:TextBox>
                <asp:RangeValidator ID="CurrentFindersFeeRangeValidator" runat="server" ErrorMessage="Please enter a value 0 to 100." ControlToValidate="CurrentFindersFeeTextBox" SetFocusOnError="True" ValidationGroup="RevenueSharing" MaximumValue="100" MinimumValue="0" Type="Double"></asp:RangeValidator>
                <asp:CompareValidator ID="CurrentFindersFeeCompareValidator" runat="server" ErrorMessage="Please enter a number." Operator="DataTypeCheck" Type="Double" ValidationGroup="RevenueSharing" ControlToValidate="CurrentFindersFeeTextBox"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td>Vendor Comment</td>
            <td>
                <asp:TextBox ID="VendorCommentTextBox" runat="server" Text='<%# Eval("RevenueSharingVendorComment") %>'></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:HiddenField ID="hfId" runat="server" />
            </td>
        </tr>
    </ItemTemplate>
    <SeparatorTemplate>
        <tr><td colspan="2"><hr /></td></tr>
    </SeparatorTemplate>
</asp:Repeater>
        <tr id="trFooter" runat="server">
            <td colspan="2">
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
                <asp:Button ID="btnSaveAndClose" runat="server" Text="Save & Close" OnClick="btnSaveAndClose_Click" />
                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="parent.CloseIFrame();" />
                <asp:Label runat="server" ID="StatusLabel" EnableViewState="false"></asp:Label>
            </td>
        </tr>
    </table>
