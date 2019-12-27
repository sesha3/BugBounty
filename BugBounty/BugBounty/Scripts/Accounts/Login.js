$(document).ready(function () {
    if (IsAdfsUserNotFound === "true") {
        $("#access-denied").css("display", "block");
    }

    $('#login-email').on("click change", function () {
        if ($("#password-field").css("display") !== "none") {
            $(".login-options, #password-field").slideUp();
            $("#password-field").removeClass("has-error");
            $("#login-button").html(window.Server.App.LocalizationContent.ContinueButton);
            $('#login-password').val("");
        }
    });

    $('#windows-login').on("click", function () {
        var emailId = $("#login-email").val();
        $("#azure-email").val(emailId);
    });

    $('#login-email').keyup(function () {
        $("#error-email").css("display", "none");
    });

    $('#login-password').keyup(function () {
        $("#error-password").css("display", "none");
    });

    $(document).on("click", ".forgot-pwd-link", function (event) {
        event.preventDefault();
        if ($("#login-email").val() != "" && $("#login-email").val() != undefined) {
            localStorage.setItem(window.location.hostname + "_email", $("#login-email").val())
        }

        window.location.href = $(this).attr("href");
    });

    $("#login-form").validate({
        errorElement: "span",
        onkeyup: function (element, event) {
            if (event.keyCode != 9) $(element).valid();
            else true;
        },
        onfocusout: function (element) { $(element).valid(); },
        rules: {
            "email": {
                required: true
            },
            "password": {
                required: true
            }
        },
        highlight: function (element) {
            $(element).closest(".input-field-form").addClass("has-error");
            $("#error-email").css("display", "none");
        },
        unhighlight: function (element) {
            $(element).closest(".input-field-form").removeClass("has-error");
            $(element).parent().find("span.validation-holder").html("");
        },
        errorPlacement: function (error, element) {
            $(element).parent().find("span.validation-holder").html(error);
            $(element).parent().find("span.validation-holder").css("display", "block");
            $("#error-password").css("display", "none");
        },
        messages: {
            "email": {
                required: window.Server.App.LocalizationContent.EmailValidator
            },
            "password": {
                required: window.Server.App.LocalizationContent.EnterPassword
            }
        }
    });
});

function FormValidate() {
    $("#access-denied").css("display", "none");
    if ($("#password-field").css("display") === "none") {
        if ($("#login-form").valid()) {
            $(".mail-loader-div").addClass("email-loader");
            $("#login-button").attr("disabled", "disabled");
            var emailId = $("#login-email").val();
            $.ajax({
                type: "POST",
                url: validateEmailUrl,
                data: { email: emailId, callBackUri: callBackUri },
                success: function (result) {
                    if (result.Value != null && result.Value != undefined) {
                        window.location.href = result.Value;
                    } else {
                        if (result.Status === true) {
                            if (result.DirectoryType === 2) {
                                $("#azure-email").val(emailId);
                                $("#windows-login").trigger("click");
                                $(".mail-loader-div").removeClass("email-loader");
                                $("#login-button").removeAttr("disabled");
                            }
                            else if (result.DirectoryType === 5) {
                                $("#syncfusion-login").trigger("click");
                                $(".mail-loader-div").removeClass("email-loader");
                            }
                            else {
                                $(".mail-loader-div").removeClass("email-loader");
                                $("#login-button").removeAttr("disabled");
                                $("#password-field, .login-options").slideDown();
                                $("#login-button").html(window.Server.App.LocalizationContent.LoginButton);
                                $("#login-password").focus();
                            }
                        } else {
                            $(".mail-loader-div").removeClass("email-loader");
                            $("#login-button").removeAttr("disabled");
                            $(".login-fields .email").addClass("has-error");
                            $("#error-email").css("display", "block").html(window.Server.App.LocalizationContent.InvalidAccount);
                        }
                    }
                }
            });
        }
        return false;
    } else {
        if ($("#login-form").valid()) {
            $("body").ejWaitingPopup("show");
        }
        return $("#login-form").valid();
    }
}

$(document).on("click", "#adfs-login-text", function () {
    $("#windows-login").trigger("click");
});