<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PCIComments.aspx.cs" Inherits="PCI.VSP.Web.CrmIFrames.PCIComments" %>
<%@ Register Src="../Controls/Comment.ascx" TagName="Comment" TagPrefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
</head>
<body style='background-image: url(<%= ResolveClientUrl("~/images/background.png") %>);'>
    <form id="CommentsDialog" runat="server">
    <div>
        <uc1:Comment ID="Comment1" runat="server" />
    </div>
    </form>
</body>
</html>
