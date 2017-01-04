<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommentsDialog.aspx.cs" Inherits="PCI.VSP.Web.Vendor.CommentsDialog" %>
<%@ Register Src="../Controls/Comment.ascx" TagName="Comment" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
