$(function () {
    $("#PopupContainer").ejDialog({
        width: "450px",
        showOnInit: false,
        allowDraggable: false,
        enableResize: false,
        height: "auto",
        title: "Validation",
        showHeader: false,
        enableModal: true,
        close: "onValidateDialogClose",
        closeOnEscape: true
    });

    var validatePopupTemplateId = createLoader("PopupContainer_wrapper");
    $("#PopupContainer_wrapper").ejWaitingPopup({ template: $("#" + validatePopupTemplateId) });

});

function createLoader(element) {
    var returnId = "";
    if (typeof element === "string") {
        var selector = (element.indexOf(".") === 0) ? "." : "#";
        element = (element.indexOf(".") === 0) ? element.slice(1, element.length) : (element.indexOf("#") === 0) ? element.slice(1, element.length) : element;
        returnId = element + "-loader-icon";

        if ($("#" + returnId).length == 0 && $(selector + element).length != 0) {
            var template = $("<div class='loader-blue loader-icon' id='" + returnId + "'><svg class='circular'><circle class='path' cx='27' cy='27' r='20' fill='none' stroke-width='4' stroke-miterlimit='10'></circle></svg></div>");
            $("body").append(template);
        }
        return returnId;
    }
    else {
        element = element.selector;
        var selector = (element.indexOf(".") === 0) ? "." : "#";
        element = element.slice(1, element.length);
        returnId = element + "-loader-icon";
        if ($("#" + returnId).length == 0 && $(selector + element).length != 0) {
            var template = $("<div class='loader-blue loader-icon' id='" + returnId + "'><svg class='circular'><circle class='path' cx='27' cy='27' r='20' fill='none' stroke-width='4' stroke-miterlimit='10'></circle></svg></div>");
            $("body").append(template);
        }
    }

    return returnId;
}

function SuccessAlert(header, content, duration) {
    window.top.$("#success-alert").css("display", "table");
    window.top.$("#message-header").html(header);
    window.top.$("#message-content").html(content);
    setTimeout(function () {
        window.top.$('#success-alert').fadeOut();
    }, duration);
}

function WarningAlert(header, content, duration) {
    parent.$("#warning-alert").css("display", "table");
    parent.$("#warning-alert #message-header").html(header);
    parent.$(" #warning-alert #message-content").html(content);
    if (duration != null && duration != "") {
        setTimeout(function () {
            parent.$('#warning-alert').fadeOut();
        }, duration);
    }
    $(document).on("click", ".close-div", function () {
        parent.$('#warning-alert').fadeOut();
    });
}

function onValidateDialogClose() {
    $("#PopupContainer").ejDialog("close");
}

$(document).on("click", "#bug-validate", function () {
    $("#PopupContainer").ejDialog("open");
});

$(document).on("click", "#update-validation", function () {
    var bugId = "f023d3e9-d1f3-4e2f-a89d-675dd665733a";
    var rate = $("#select-rate").val();
    $("#PopupContainer_wrapper").ejWaitingPopup("show");
    $.ajax({
        type: "POST",
        url: validateUrl,
        data: { bugId: bugId, rate: rate },
        success: function (data) {
            parent.$("#PopupContainer_wrapper").ejWaitingPopup("hide");
            if (data.status) {
                window.parent.$("#PopupContainer").ejDialog("close");
                //parent.messageBox("su-copy", window.Server.App.LocalizationContent.CopyDashboard, window.Server.App.LocalizationContent.CopyDashboardSuccess, "success", function () {
                //    parent.RefreshCategorySection(toCategory);
                //    parent.onCloseMessageBox();
                //});
                
                SuccessAlert("Validate", "Success", 7000);
            }
            else {
                WarningAlert("Validate", "Failed", 7000);
            }
        }
    });
});

$(document).on("click", "#CancelButton", function () {
    onValidateDialogClose();
});

$(document).on("click", ".Head-icon", function () {
    onValidateDialogClose();
});
