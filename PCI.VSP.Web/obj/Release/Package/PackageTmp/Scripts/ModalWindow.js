window.onresize = CenterWindow();

function CenterWindow() {
    try {
        var pageHeight = top.document.documentElement.clientHeight;
        var pageWidth = top.document.documentElement.clientWidth;
        var windowHeight = parseInt(document.getElementById('ModalWindow').style.height, 10);
        var windowWidth = parseInt(document.getElementById('ModalWindow').style.width, 10);

        document.getElementById('ModalWindow').style.top = (pageHeight - windowHeight + 5) / 2 + "px";
        document.getElementById('ModalWindow').style.left = (pageWidth - windowWidth + 5) / 2 + "px";
        top.scrollTo(0, 0);
    }
    catch (err) {
        setTimeout("CenterWindow()", 1250);
        return;
    }
}

function ShowModalWindow(url, height, width) {
    $('body').css('overflow-y', 'hidden');
    $('#ModalWindow').css({ 'height': height+5, 'width': width+5 });
    $('#iWindow').css({ 'height': height, 'width': width });
    $('#iWindow').css({ 'border': "0px solid #D1D1D1" });
    $('#iWindow').get(0).contentWindow.location.replace(url);
    $('#MainModalWindow').css('display', 'block');
    CenterWindow();
}

function ResizeModalWindow(height, width) {
    $('#ModalWindow').css({ 'height': height + 5, 'width': width + 5 });
    $('#iWindow').css({ 'height': height, 'width': width });
    CenterWindow();
}

function HideModalWindow(reload) {
    $('body').css('overflow-y', 'auto');
    $('#MainModalWindow').css('display', 'none');
    if ($('#iWindow').get(0) != null)
        $('#iWindow').get(0).contentWindow.location.replace('about:blank');
    else if ($('#iWindow', parent.document).get(0) != null)
        $('#iWindow', parent.document).get(0).contentWindow.location.replace('about:blank');
    //$('#iWindow').attr('src', '/blank.htm');
    //$('#iWindow', parent.document).attr('src', '/blank.htm');
    $('body', parent.document).css('overflow-y', 'auto');
    $('#MainModalWindow', parent.document).css('display', 'none');
    if (reload) {
        if (window.parent != null) {
            window.parent.location.replace(window.parent.location.href);
        }
        else if (parent != null) {
            parent.location.replace(parent.location.href);
        }
        else {
            window.location.replace( window.location.href);
        }
    }
}

