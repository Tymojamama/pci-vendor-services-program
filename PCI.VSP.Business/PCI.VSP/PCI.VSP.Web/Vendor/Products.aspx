<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Products.aspx.cs" Inherits="PCI.VSP.Web.Vendor.Products" %>

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
    <script src="../Scripts/ModalWindow.js" type="text/javascript"></script>
    <script type="text/javascript">
        function launchQnaForm(projectVendorId) {
            var url = 'VendorProductQuestionsDialog.aspx?VendorProductId=' + projectVendorId.toString();
            ShowModalWindow(url, 768, 1024);
        }
    </script>
    <div class="small-width">
        <div class="contentheader">
            <div class="contentheadertext">
                Vendor Products
            </div>
            <div class="contentheadersubtext">
                Complete your product profiles
            </div>
        </div>
        <div class="contentsubheader">
            Products Summary
        </div>
        <div style="vertical-align: top; width: 100%;">
            <table style="vertical-align: top; width: 100%;">
                <tr>
                    <td>
                        <asp:Repeater ID="Repeater1" runat="server">
                            <HeaderTemplate>
                                <div class="contentgridviewcontainer">
                                    <table id="repeatertable" style="margin: 0px auto; width: 100%; background-color: White; color: Black; text-align: center;" cellpadding="4" cellspacing="0">
                                        <tr class="table-header">
                                            <td>
                                                <b>Product</b>
                                            </td>
                                            <td>
                                                <b>Last Updated</b>
                                            </td>
                                            <td>
                                                <b>Updated By</b>
                                            </td>
                                            <td>
                                                <b>% Complete </b>
                                            </td>
                                            <td>
                                                <b>Action</b>
                                            </td>
                                        </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <%# Eval("ProductName")%>
                                    </td>
                                    <td>
                                        <%#Eval("LastUpdated","{0:d}")%>
                                    </td>
                                    <td>
                                        <%#Eval("LastUpdatedBy")%>
                                    </td>
                                    <td>
                                        <%#Eval("PercentComplete", "{0:P0}")%>
                                    </td>
                                    <td>
                                        <a onclick='launchQnaForm("<%#Eval("VendorProductId") %>");' href="#">Edit</a>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table> </div>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#ProductsNavImg").css('background', 'url(<%= ResolveClientUrl("~/images/selectedbutton.png") %>)');
        });
    </script>
</asp:Content>
