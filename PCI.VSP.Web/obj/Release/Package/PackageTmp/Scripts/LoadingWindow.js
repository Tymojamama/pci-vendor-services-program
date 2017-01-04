//window.onresize = centerWindow;
function centerLoadingWindow() {
    try {
        var pageHeight = top.document.documentElement.clientHeight;
        var pageWidth = top.document.documentElement.clientWidth;
        var windowHeight = parseInt(document.getElementById('LoadingWindow').style.height, 10);
        var windowWidth = parseInt(document.getElementById('LoadingWindow').style.width, 10);

        document.getElementById('LoadingWindow').style.top = (pageHeight - windowHeight + 5) / 2 + "px";
        document.getElementById('LoadingWindow').style.left = (pageWidth - windowWidth + 5) / 2 + "px";
        top.scrollTo(0, 0);
    }
    catch (err) {
        setTimeout("centerWindow()", 1250);
        return;
    }
}

function ShowLoadingWindow() {
    $('body').css('overflow-y', 'hidden');
    $('#MainLoadingWindow').css('display', 'block');
    centerLoadingWindow();
}

function HideLoadingWindow() {
    $('body').css('overflow-y', 'auto');
    $('#MainLoadingWindow').css('display', 'none');
    $('body', parent.document).css('overflow-y', 'auto');
    $('#MainLoadingWindow', parent.document).css('display', 'none');
}

function showWindow() {
    if (typeof (Page_ClientValidate) == 'function') {
         Page_ClientValidate();
    }
    if (Page_IsValid) {
        ShowLoadingWindow();
    }
}