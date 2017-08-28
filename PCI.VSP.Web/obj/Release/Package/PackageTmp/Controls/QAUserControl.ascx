<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QAUserControl.ascx.cs" Inherits="PCI.VSP.Web.Controls.QAUserControl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!-- Fee Tier Methods -->
<script type="text/javascript">

    $(document).ready(function () {
        $('.FeeTierParent').each(function () {
            var FeeTierTotals = $(this).find('.FeeTierTotals');
            var FeeTiers = $(FeeTierTotals).val().split('\n');

            for (var i = 0; i < FeeTiers.length; i++) {
                if (FeeTiers[i] == '') continue;
                var FeeData = FeeTiers[i].split('|');

                $FeeTierDiv = $('<div class="FeeTierDiv"></div>');
                $(this).append($FeeTierDiv);

                $min = $('<input type="text" id="1" class="MinValue" onchange="buildFeeTierTotals(this);" value="' + FeeData[0] + '" />');
                $max = $('<input type="text" id="2" class="MaxValue" onchange="buildFeeTierTotals(this);" value="' + FeeData[1] + '" />');
                $fee = $('<input type="text" id="3" class="FeeValue" onchange="buildFeeTierTotals(this);" value="' + FeeData[2] + '" />');
                $removeTier = $('<button id="removeTier" onclick="removeFeeTier(this);" type="button" class="RemoveTierButton">Remove Tier</button>');

                $FeeTierDiv.append($('<div class="FeeTierLabel">Minimum:</div>'));
                $FeeTierDiv.append($min);
                $FeeTierDiv.append($('<br/>'));
                $FeeTierDiv.append($('<div class="FeeTierLabel">Maximum:</div>'));
                $FeeTierDiv.append($max);
                $FeeTierDiv.append($('<br/>'));
                $FeeTierDiv.append($('<div class="FeeTierLabel">Fee:</div>'));
                $FeeTierDiv.append($fee);
                $FeeTierDiv.append($removeTier);
                $FeeTierDiv.append($('<br/>'));
                $FeeTierDiv.append($('<hr/>'));
            }
        });
    });

    function addFeeTier(btn) {
        var FeeTierParent = $(btn).parent('.FeeTierParent');
        $FeeTierDiv = $('<div class="FeeTierDiv"></div>');
        $(FeeTierParent).append($FeeTierDiv);

        $min = $('<input type="text" id="1" class="MinValue" onchange="buildFeeTierTotals(this);" />');
        $max = $('<input type="text" id="2" class="MaxValue" onchange="buildFeeTierTotals(this);" />');
        $fee = $('<input type="text" id="3" class="FeeValue" onchange="buildFeeTierTotals(this);" />');
        $removeTier = $('<button id="removeTier" onclick="removeFeeTier(this);" type="button" class="RemoveTierButton">Remove Tier</button>');

        $FeeTierDiv.append($('<div class="FeeTierLabel">Minimum:</div>'));
        $FeeTierDiv.append($min);
        $FeeTierDiv.append($('<br/>'));
        $FeeTierDiv.append($('<div class="FeeTierLabel">Maximum:</div>'));
        $FeeTierDiv.append($max);
        $FeeTierDiv.append($('<br/>'));
        $FeeTierDiv.append($('<div class="FeeTierLabel">Fee:</div>'));
        $FeeTierDiv.append($fee);
        $FeeTierDiv.append($removeTier);
        $FeeTierDiv.append($('<br/>'));
        $FeeTierDiv.append($('<hr/>'));
    }

    function removeFeeTier(btn) {
        var parent = $(btn).parent().parent(".FeeTierParent");
        $(btn).parent().remove();
        conCat(parent);
    }

    function buildFeeTierTotals(txtbox) {
        conCat($(txtbox).parent().parent(".FeeTierParent"));
    }

    function conCat(feeTierParent) {
        $(feeTierParent).find('.FeeTierTotals').val("");
        $(feeTierParent).find('.FeeTierDiv').each(function () {
            var a = $(this).find('.MinValue').val();
            var b = $(this).find('.MaxValue').val();
            var c = $(this).find('.FeeValue').val();
            $(feeTierParent).find('.FeeTierTotals').val($(feeTierParent).find('.FeeTierTotals').val() + a + "|" + b + "|" + c + "\r\n");
        });

        if ($(feeTierParent).find('.FeeTierTotals').val().length > 0) {
            $(feeTierParent).find('.FeeTierTotals').val($(feeTierParent).find('.FeeTierTotals').val().substring(0, $(feeTierParent).find('.FeeTierTotals').val().length - 1));
        }
    }

</script>

<!-- Validation Methods -->
<script type="text/javascript">

    function Validate() {
        var valid = true;
        $('.FeeTierParent').each(function () {
            conCat($(this));
        });

        if (!Page_ClientValidate('Questions')) return false;
        showSavingStatus();
        return valid;
    }

    function MultiLineInteger_Validate(sender, args) {
        try {
            args.IsValid = false;
            var intMin = parseInt(sender.attributes["minValue"].value);
            var intMax = parseInt(sender.attributes["maxValue"].value);
            var sa = args.Value.split('\n');

            for (var i = 0; i < sa.length; i++) {
                var temp = sa[i];
                if (temp == null || temp == '') continue;

                var reg = /^\s*\d+\s*$/;
                if (reg.test(temp)) {
                    var tempint = parseInt(temp);
                    if (tempint < intMin || tempint > intMax) {
                        return;
                    }
                }
                else {
                    return;
                }
            }
        }
        catch (ex) { }

        args.IsValid = true;
    }

    function MultiLineDouble_Validate(sender, args) {
        try {
            args.IsValid = false;
            var dbMin = parseFloat(sender.attributes["minValue"].value);
            var dbMax = parseFloat(sender.attributes["maxValue"].value);
            var sa = args.Value.split('\n');

            for (var i = 0; i < sa.length; i++) {
                var temp = sa[i];
                if (temp == null || temp == '') continue;

                var reg = /^[-+]?\d+(\.\d+)?$/;
                if (reg.test(temp)) {
                    var tempint = parseFloat(temp);
                    if (tempint < dbMin || tempint > dbMax) {
                        return;
                    }
                }
                else {
                    return;
                }
            }
        }
        catch (ex) { }

        args.IsValid = true;
    }

    function FeeTierInt_Validate(sender, args) {
        try {
            args.IsValid = false;
            var FeeTiers = args.Value.split('\n');
            for (var i = 0; i < FeeTiers.length; i++) {
                var FeeData = FeeTiers[i].split('|');
                for (var j = 0; j < FeeData.length; j++) {
                    if (FeeData[j] == null || FeeData[j] == '') return;

                    if (j == 0 || j == 1) { // Minimum || Maximum Value
                        var reg = /^[-+]?\d+(\.\d+)?$/;
                        if (!reg.test(FeeData[j])) {
                            return;
                        }
                    }

                    if (j == 2) { // Fee Value
                        var reg = /^\s*\d+\s*$/;
                        if (!reg.test(FeeData[j])) {
                            return;
                        }
                    }
                }
            }
        }
        catch (ex) { }

        args.IsValid = true;
    }

    function FeeTierDouble_Validate(sender, args) {
        try {
            args.IsValid = false;
            var FeeTiers = args.Value.split('\n');
            for (var i = 0; i < FeeTiers.length; i++) {
                var FeeData = FeeTiers[i].split('|');
                for (var j = 0; j < FeeData.length; j++) {
                    if (FeeData[j] == null || FeeData[j] == '') return;
                    var reg = /^[-+]?\d+(\.\d+)?$/;
                    if (!reg.test(FeeData[j])) return;
                }
            }
        }
        catch (ex) { }

        args.IsValid = true;
    }

    function FeeTierMoney_Validate(sender, args) {
        try {
            args.IsValid = false;
            var FeeTiers = args.Value.split('\n');
            for (var i = 0; i < FeeTiers.length; i++) {
                var FeeData = FeeTiers[i].split('|');
                for (var j = 0; j < FeeData.length; j++) {
                    if (FeeData[j] == null || FeeData[j] == '') return;

                    if (j == 0 || j == 1) { // Minimum || Maximum Value
                        var reg = /^[-+]?\d+(\.\d+)?$/;
                        if (!reg.test(FeeData[j])) {
                            return;
                        }
                    }

                    if (j == 2) { // Fee Value
                        var reg = /^[0-9]+(,[0-9]{3})*(\.[0-9]{1,5})?$/;
                        if (!reg.test(FeeData[j])) {
                            return;
                        }
                    }
                }
            }
        }
        catch (ex) { }

        args.IsValid = true;
    }

</script>

<style type='text/css'>
    .blackOut2
    {
        position: absolute;
        top: 0px;
        left: 0px;
        width: 100%;
        height: 100%;
        background-color: black;
        filter: alpha(opacity=50);
        opacity: 0.50;
        -moz-opacity: 0.50;
        z-index: 103;
    }
    .ModalWindow2
    {
        position: absolute;
        border-top: solid 2px black;
        border-right: solid 2px black;
        border-bottom: solid 2px black;
        border-left: solid 2px black;
        border-radius: 15px;
        padding: 5px;
        background-color: #D1D1D1;
        z-index: 104;
        border-image: initial;
        margin: 0px auto;
    }
    
    .outerDiv
    {
        width: 100%; 
        height: 100%; 
        display: none; 
        position: absolute; 
        left: 0px; 
        top: 0px;
    }
    
    .innerDiv
    {
        width: 300px; 
        /*height: 125px; */
        left: 191.5px; 
        top: 121px;
    }
    
    .FeeTierTotals
    {
        display: none;
    }
    
    .AddTierButton
    {
        width: 70px;
    }
    
    .FeeTierLabel
    {
        width:60px;
        float:left;
        padding-top:4px;
    }
</style>

<script type="text/javascript">

    function showSavingStatus() {
        var statusLabel = $('<%= StatusLabel.ClientID %>');
        if (statusLabel == null || statusLabel == undefined) return;
        statusLabel.val('Saving...');
    }

    function HideCommentsModalWindow(reload) {
        $('body').css('overflow-y', 'auto');
        $('#CommentsMainModalWindow').css('display', 'none');
        if ($('#iCommentsWindow').get(0) != null)
            $('#iCommentsWindow').get(0).contentWindow.location.replace('about:blank');
        else if ($('#iCommentsWindow', parent.document).get(0) != null)
            $('#iCommentsWindow', parent.document).get(0).contentWindow.location.replace('about:blank');
        $('body', parent.document).css('overflow-y', 'auto');
        $('#CommentsMainModalWindow', parent.document).css('display', 'none');
        if (reload) {
            if (window.parent != null) {
                window.parent.location.replace(window.parent.location.href);
            }
            else if (parent != null) {
                parent.location.replace(parent.location.href);
            }
            else {
                window.location.replace(window.location.href);
            }
        }
    }

    function launchCommentWindow(entityId, commentType) {
        // var entityType = $('#<%= EntityTypeHiddenField.ClientID %>').val();
        //  var url = '<%= ResolveClientUrl("~/Vendor/CommentsDialog.aspx")%>?EntityType=' + entityType + '&EntityId=' + entityId + '&CommentType=' + commentType;
        var url = '<%= ResolveClientUrl("~/Vendor/CommentsDialog.aspx")%>?EntityId=' + entityId + '&CommentType=' + commentType;
        ShowModalCommentsWindow(url, 233, 440);
    }

    function launchPCICommentWindow(entityId, commentType) {
        //  var entityType = $('#<%= EntityTypeHiddenField.ClientID %>').val();
        //var url = '<%= ResolveClientUrl("~/crmiframes/PCIComments.aspx") %>?EntityType=' + entityType + '&EntityId=' + entityId + '&CommentType=' + commentType;
        var url = '<%= ResolveClientUrl("~/crmiframes/PCIComments.aspx") %>?EntityId=' + entityId + '&CommentType=' + commentType;
        ShowModalCommentsWindow(url, 233, 440);
    }

    function ShowModalCommentsWindow(url, height, width) {
        debugger;
        $('body').css('overflow-y', 'hidden');
        $("div#CommentsModalWindow").css({ 'height': height + 5, 'width': width + 5 });
        $("iframe#iCommentsWindow").css({ 'height': height, 'width': width });
        $("iframe#iCommentsWindow").css({ 'border': "0px solid #D1D1D1" });
        $("iframe#iCommentsWindow").get(0).contentWindow.location.replace(url);
        $("div#CommentsMainModalWindow").css('display', 'block');
        CenterCommentsWindow();
    }

    function CenterCommentsWindow() {
        try {
            var pageHeight = $(window).height();//top.document.documentElement.clientHeight;
            var pageWidth = $(window).width();//top.document.documentElement.clientWidth;
            var windowHeight = 125;// parseInt(document.getElementById('ModalWindow').style.height, 10);
            var windowWidth = 280;//parseInt(document.getElementById('ModalWindow').style.width, 10);

            document.getElementById('CommentsModalWindow').style.top = '100px';//(pageHeight - windowHeight+5) / 2 + "px";
            document.getElementById('CommentsModalWindow').style.left = (pageWidth - windowWidth - 300) / 2 + "px";
            // parent.scrollTo(0, 0); not working in crm 2011
            $("html,body").scrollTop(0);
        }
        catch (err) {
            alert("An error occured in the DOM " + err + ".");
            // return;
        }
    }

    function displayFiles(id) {
        $('.' + id).show();
    }
</script>
<asp:HiddenField ID="EntityTypeHiddenField" runat="server" Value=""/>

<table>
    <tr>
        <td>
            <div style="width: 800px; height: 710px; overflow: auto;">
                <asp:ValidationSummary ID="QuestionValidationSummary" runat="server" ValidationGroup="Questions" ShowSummary="true" HeaderText="To save the questions, please correct the following answers:" EnableClientScript="true" DisplayMode="List" ForeColor="Red" />
                <asp:Repeater ID="rptQuestions" runat="server" Visible="true" OnItemDataBound="rptQuestions_ItemDataBound">
                    <HeaderTemplate>
                        <table style="width: 100%;">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr><td style="width: 10px;"><asp:Literal ID="litNumber" runat="server" /></td><td style="text-align: left;"><%# Eval("AccountWording")%></td><td></td></tr><tr><td colspan="2" style="text-align: left;"><table><tr><td><asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder></td><td style="width:400px; vertical-align:top;"><asp:PlaceHolder ID="phOther" runat="server"></asp:PlaceHolder><asp:HiddenField ID="hfId" runat="server" /></td></tr></table></td></tr><tr><td colspan="2" style="width: 10px; text-align: left;"><asp:Literal ID="LiteralError" runat="server" /></td></tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="Validate();" />
            <asp:Button ID="btnSaveAndClose" runat="server" Text="Save & Close" OnClientClick="Validate();" OnClick="btnSaveAndClose_Click" />
            <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="parent.CloseIFrame();" />
            <asp:Label runat="server" ID="StatusLabel" EnableViewState="false"></asp:Label>
        </td>
    </tr>
</table>

<div id="CommentsMainModalWindow" style="width: 100%; height: 100%; display: none; position: absolute; left: 0px; top: 0px;">
    <div class="blackOut"></div>
    <div id="CommentsModalWindow" class="ModalWindow" style="width: 280px; height: 125px; left: 191.5px; top: 21px;">
        <iframe id="iCommentsWindow" src="about:blank" height="125px" width="280px" frameborder="0"></iframe>
    </div>
</div>


