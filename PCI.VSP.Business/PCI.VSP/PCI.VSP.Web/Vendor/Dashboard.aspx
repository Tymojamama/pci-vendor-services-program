<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="PCI.VSP.Web.Vendor.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" id="GlobalScript">
        function CloseIFrame() {
            HideModalWindow(true);
        }
    </script>
    <script src="../Scripts/ModalWindow.js" type="text/javascript"></script>
    <script type="text/javascript">
        function launchModalWindow(windowToOpen, entityId) {
            var url = null;
            var windowName = null;

            switch (windowToOpen) {
                case 'VendorProductQuestions':
                    url = '<%= ResolveClientUrl("~/Vendor/VendorProductQuestionsDialog.aspx") %>' + '?VendorProductId=' + entityId.toString();
                    windowName = 'Vendor Product Questions';
                    break;
                case 'ProjectInquiries':
                    url = '<%= ResolveClientUrl("~/Vendor/ProjectInquiriesDialog.aspx") %>' + '?ProjectVendorId=' + entityId.toString();
                    windowName = 'Project Inquiries';
                    break;
                default:
                    return;
            }

            ShowModalWindow(url, 768, 1024);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="NavContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#DashboardNavImg").css('background', 'url(<%= ResolveClientUrl("~/images/selectedbutton.png") %>)');
        });
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="small-width">
        <div class="contentheader">
            <div class="contentheadertext">
                Welcome to Your VSP Dashboard
            </div>
            <div class="contentheadersubtext">
                Your profile is <asp:Label runat="server" ID="lblPercentProfileComplete"></asp:Label> complete
            </div>
        </div>
        <div class="contentsubheader">
            Products Summary
        </div>
        <div class="contentgridviewcontainer">
            <asp:GridView ID="VendorProductGridView" runat="server" AutoGenerateColumns="False"
                EnableViewState="False" Width="100%" CellPadding="4" ForeColor="Black" BackColor="White"
                PageSize="3" BorderColor="#E0DCD9" HeaderStyle-CssClass="table-header">
                <Columns>
                    <asp:BoundField HeaderText="Product" DataField="ProductName" />
                    <asp:BoundField HeaderText="Modified On" DataField="LastUpdated" DataFormatString="{0:d}" />
                    <asp:BoundField HeaderText="% Complete" DataField="PercentComplete" DataFormatString="{0:P0}"
                        ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Center" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <a href="#" onclick='launchModalWindow("VendorProductQuestions", "<%# Eval("VendorProductId") %>");'
                                class="nodecoration">Edit</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Left" />
                <RowStyle HorizontalAlign="Left" />
            </asp:GridView>
        </div>
        <div class="view-more-container">
            <div class="ViewMore" onclick="location.replace('<%= ResolveClientUrl("Products.aspx") %>')">
                <p>
                    More >>
                </p>
            </div>
        </div>
        <div class="contentsubheader">
            Project Inquiries Summary
        </div>
        <div class="contentgridviewcontainer">
            <asp:GridView ID="ClientInquiriesGridView" runat="server" EnableViewState="False" AutoGenerateColumns="False" Width="100%" CellPadding="4"
                ForeColor="Black" BackColor="White" PageSize="3" BorderColor="#E0DCD9" HeaderStyle-CssClass="table-header">
                <Columns>
                    <asp:BoundField HeaderText="Product" DataField="ProductName" />
                    <asp:BoundField HeaderText="Project" DataField="ClientProjectName" />
                    <asp:BoundField HeaderText="Last Updated" DataField="LastUpdated" DataFormatString="{0:d}" />
                    <asp:BoundField HeaderText="Status" DataField="Status" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <a href="#" class="nodecoration" onclick='launchModalWindow("ProjectInquiries", "<%# Eval("ProjectVendorId") %>");'>
                                Respond
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Left" />
                <RowStyle HorizontalAlign="Left" />
            </asp:GridView>
        </div>
        <div class="view-more-container">
            <div class="ViewMore" onclick="location.replace('<%= ResolveClientUrl("ProjectInquiries.aspx") %>')">
                <p>
                    More >>
                </p>
            </div>
        </div>
    </div>
</asp:Content>
