<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="PCI.VSP.Web.Error" %>

<asp:Content ID="Navigation" runat="server" ContentPlaceHolderID="NavContent"></asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="small-width">
        <div class="contentheader">
            <div class="contentheadertext">
                Oh no! An unknown error occurred.
            </div>
            <div class="contentheadersubtext">
                Please contact our internal IT department.
            </div>
        </div>
    </div>
</asp:Content>