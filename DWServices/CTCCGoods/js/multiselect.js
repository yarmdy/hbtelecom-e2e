(function ($) {
    var init = function (qq,data) {
        var kuang = $("<div></div>");
        kuang.css({
            "width": "450px",
            "height": "400px",
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
        kuang.on("click", "a", {k:kuang},kcevent);
        $("body").append(kuang);
        qq.multiselect.kuang = kuang;
        qq.multiselect.data = data;
        qq.trigger("selected");
    };
    var edit = function (qq) {
        if (qq.prop("disabled")) return;
        setTimeout(function () {
            qq.multiselect.kuang.edit = true;
            qq.multiselect.kuang.css({
                "left": (qq.offset().left) + "px",
                "top": (qq.offset().top + qq.outerHeight()) + "px",
                "width": "450px",
                "height": "400px"
            });
            qq.multiselect.kuang.empty();
            {
                var year = ''
                var yearold = []
                var month = ''
                var monthold = []
                var csvs = ''
                $.each(qq.multiselect.data.data, function () {
                    if (yearold.indexOf(this.year) == -1) {
                        var thisyear = '<a data-id="' + this.year + '">' + this.year + '</a>';
                        if (!qq.multiselect.kuang.multiselect.year) {
                            qq.multiselect.kuang.multiselect.year = this.year;
                        }
                        if (qq.multiselect.kuang.multiselect.year == this.year) {
                            thisyear = '<a data-id="' + this.year + '" class="act-active">' + this.year + '</a>';
                        }
                        year += thisyear;
                        yearold.push(this.year);
                    }
                    if (monthold.indexOf(this.month) == -1) {
                        var thismonth = '<a data-id="' + this.month + '">' + this.month + '</a>';
                        if (!qq.multiselect.kuang.multiselect.month) {
                            qq.multiselect.kuang.multiselect.month = this.month;
                        }
                        if (qq.multiselect.kuang.multiselect.month == this.month) {
                            thismonth = '<a data-id="' + this.month + '" class="act-active">' + this.month + '</a>';
                        }
                        month += thismonth;
                        monthold.push(this.month)
                    }
                    if (qq.multiselect.kuang.multiselect.year == this.year && qq.multiselect.kuang.multiselect.month == this.month) {
                        csvs += '<a data-id="' + this.id + '" class="' + (this.selected?'act-active':'') + '">' + this.name + '</a>'
                    }
                });
                timese = '<div act="timese">时间段：<span class="t1">' + qq.multiselect.data.start + '</span>-<span class="t2">' + qq.multiselect.data.end + '</span><a class="timese">选择时间段</a></div>';
                msg = '<div act="msg">提示：<span class="msg"></span></div>';
                year = '<div act="year">年份：' + year + '</div>'
                month = '<div act="month">月份：' + month + '</div>'
                csvs = '<div act="csvs">文件：' + csvs + '</div>'
                qq.multiselect.kuang.append(timese).append(msg).append(year).append(month).append(csvs);
            }

            qq.multiselect.kuang.show();
        }, 0);
    }
    var show = function (qq) {
        setTimeout(function () {
            qq.multiselect.kuang.edit = false;
            qq.multiselect.kuang.css({
                "left": (qq.offset().left) + "px",
                "top": (qq.offset().top + qq.outerHeight()) + "px",
                "width": "450px",
                "height": "300px",
            });
            qq.multiselect.kuang.empty();

            var list = qq.multiselect("selected");

            var msg = '<div act="list">已选择：<span>' + qq.multiselect.data.start + '</span>-<span>' + qq.multiselect.data.end + '</span></div>'
            var liststr = "";
            $.each(list, function () {
                liststr += '<a>' + this + '</a>';
            });
            qq.multiselect.kuang.append(msg).append("<div act='list'>"+liststr+"</div>");


            qq.multiselect.kuang.show();
        }, 0);
    }
    var selected = function (qq) {
        var data = [];
        $.each(qq.multiselect.data.data, function () {
            if (this.selected) {
                data.push(this.name);
            }
        })
        return data.sort();
    }
    var info = function (qq) {
        var data = selected(qq);
        var res = { list: data };
        res.count = data.length;
        res.start=qq.multiselect.data.start;
        res.end = qq.multiselect.data.end;
        return res;
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
                case "selected":
                    return selected(this);
                    break;
                case "info":
                    return info(this);
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
    var kcevent = function (e) {
        var kuang = e.data.k;
        if ($(this).parent().attr('act') == 'list') {
            return;
        }
        if ($(this).parent().attr('act') == 'csvs') {
            var timese = kuang.find(".timese");
            var t1 = kuang.find(".t1");
            var t2 = kuang.find(".t2");
            var msg = kuang.find(".msg");
            if (timese.hasClass("act-active")) {
                if (t1.html() == "") {
                    t1.html($(this).html());
                    msg.html("选择结束时间");
                } else {
                    timese.removeClass("act-active");
                    msg.html("");
                    t2.html($(this).html());

                    $.each(kuang.multiselect.zhuren.multiselect.data.data, function () {
                        if (this.name.localeCompare(t1.html()) >= 0 && this.name.localeCompare(t2.html()) <= 0) {
                            this.selected = true;
                        } else {
                            this.selected = false;
                        }
                    });
                    $.each($(this).parent().find("a"), function () {
                        if ($(this).html().localeCompare(t1.html()) >= 0 && $(this).html().localeCompare(t2.html()) <= 0) {
                            $(this).addClass("act-active");
                        } else {
                            $(this).removeClass("act-active");
                        }
                    });
                    kuang.multiselect.zhuren.multiselect.data.start = t1.html();
                    kuang.multiselect.zhuren.multiselect.data.end = t2.html();
                    kuang.multiselect.zhuren.trigger("selected");
                }
            } else {
                timese.removeClass("act-active");
                msg.html("");
                t1.html("");
                t2.html("");
                kuang.multiselect.zhuren.multiselect.data.start = "";
                kuang.multiselect.zhuren.multiselect.data.end = "";
                if ($(this).hasClass("act-active")) {
                    $(this).removeClass("act-active");
                } else {
                    $(this).addClass("act-active");
                }
                var $this = $(this);
                $.each(kuang.multiselect.zhuren.multiselect.data.data, function () {
                    if (this.name == $this.html()) {
                        this.selected = $this.hasClass("act-active");
                    } 
                });
                kuang.multiselect.zhuren.trigger("selected");
            }
            return;
        }
        if ($(this).parent().attr('act') == 'timese') {
            var t1 = kuang.find(".t1");
            var t2 = kuang.find(".t2");
            var msg = kuang.find(".msg");
            t1.html("");
            t2.html("");
            if ($(this).hasClass("act-active")) {
                msg.html("");
                $(this).removeClass("act-active");
            } else {
                msg.html("选择起始时间");
                $(this).addClass("act-active");
            }
            return
        }
        var act = $(this).parent().attr('act')
        kuang.multiselect[act] = $(this).attr('data-id')
        if ($(this).hasClass('act-active')) {
            //$(this).removeClass('act-active')
            //kuang.multiselect[act] = null
        } else {
            $(this).parent().find('.act-active').removeClass('act-active')
            $(this).addClass('act-active')
        }
        var newData = ''
        var year = kuang.multiselect.year
        var month = kuang.multiselect.month
        for (var i = 0; i < kuang.multiselect.zhuren.multiselect.data.data.length; i++) {
            if (year != null && kuang.multiselect.zhuren.multiselect.data.data[i].year != year) continue
            if (month != null && kuang.multiselect.zhuren.multiselect.data.data[i].month != month) continue
            newData += '<a data-id="' + kuang.multiselect.zhuren.multiselect.data.data[i].id + '" class="' + (kuang.multiselect.zhuren.multiselect.data.data[i].selected ? 'act-active' : '') + '">' + kuang.multiselect.zhuren.multiselect.data.data[i].name + '</a>'
        }
        kuang.find('[act="csvs"]').empty().html("文件：").append(newData)
    };
    $("head").append("<style>.multiselectk>div{padding:10px 0;}.multiselectk>div a:hover{cursor:pointer;}.multiselectk>div a {padding: 0 10px;color:#337ab7;display: inline-block;}.multiselectk>div a.act-active{background-color:#337ab7;color:#fff;}.multiselectk>div span{color:red}</style>");
})(window.jQuery);