<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FilterPhase2.aspx.cs" Inherits="PCI.VSP.Web.CrmIFrames.FilterPhase2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>VSP Filter</title>
</head>
<body>
    <form id="FilterPhase2Form" runat="server">
    <script type="text/javascript">
        function showStatusLabel() {
            document.getElementById('StatusSpan').style.visibility = 'visible';
        }
    </script>
    <div>
        <table>
            <tr>
                <td>
                    <asp:Button ID="PerformFilterButton" runat="server" Text="Perform Filter" OnClick="PerformFilterButton_Click"
                        OnClientClick="showStatusLabel();" />
                </td>
            </tr>
            <tr>
                <td>
                    <span id="StatusSpan" style="visibility: hidden;">Please wait... this may take a few minutes. </span>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="FilterResultsTextBox" runat="server" Visible="false" ReadOnly="True"
                        TextMode="MultiLine" ViewStateMode="Disabled" Height="573px" Width="715px"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
