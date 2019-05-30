(function ($) {
    var init = function (qq,data) {
        var kuang = $("<div></div>");
        kuang.css({
            "width": "400px",
            "height": "300px",
            "box-shadow": "0 0 5px 0 rgba(0,0,0,.5)",
            "position": "absolute",
            "left": "0",
            "top": "0",
            "display": "none",
            "background-color": "#fff",
            "overflow": "auto",
            "z-index": "9999",
            "border-radius": "5px",
            "padding": "10px",
            "font-size": "14px"
        });
        kuang.addClass("multiselectk");
        kuang.multiselect.zhuren = qq;
        $("body").append(kuang);
        qq.multiselect.kuang = kuang;
        qq.multiselect.data = data;
    };
    var edit = function (qq) {
        if (qq.prop("disabled")) return;
        setTimeout(function () {
            qq.multiselect.kuang.edit = true;
            qq.multiselect.kuang.css({
                "left": (qq.offset().left) + "px",
                "top": (qq.offset().top + qq.outerHeight()) + "px"
            });
            qq.multiselect.kuang.empty();
            {
                var year = ''
                var yearold = []
                var month = ''
                var monthold = []
                var csvs = ''
                $.each(qq.multiselect.data.all, function () {
                    if (yearold.indexOf(this.year) == -1) {
                        year += '<a data-id="' + this.year + '">' + this.year + '</a>'
                        yearold.push(this.year)
                    }
                    if (monthold.indexOf(this.month) == -1) {
                        month += '<a data-id="' + this.month + '">' + this.month + '</a>'
                        monthold.push(this.month)
                    }

                    csvs += '<a data-id="' + this.id + '">' + this.name + '</a>'
                });
                year = '<div act="year">年份：' + year + '</div>'
                month = '<div act="month">月份：' + month + '</div>'
                csvs = '<div act="csvs">文件：' + csvs + '</div>'
                qq.multiselect.kuang.append(year).append(month).append(csvs);
            }

            qq.multiselect.kuang.show();
        }, 0);
    }
    var show = function (qq) {
        setTimeout(function () {
            qq.multiselect.kuang.edit = false;
            qq.multiselect.kuang.css({
                "left": (qq.offset().left) + "px",
                "top": (qq.offset().top + qq.outerHeight()) + "px"
            });
            qq.multiselect.kuang.show();
        }, 0);
    }
    $.fn.extend({
        multiselect: function (act, data) {
            var res=this;
            switch (act) {
                case "init":
                    init(this,data);
                    break;
                case "edit":
                    edit(this);
                    break;
                case "show":
                    show(this);
                    break;
                default:
                    break;
            }
            return res;
        }
    });
    $(document).on("click", function (e) {
        var target = $(e.target);
        $.each($(".multiselectk"), function () {
            if ($(this).is(target) || $(this).has(target).length > 0 || $(this).multiselect.zhuren && ($(this).multiselect.zhuren.is(target) || $(this).multiselect.zhuren.has($(this)).length > 0)) return;
            $(this).hide();
        });
    });
    $("head").append("<style>.multiselectk>div{padding:10px 0;}.multiselectk>div a:hover{cursor:pointer;}.multiselectk>div a {padding: 0 10px;color:#337ab7;display: inline-block;}.multiselectk>div a.act-active{background-color:#337ab7;color:#fff;}</style>");
})(window.jQuery);