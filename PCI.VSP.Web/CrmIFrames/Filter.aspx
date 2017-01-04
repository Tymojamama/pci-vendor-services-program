<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Filter.aspx.cs" Inherits="PCI.VSP.Web.CrmIFrames.Filter" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>VSP Filter</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--<form id="FilterForm" runat="server">--%>
    <script type="text/javascript">
        function processClick() {
            var hiddenField = document.getElementById('<%= Filter1RunHiddenField.ClientID %>');
            if (hiddenField != null) {
                if (hiddenField.value == 'true') {
                    if (!confirm('A phase 1 filter has already been run for this project. Do you wish to re-run the filter?'))
                        return false;
                }
            }
            document.getElementById('StatusSpan').style.visibility = 'visible';
            return true;
        }
    </script>
    <div runat="server" id="filterFormError">
        <div class="small-width">
            <div class="contentheader">
                <div class="contentheadertext">
                    Whoops!
                </div>
                <div class="contentheadersubtext">
                    It looks like an error occurred loading this page.
                </div>
            </div>
        </div>
    </div>
    <div runat="server" id="filterForm">
        <div class="small-width">
            <div class="contentheader">
                <div class="contentheadertext">
                    Filter - Phase 1
                </div>
                <div class="contentheadersubtext">
                    Click the button below to perform the phase one filter.
                </div>
            </div>
            <div class="contentsubheader">
                <asp:HiddenField ID="Filter1RunHiddenField" runat="server" />
                <asp:Button ID="PerformFilterButton" runat="server" Text="Perform Filter" OnClick="PerformFilterButton_Click" OnClientClick="return processClick();" />
                <span id="StatusSpan" style="visibility: hidden;">Please wait... this may take a few minutes. </span>
                <asp:TextBox ID="FilterResultsTextBox" runat="server" Visible="false" ReadOnly="True" TextMode="MultiLine" ViewStateMode="Disabled" Height="573px" Width="715px"></asp:TextBox>
            </div>
        </div>
<%--        <table>
            <tr>
                <td>
                    <asp:HiddenField ID="Filter1RunHiddenField" runat="server" />
                    <asp:Button ID="PerformFilterButton" runat="server" Text="Perform Filter" OnClick="PerformFilterButton_Click" OnClientClick="return processClick();" />
                </td>
            </tr>
            <tr>
                <td>
                    <span id="StatusSpan" style="visibility: hidden;">Please wait... this may take a few minutes. </span>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="FilterResultsTextBox" runat="server" Visible="false" ReadOnly="True" TextMode="MultiLine" ViewStateMode="Disabled" Height="573px" Width="715px"></asp:TextBox>
                </td>
            </tr>
        </table>--%>
    </div>
    <%--</form>--%>
</asp:Content>