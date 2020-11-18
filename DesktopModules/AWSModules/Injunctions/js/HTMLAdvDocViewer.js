Atalasoft.Utils.InitClientScript(myPageLoad);

var enc = getParameterByName('enc');
var status = 1;
var customAnnotation = null;
var _redirect = false;
$(document).ready(function () {
    var customTextDialog;

    $('#customTextList').on('click', '.listItem', function () {
        var selectedText = $(this).text();
        if (selectedText !== "") {
            CreateCustomTextAnnotation(selectedText);
        }
        $("#dialog_customText").dialog("close");
    });

    customTextDialog = $("#dialog_customText").dialog({
        autoOpen: false,
        title: "Custom Text List",
        height: 400,
        width: 600,
        modal: true,
        buttons: {
            Cancel: function () {
                customTextDialog.dialog("close");
            }
        },
        close: function () {
        }
    });
    $("#showCustomTextList").on("click", function () {
        customTextDialog.dialog("open");
        return false;
    });


});
function getParameterByName(name, url) {
    if (!url) {
        url = window.location.href;
    }
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function myPageLoad() {

    WebThumbnailViewer1.UrlChanged = OnThumbnailUrlChanged;
    WebThumbnailViewer1.ThumbnailDropServer = OnThumbnailsReordered;
    WebThumbnailViewer1.ThumbnailClicked = OnThumbnailClicked;
    WebThumbnailViewer1.ThumbnailDropServer = OnThumbnailDroppedServer;
    WebAnnotationViewer1.SelectionChanged = OnAddAnnotationText;
    GetCustomTextList();
}

function OnAddAnnotationText() {
    var annotations = WebAnnotationViewer1.getSelectedAnnotations();
    var annotation = annotations[0];
    if (annotation !== null) {
        var annType = annotation.getType();
        if (annType === "TextData") {
            annotation.ShowEditor();
        }
    }
}
function AppendStatus(msg) {
    $('#status').append('<p>' + msg + '</p>');
}

function OnThumbnailUrlChanged() {

    var count = WebThumbnailViewer1.getCount();
    WebThumbnailViewer1.SelectThumb(0);
}

function OnThumbnailsReordered(e) {
    WebThumbnailViewer1.SelectThumb(e.dropIndex);

}

function OnThumbnailClicked() {
    var index = WebThumbnailViewer1.getSelectedIndex();
}

// event only fires after the server has responded to a drag drop event
function OnThumbnailDroppedServer(e) {
    DragDropLayers(e.dragIndex, e.dropIndex);
}

// Uses RemoteInvoke to drag one layer from one index to another
function DragDropLayers(startIndex, destIndex) {
    WebAnnotationViewer1.RemoteInvoked = DragDropLayersCallBack;
    WebAnnotationViewer1.RemoteInvoke('Remote_DragDropLayers', [startIndex, destIndex]);
}

function DragDropLayersCallBack() {
    WebAnnotationViewer1.RemoteInvoked = function () { };
    WebThumbnailViewer1.SelectThumb(parseInt(WebAnnotationViewer1.getReturnValue()));
}
function CreateAnnotation(type, name) {
    WebAnnotationViewer1.CreateAnnotation(type, name);
}
function Delete() {
    var anns = WebAnnotationViewer1.getSelectedAnnotations();
    WebAnnotationViewer1.DeleteAnnotations(anns);
}
function SetText() {
    var text = document.getElementById("txtStampText").value;
    WebAnnotationViewer1.RemoteInvoke('InsertTextAnnotation', new Array(text));
}
function ApproveInjunction() {
    disableButtons();
    WebAnnotationViewer1.RemoteInvoked = remoteDoneHandler;
    WebAnnotationViewer1.RemoteInvoke('ApproveInjunction', new Array(enc));
}
function RejectInjunction() {
    disableButtons();
    document.getElementById('txtReason');
    var reason = document.getElementById('txtReason').value;

    WebAnnotationViewer1.RemoteInvoked = remoteDoneHandler;
    WebAnnotationViewer1.RemoteInvoke('RejectInjunction', new Array(enc, reason));
}
function CancelInjunction() {
    disableButtons();
    WebAnnotationViewer1.RemoteInvoked = remoteDoneHandler;
    WebAnnotationViewer1.RemoteInvoke('CancelInjunction', new Array(enc));
}
function remoteDoneHandler() {
    WebAnnotationViewer1.RemoteInvoked = function () { };
    var returnString = WebAnnotationViewer1.getReturnValue();
    var replyArray = returnString.split('|'); // tokenize on |
    // parse return string
    if (replyArray[0] === '-1') {
        ShowDialogBox('Error', 'Error\Mmessage: ' + replyArray[1] + '\nStackTrace: ' + replyArray[2], 'Ok', '', '', true);
        // alert("ERROR\nmessage: " + replyArray[1] + "\nStackTrace: " + replyArray[2]);
    } else if (replyArray[0] === '0') {
        ShowDialogBox('Warning', replyArray[1], 'Ok', '', '', true);
        //alert("Please Note: " + replyArray[1]);
    } else {
        location.replace(document.referrer);
    }
}

function disableButtons() {
    $('[id*=cmd]').prop('disabled', true);
    setTimeout(function () {
        $('[id*=cmd]').prop('disabled', false);
    }, 5 * 1000);
}
function CloseDialog() {
    $("#dialog").dialog("close");

    if (_redirect) {
        location.replace(document.referrer);
    }
}
//    Usage:
//    ShowDialogBox('Warning', 'Record updated successfully.', 'Ok', '', 'GoToAssetList', true);
function ShowDialogBox(title, content, btn1text, btn2text, functionText, redirect) {
    var btn1css;
    var btn2css;
    _redirect = redirect;
    if (btn1text === '') {
        btn1css = "hidecss";
    } else {
        btn1css = "showcss";
    }
    $("#lblMessage").html(content);

    $("#dialog").dialog({
        resizable: false,
        title: title,
        modal: true,
        width: '400px',
        height: 'auto',
        bgiframe: false,
        hide: { effect: 'scale', duration: 400 },
        close: CloseDialog,
        buttons: [
            {
                text: btn1text,
                "class": btn1css,
                click: function () {
                    $("#dialog").dialog("close");
                }
            }
        ]
    });
}

function GetCustomTextList() {
    var Url = "/DesktopModules/AWSModules/Injunctions/api/HttpData/GetAnnotations/";
    var annotationList = $('#customTextList');
    var choices = '';
    annotationList.empty();
    annotationList.append('<li><span class="listItem">Per Schedule</span></li>');

    $.ajax({
        type: "GET",
        dataType: "json",
        url: Url,
        success: function (j) {
           
            if (j.length > 0) {
                for (var i = 0; i < j.length; i++) {
                    c = j[i];
                    choices += '<li><span class="listItem">' + c.Text + '</span><a class="deletebutton" href="#" onclick="DeleteCustomTextFromList(' + c.Value + '); return false;"><img class="deleteButton" src="/images/delete.gif" alt="Delete Item" /></a></li>';
                }
                annotationList.append(choices);
            }
        },
        error: function () {
            ShowDialogBox("Warning", "Error Retrieving Annotations.", "Ok", "", "", false);
        }
    });
}

function RepaintAnnotations() {
    WebAnnotationViewer1.AnnotationCreated = emptyFunction;

    var annos = WebAnnotationViewer1.getAnnotations();
    for (var i = 0; i < annos.length; i++) {
        annos[i].Repaint();

    }
    WebThumbnailViewer1.UpdateThumb(WebThumbnailViewer1.getSelectedIndex());
}

function CreateCustomTextAnnotation(annoText) {
    var args = new Array();
    args.push(annoText);
    args.push(100); // this will be the X location of the anno
    args.push(100); // this wil lbe the Y location of the anno
    args.push(600); // this will be the width of the anno
    args.push(150); // this will be the height of the anno
    WebAnnotationViewer1.AnnotationCreated = annoCreated;
    WebAnnotationViewer1.RemoteInvoked = RepaintAnnotations;
    WebAnnotationViewer1.RemoteInvoke('RemoteCreateText', args);
}

function GetTextFromAnnotation() {
    var iframe = $("[id^='WebAnnotationViewer']");
    var foundText = false;
    $("[class^='textdata']", iframe.contents()).each(function (t) {
        if ($(this).prev().css('display') !== 'none') {
            foundText = true;
            AddCustomTextToList($(this).val());
        }

    });
    if (!foundText) {
        ShowDialogBox("No Text Box Selected", "Please select a textbox and try again.", "Ok", "", "", false);
    }
}

function AddCustomTextToList(text) {
    var Url = "/DesktopModules/AWSModules/Injunctions/api/HttpData/AddAnnotation/";

    $.ajax({
        type: "POST",
        cache: "false",
        url: Url,
        data: ({ AnnotationText: text }),
        success: function () {
            GetCustomTextList();
            ShowDialogBox("Notice", "The selected text has been added to your list", "Ok", "", "", false);
        },
        error: function () {
            ShowDialogBox("Warning", "Error Adding Custom Text.", "Ok", "", "", false);
        }
    });
    return false;

}

function DeleteCustomTextFromList(id) {
    var deleteConfirmed = confirm('Delete Selected Text?\n\nWARNING: This operation cannot be reversed.');
    if (deleteConfirmed) {
        var Url = "/DesktopModules/AWSModules/Injunctions/api/HttpData/DeleteAnnotation/";

        $.ajax({
            type: "POST",
            cache: "false",
            url: Url,
            data: ({ AnnotationId: id }),
            success: function () {
                GetCustomTextList();
            },
            error: function () {
                ShowDialogBox("Warning", "Error Adding Custom Text.", "Ok", "", "", false);
            }
        });
    }
    return false;
}


function UpdateSelectedTextAnnotation(textValue) {
    var iframe = $("[id^='WebAnnotationViewer']");
    var foundText = false;
    $("[class^='textdata']", iframe.contents()).each(function (t) {
        if ($(this).prev().css('display') !== 'none') {
            foundText = true;
            $(this).val(textValue);
        }
    });
    if (!foundText) {
        ShowDialogBox("No Text Box Selected", "Please select a textbox and try again.", "Ok", "", "", false);
    }
}

function annoCreated(e) {
    if (e.annotation !== null) {
        customAnnotation = e.annotation;
        setTimeout(setSelectedAnnotation, 1000);
    }

}
function setSelectedAnnotation(e) {
    customAnnotation.setSelected(true);
}
function emptyFunction() {
    return false;
}