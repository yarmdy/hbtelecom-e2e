$(function () {
    $.post("/services/loginyz.ashx", function (data) {
        var res = eval("(" + data + ")");
        if (!res.ok) {
            parent.parent.parent.parent.location.href = "/login.html";
        }
    });
});