// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    var update = function () {
        $('#serializearray').text(
            JSON.stringify($('form').serializeArray())
        );
        $('#serialize').text(
            JSON.stringify($('form').serialize())
        );
    };
    update();
    $('form').change(update);
})

$('#theme').on("change", function () {
    var item = $("#theme option:selected").text();

    $.post("/Home/SetTheme", {
        data: item
    });
});

$(".log-in").click(function () {
    $(".signIn").addClass("active-dx");
    $(".signUp").addClass("inactive-sx");
    $(".signUp").removeClass("active-sx");
    $(".signIn").removeClass("inactive-dx");
});

$(".back").click(function () {
    $(".signUp").addClass("active-sx");
    $(".signIn").addClass("inactive-dx");
    $(".signIn").removeClass("active-dx");
    $(".signUp").removeClass("inactive-sx");
});