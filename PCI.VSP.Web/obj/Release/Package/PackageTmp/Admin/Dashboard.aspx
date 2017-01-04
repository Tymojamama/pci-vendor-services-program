<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="PCI.VSP.Web.Admin.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="NavContent" runat="server">
    <div style="text-align:left;">
Dashboard<br />
Question Management<br />
Template Management<br />
User Management<br />
Projects<br />
<div style="padding-left: 15px;">
Client Questions<br />
Vendor Filtering<br />
Search Results<br />
Benchmarks<br />
</div>
Vendors<br />
<div style="padding-left: 15px;">
Profile<br />
Products<br />
Client Inquires<br />
User Management<br />
</div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<h2>Active Projects:</h2>
<table style="margin: 0px auto;" border="1px" cellpadding="3" cellspacing="0">
<tr>
<th>Project</th>
<th>Last Updated</th>
<th>Current Step</th>
<th>View</th>
</tr>
<tr>
<td>Company A - 401K Benefit</td>
<td>6/17/2012 1:13PM</td>
<td align="center">Vendor Responses</td>
<td><a href="#">View</a></td>
</tr>
<tr>
<td>Company B - 401K Benefit</td>
<td>6/12/2012 12:13PM</td>
<td align="center">Vendor Exclusions</td>
<td><a href="#">View</a></td>
</tr>
<tr>
<td>Company C - 401K Benefit</td>
<td>6/09/2012 11:13AM</td>
<td align="center">Vendor Responses</td>
<td><a href="#">View</a></td>
</tr>
<tr>
<td>Company 1 - Life Insurance</td>
<td>5/21/2012 3:45PM</td>
<td align="center">System Search</td>
<td><a href="#">View</a></td>
</tr>
<tr>
<td>Company 2 - Life Insurance</td>
<td>5/11/2012 3:45PM</td>
<td align="center">Client Interview</td>
<td><a href="#">View</a></td>
</tr>
<tr>
<td>Company 3 - Life Insurance</td>
<td>5/1/2012 3:45PM</td>
<td align="center">Second Filter</td>
<td><a href="#">View</a></td>
</tr>
<tr>
<td>Other Project</td>
<td>1/1/2012 12:00AM</td>
<td align="center">Client Review</td>
<td><a href="#">View</a></td>
</tr>
</table>

</asp:Content>
