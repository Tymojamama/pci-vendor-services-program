<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ProjectInquiries.aspx.cs" Inherits="PCI.VSP.Web.Vendor.ProjectInquiries" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" id="GlobalScript">
        function CloseIFrame() {
            HideModalWindow(true);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="NavContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    
    <script type="text/javascript">
        function launchQnaForm(projectVendorId) {
            var url = 'ProjectInquiriesDialog.aspx?ProjectVendorId=' + projectVendorId.toString();
            ShowModalWindow(url, 768, 1024);
        }
    </script>
    <div class="small-width">
        <div class="contentheader">
            <div class="contentheadertext">
                Project Inquiries
            </div>
            <div class="contentheadersubtext">
                Respond to inquiries
            </div>
        </div>
        <div class="contentsubheader">
            Project Inquiries
        </div>
        <div class="contentgridviewcontainer">
            <asp:GridView ID="ClientInquiriesGridView" runat="server" HorizontalAlign="Right"
                EnableViewState="False" AutoGenerateColumns="False" Width="100%" CellPadding="4"
                ForeColor="Black" BackColor="White" PageSize="3" BorderColor="#E0DCD9" HeaderStyle-CssClass="table-header">
                <Columns>
                    <asp:BoundField HeaderText="Product" DataField="ProductName" />
                    <asp:BoundField HeaderText="Last Updated" DataField="LastUpdated" DataFormatString="{0:d}" />
                    <asp:BoundField HeaderText="Status" DataField="Status" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <a href="#" onclick='launchQnaForm("<%# Eval("ProjectVendorId") %>");'>Respond</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Left" />
                <RowStyle HorizontalAlign="Left" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
