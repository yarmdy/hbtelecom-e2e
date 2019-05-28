

(function ($) {
    var rrucsvreferk = $("<div></div>");
    $.extend({
        rrucsvrefer: function (obj) {
            rrucsvreferk.data = obj
            rrucsvreferk.empty();
            rrucsvreferk.css({
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
            rrucsvreferk.addClass("rrucsvreferk");

            var year = ''
            var yearold = []
            var month = ''
            var monthold = []
            var csvs = ''

            $.each(obj, function () {
                if (yearold.indexOf(this.year) == -1) {
                    year += '<a data-id="' + this.year + '">' + this.year + '</a>'
                    yearold.push(this.year)
                }
                if (monthold.indexOf(this.month) == -1) {
                    month += '<a data-id="' + this.month + '">' + this.month + '</a>'
                    monthold.push(this.month)
                }
                
                csvs += '<a data-id="' + this.id + '">' + this.name + '</a>'
            })
            year = '<div act="year">年份：' + year + '</div>'
            month = '<div act="month">月份：' + month + '</div>'
            csvs = '<div act="csvs">文件：' + csvs + '</div>'
            rrucsvreferk.append(year).append(month).append(csvs);

            rrucsvreferk.appendTo("body");

        }
    });
    $.fn.extend({
        rrucsvrefer: function () {
            var $this = $(this);
            if ($this.prop("disabled")) return;
            setTimeout(function () {
                rrucsvreferk.refer = $this;

                rrucsvreferk.css({
                    "left": ($this.offset().left) + "px",
                    "top": ($this.offset().top + $this.outerHeight()) + "px"
                });
                rrucsvreferk.find('[act=csvs]').find('a').removeClass('act-active')
                rrucsvreferk.find('[act=csvs]').find('[data-id="' + rrucsvreferk.refer.val() + '"]').addClass('act-active')
                rrucsvreferk.show();
            }, 0)
        }
    });
    $(document).on("click", function (e) {
        var target = $(e.target);
        if (rrucsvreferk.is(target) || rrucsvreferk.has(target).length > 0 || rrucsvreferk.refer && (rrucsvreferk.refer.is(target) || rrucsvreferk.refer.has(target).length > 0)) return;
        rrucsvreferk.hide();
    });
    rrucsvreferk.on('click', 'a', function () {
        if ($(this).parent().attr('act') == 'csvs') {
            rrucsvreferk.refer.val($(this).attr('data-id'));
            rrucsvreferk.refer.trigger("change");
            $(this).parent().find('.act-active').removeClass('act-active')
            $(this).addClass('act-active')
            rrucsvreferk.hide();
            return
        }

        var act = $(this).parent().attr('act')
        rrucsvreferk[act] = $(this).attr('data-id')
        if ($(this).hasClass('act-active')) {
            $(this).removeClass('act-active')
            rrucsvreferk[act] = null
        } else {
            $(this).parent().find('.act-active').removeClass('act-active')
            $(this).addClass('act-active')
        }
        var newData = ''
        var year = rrucsvreferk.year
        var month = rrucsvreferk.month
        for (var i = 0; i < rrucsvreferk.data.length; i++) {
            if (year != null && rrucsvreferk.data[i].year != year) continue
            if (month != null && rrucsvreferk.data[i].month != month) continue
            newData += '<a data-id="' + rrucsvreferk.data[i].id + '">' + rrucsvreferk.data[i].name + '</a>'
        }
        rrucsvreferk.find('[act="csvs"]').empty().html("文件：").append(newData)
    })
    $("head").append("<style>.rrucsvreferk>div{padding:10px 0;}.rrucsvreferk>div a:hover{cursor:pointer;}.rrucsvreferk>div a {padding: 0 10px;color:#337ab7;display: inline-block;}.rrucsvreferk>div a.act-active{background-color:#337ab7;color:#fff;}</style>");
    window.rrucsvreferk = $.rrucsvreferk = rrucsvreferk;
})(window.jQuery);