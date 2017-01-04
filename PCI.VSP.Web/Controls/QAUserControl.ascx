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

                var $FeeTierDiv = $('<div class="FeeTierDiv"></div>');
                $(this).append($FeeTierDiv);

                $min = $('<input type="text" id="1" class="MinValue" onchange="buildFeeTierTotals(this);" value="' + FeeData[0] + '" />');
                $max = $('<input type="text" id="2" class="MaxValue" onchange="buildFeeTierTotals(this);" value="' + FeeData[1] + '" />');
                $fee = $('<input type="text" id="3" class="FeeValue" onchange="buildFeeTierTotals(this);" value="' + FeeData[2] + '" />');
                $removeTier = $('<button id="removeTier" onclick="removeFeeTier(this);" type="button" class="RemoveTierButton">Remove Tier</button>');

                $FeeTierDiv.append($('<div class="FeeTierLabel">Minimum:</div>'));
                $FeeTierDiv.append($min);
                $FeeTierDiv.append($('<div class="FeeTierLabel">Maximum:</div>'));
                $FeeTierDiv.append($max);
                $FeeTierDiv.append($('<div class="FeeTierLabel">Fee:</div>'));
                $FeeTierDiv.append($fee);
                $FeeTierDiv.append($removeTier);
            }
        });
    });

    function addFeeTier(btn) {
        var FeeTierParent = $(btn).parent('.FeeTierParent');
        var $FeeTierDiv = $('<div class="FeeTierDiv"></div>');
        $(FeeTierParent).append($FeeTierDiv);

        $min = $('<input type="text" id="1" class="MinValue" onchange="buildFeeTierTotals(this);" />');
        $max = $('<input type="text" id="2" class="MaxValue" onchange="buildFeeTierTotals(this);" />');
        $fee = $('<input type="text" id="3" class="FeeValue" onchange="buildFeeTierTotals(this);" />');
        $removeTier = $('<button id="removeTier" onclick="removeFeeTier(this);" type="button" class="RemoveTierButton">Remove Tier</button>');

        $FeeTierDiv.append($('<div class="FeeTierLabel">Minimum:</div>'));
        $FeeTierDiv.append($min);
        $FeeTierDiv.append($('<div class="FeeTierLabel">Maximum:</div>'));
        $FeeTierDiv.append($max);
        $FeeTierDiv.append($('<div class="FeeTierLabel">Fee:</div>'));
        $FeeTierDiv.append($fee);
        $FeeTierDiv.append($removeTier);
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
        var url = '<%= ResolveClientUrl("~/Vendor/CommentsDialog.aspx")%>?EntityId=' + entityId + '&CommentType=' + commentType;
        ShowModalCommentsWindow(url, 233, 440);
    }

    function launchPCICommentWindow(entityId, commentType) {
        var url = '<%= ResolveClientUrl("~/crmiframes/PCIComments.aspx") %>?EntityId=' + entityId + '&CommentType=' + commentType;
        ShowModalCommentsWindow(url, 233, 440);
    }

    function ShowModalCommentsWindow(url, height, width) {
        debugger;
        $("div#CommentsModalWindow").css({ 'height': height + 5, 'width': width + 5 });
        $("iframe#iCommentsWindow").css({ 'height': height, 'width': width });
        $("iframe#iCommentsWindow").css({ 'border': "0px solid #D1D1D1" });
        $("iframe#iCommentsWindow").get(0).contentWindow.location.replace(url);
        $("div#CommentsMainModalWindow").css('display', 'block');
        CenterCommentsWindow();
    }

    function CenterCommentsWindow() {
        try {
            var pageHeight = $(window).height();
            var pageWidth = $(window).width();
            var windowHeight = 125;
            var windowWidth = 280;

            document.getElementById('CommentsModalWindow').style.top = '100px';
            document.getElementById('CommentsModalWindow').style.left = (pageWidth - windowWidth - 300) / 2 + "px";
        }
        catch (err) {
            alert("An error occured in the DOM " + err + ".");
        }
    }

    function displayFiles(id) {
        $('.' + id).show();
    }
</script>
<asp:HiddenField ID="EntityTypeHiddenField" runat="server" Value=""/>

<div style="width: 100%;">
    <asp:ValidationSummary ID="QuestionValidationSummary" runat="server" ValidationGroup="Questions" ShowSummary="true" HeaderText="To save the questions, please correct the following answers:" EnableClientScript="true" DisplayMode="List" ForeColor="Red" />
    <asp:Repeater ID="rptQuestions" runat="server" Visible="true" OnItemDataBound="rptQuestions_ItemDataBound">
        <ItemTemplate>
            <div class="question-container">
                <div class="question-number">
                    <p><asp:Literal ID="litNumber" runat="server" /></p>
                </div>
                <div class="question-subcontainer">
                    <div class="question-wording">
                        <p><%# Eval("AccountWording")%></p>
                    </div>
                    <div>
                        <div class="question-response">
                            <asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>
                        </div>
                        <div>
                            <asp:PlaceHolder ID="phOther" runat="server"></asp:PlaceHolder>
                            <asp:HiddenField ID="hfId" runat="server" />
                        </div>
                    </div>
                    <div>
                        <div style="width: 10px; text-align: left;">
                            <asp:Literal ID="LiteralError" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
<div>
    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="Validate();" CssClass="button" />
    <asp:Button ID="btnSaveAndClose" runat="server" Text="Save & Close" OnClientClick="Validate();" OnClick="btnSaveAndClose_Click" CssClass="button" />
    <asp:Label runat="server" ID="StatusLabel" EnableViewState="false"></asp:Label>
</div>

<div id="CommentsMainModalWindow" style="width: 100%; height: 100%; display: none; position: fixed; left: 0px; top: 0px;">
    <div class="blackOut"></div>
    <div id="CommentsModalWindow" class="ModalWindow" style="width: 280px; height: 125px; max-width: 95%;">
        <iframe id="iCommentsWindow" src="about:blank" height="125px" width="280px" frameborder="0"></iframe>
    </div>
</div>