

(function ($) {
    var goodsreferk = $("<div></div>");
    $.extend({
        goodsrefer: function (obj) {
            //for (var i = 0; i < 100;i++) {
            //    obj.push({ 'id':i+100, 'name': i, 'cid': (i+100)%10, 'cname': i, 'class2': (i+100)%10, 'pid': (i+100)%10, 'pname': i})
            //}
            goodsreferk.data = obj 
            goodsreferk.empty();
            goodsreferk.css({
                "width": "400px",
                "height": "300px",
                //"border": "1px solid #000",
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
            goodsreferk.addClass("goodsreferk");

            var cclass = ''
            var cclassold = []
            var class2 = ''
            var cclas2old = []
            var chang = ''
            var changold = []
            var goods = ''

            $.each(obj, function () {
                if (cclassold.indexOf(this.cid) == -1) {
                    cclass += '<a data-id="' + this.cid + '">' + this.cname + '</a>'
                    cclassold.push(this.cid)
                }
                if (cclas2old.indexOf(this.class2) == -1) {
                    class2 += '<a data-id="' + this.class2 + '">' + this.class2 + '</a>'
                    cclas2old.push(this.class2)
                }
                if (changold.indexOf(this.pid) == -1) {
                    chang += '<a data-id="' + this.pid + '">' + this.pname + '</a>'
                    changold.push(this.pid)
                }
                goods += '<a data-id="'+this.id+'">'+this.name+'</a>'
            })
            cclass = '<div act="cclass">设备类型：' + cclass + '</div>'
            class2 = '<div act="class2">设备分类：' + class2 + '</div>'
            chang = '<div act="chang">厂家：' + chang + '</div>'
            goods = '<div act="goods">设备型号：' + goods + '</div>'
            goodsreferk.append(class2).append(cclass).append(chang).append(goods);

            //goodsreferk.append(class1Box).append(class2Box).append(changBox).append(goodsstr);
            goodsreferk.appendTo("body");
           
        }
    });
    $.fn.extend({
        goodsrefer: function () {
            var $this = $(this);
            if($this.prop("disabled")) return;
            setTimeout(function () {
                goodsreferk.refer = $this;

                goodsreferk.css({
                    "left": ($this.offset().left) + "px",
                    "top": ($this.offset().top + $this.outerHeight()) + "px"
                });
                goodsreferk.find('[act=goods]').find('a').removeClass('act-active')
                goodsreferk.find('[act=goods]').find('[data-id="' + goodsreferk.refer.val() + '"]').addClass('act-active')
                goodsreferk.show();
            },0)
        }
    });
    $(document).on("click", function (e) {
        var target = $(e.target);
        if (goodsreferk.is(target) || goodsreferk.has(target).length > 0 || goodsreferk.refer&&(goodsreferk.refer.is(target) || goodsreferk.refer.has(target).length > 0)) return;
        goodsreferk.hide();
        //console.log(e);

    });
    goodsreferk.on('click', 'a', function () {
        if ($(this).parent().attr('act') == 'goods') {
            goodsreferk.refer.val($(this).attr('data-id'))
            $(this).parent().find('.act-active').removeClass('act-active')
            $(this).addClass('act-active')
            goodsreferk.hide();
            return
        }

        var act = $(this).parent().attr('act')
        goodsreferk[act] = $(this).attr('data-id')
        if ($(this).hasClass('act-active')) {
            $(this).removeClass('act-active')
            goodsreferk[act] = null
        } else {
            $(this).parent().find('.act-active').removeClass('act-active')
            $(this).addClass('act-active')
        }
        var newData = ''
        var cclass = goodsreferk.cclass
        var chang = goodsreferk.chang
        var class2 = goodsreferk.class2
        for (var i = 0; i < goodsreferk.data.length; i++) {
            if (cclass != null && goodsreferk.data[i].cid != cclass) continue
            if (chang != null && goodsreferk.data[i].pid != chang) continue
            if (class2 != null && goodsreferk.data[i].class2 != class2) continue
            newData += '<a data-id="' + goodsreferk.data[i].id + '">' + goodsreferk.data[i].name + '</a>'
        }
        goodsreferk.find('[act="goods"]').empty().html("设备型号：").append(newData)
    })
    $("head").append("<style>.goodsreferk>div{padding:10px 0;}.goodsreferk>div a:hover{cursor:pointer;}.goodsreferk>div a {padding: 0 10px;color:#337ab7;display: inline-block;}.goodsreferk>div a.act-active{background-color:#337ab7;color:#fff;}</style>");
    window.goodsreferk = $.goodsreferk = goodsreferk;
})(window.jQuery);