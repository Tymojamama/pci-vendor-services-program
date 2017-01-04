<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuestionSetup.aspx.cs"
    Inherits="PCI.VSP.Web.CrmIFrames.QuestionSetup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function GetQuestionDataType() {
            return '1';
//            return $('#<%= QuestionTypeDropDownList.ClientID %>').val();
        }

        function GetClientAnswerType() {
            return $('#<%= ClientResponseTypeDropDownList.ClientID %>').val();
        }

        function GetVendorAnswerType() {
            return $('#<%= VendorResponseTypeDropDownList.ClientID %>').val();
        }

        function GetComparisonType() {
            return $('#<%= ComparisonTypeDropDownList.ClientID %>').val();
        }
    </script>
</head>
<body>
    <form id="QuestionSetupForm" runat="server">
    <script type="text/javascript" src='<%= ResolveClientUrl("~/scripts/jquery-1.7.2.min.js") %>'></script>
    <input type="hidden" runat="server" id="hdCommand" />
    <!-- question data type, client response type and vendor response type -->
    <div>
        <table>
            <tr>
                <td>
                    Question Type
                </td>
                <td>
                    <asp:DropDownList ID="QuestionTypeDropDownList" runat="server" 
                        DataTextField="Name" DataValueField="Value" AutoPostBack="true" 
                        onselectedindexchanged="QuestionTypeDropDownList_SelectedIndexChanged">
                    </asp:DropDownList>
                    <input id="QuestionTypeHiddenField" type="hidden" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Client Response Type
                </td>
                <td>
                    <asp:DropDownList ID="ClientResponseTypeDropDownList" runat="server" 
                        DataTextField="Name" DataValueField="Value" AutoPostBack="true" 
                        onselectedindexchanged="ClientResponseTypeDropDownList_SelectedIndexChanged">
                    </asp:DropDownList>
                    <input id="ClientResponseTypeHiddenField" type="hidden" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Vendor Response Type
                </td>
                <td>
                    <asp:DropDownList ID="VendorResponseTypeDropDownList" runat="server" 
                        DataTextField="Name" DataValueField="Value" AutoPostBack="true" 
                        onselectedindexchanged="VendorResponseTypeDropDownList_SelectedIndexChanged">
                    </asp:DropDownList>
                    <input id="VendorResponseTypeHiddenField" type="hidden" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Comparison Type
                </td>
                <td>
                    <asp:DropDownList ID="ComparisonTypeDropDownList" runat="server" DataTextField="Name" DataValueField="Value">
                    </asp:DropDownList>
                    <input id="ComparisonTypeHiddenField" type="hidden" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
//        $(document).ready(function () {
//            //            alert('Question Type: ' + GetQuestionDataType().toString());
//            //            alert('Client Answer Type: ' + GetClientAnswerType().toString());
//            //            alert('Vendor Answer Type: ' + GetVendorAnswerType().toString());
//            //            alert('Comparison Type: ' + GetComparisonType().toString());
////            alert(parent.document.getElementById('vsp_questiondatatype'));
////            alert(parent.document.getElementById('vsp_questiondatatype').DataValue);
        //        });

        alert(parent.document.forms[0].all.vsp_questiondatatype.DataValue);

//        alert(parent);
//        alert(parent.document);
//        alert(parent.document.getElementById('vsp_questiondatatype'));
//        alert(parent.document.getElementById('vsp_questiondatatype').DataValue);
    </script>
    </form>
</body>
</html>
