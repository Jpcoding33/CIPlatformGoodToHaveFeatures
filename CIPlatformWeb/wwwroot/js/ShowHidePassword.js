$(document).ready(function () {
    $("#show_hide_password1 a").on('click', function (event) {
        event.preventDefault();
        if ($('#show_hide_password1 input').attr("type") == "text") {
            $('#show_hide_password1 input').attr('type', 'password');
            $('#show_hide_password1 i').addClass("bi-eye-slash-fill");
            $('#show_hide_password1 i').removeClass("bi-eye-fill");
        }
        else if ($('#show_hide_password1 input').attr("type") == "password") {
            $('#show_hide_password1 input').attr('type', 'text');
            $('#show_hide_password1 i').removeClass("bi-eye-slash-fill");
            $('#show_hide_password1 i').addClass("bi-eye-fill");
        }
    });
});

$(document).ready(function () {
    $("#show_hide_password2 a").on('click', function (event) {
        event.preventDefault();
        if ($('#show_hide_password2 input').attr("type") == "text") {
            $('#show_hide_password2 input').attr('type', 'password');
            $('#show_hide_password2 i').addClass("bi-eye-slash-fill");
            $('#show_hide_password2 i').removeClass("bi-eye-fill");
        }
        else if ($('#show_hide_password2 input').attr("type") == "password") {
            $('#show_hide_password2 input').attr('type', 'text');
            $('#show_hide_password2 i').removeClass("bi-eye-slash-fill");
            $('#show_hide_password2 i').addClass("bi-eye-fill");
        }
    });
});