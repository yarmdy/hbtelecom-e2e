
var rowOne = parent.rowOne;

var menuarr = [
{
    key: "fgwt", value: "MR问题", "url": "../services/MRDataServices.ashx", childs: [

            { key: "rfgbl", value: "弱覆盖", "decimal": "2", unit: "%", yname: "弱覆盖比例" },
         { key: "cdfgk", value: "重叠覆盖", "decimal": "2", unit: "%", yname: "重叠覆盖比例" },
          { key: "gfglqgs", value: "过覆盖", "decimal": "2", unit: "个", yname: "过覆盖邻区个数" },
             { key: "xqpjrsrp", value: "小区平均RSRP", "decimal": "0", unit: "dBm", "yname": "RSRP" }, 
            
            { key: "pjjl", value: "平均TA距离", "decimal": "0", unit: "米", yname: "距离" }]

},

{
    key: "grwt", value: '干扰问题', "url": "../services/MRDataServices.ashx", childs: [ 
              { key: "modgrcd", value: "下行模三干扰", "decimal": "2", unit: "%", yname: "MOD3干扰比例" }, 
              { key: "sxgr", value: "上行干扰", "decimal": "2", unit: "dBm", yname: "RSSI" }
    ]
}, {
    key: "rlwt", value: "容量问题", "url": "../services/KPIServices.ashx?iseight=" + rowOne.ISEIGHT, childs: [{
        key: "1800M", value: '1800M', childs: [
            { key: "sxprbpjlyl18", value: "上行PRB平均利用率", "decimal": "2", "clickson": 1, unit: "%", yname: "利用率" },
            { key: "xxprbpjlyl18", value: "下行PRB平均利用率", "decimal": "2", "clickson": 1, unit: "%", yname: "利用率" },
            { key: "pjrrcljyhs18", value: "RRC连接用户数", "decimal": "0", "clickson": 1, unit: "户", yname: "用户数" },
            { key: "pdcpxx18", value: "下行流量", "decimal": "0", "clickson": 1, unit: "MB", yname: "流量" },
            { key: "pdcpsx18", value: "上行流量", "decimal": "0", "clickson": 1, unit: "MB", yname: "流量" },
            { key: "pjjhyhs18", value: "4.19 平均激活用户数", "decimal": "0", "clickson": 1, unit: "户", yname: "用户数" }
        ]
    },
		{
		    key: "800M", value: '800M', childs: [
                { key: "sxprbpjly8", value: "平均利用率", "decimal": "2", "clickson": 1, unit: "%", yname: "利用率" },
                { key: "xxprbpjly8", value: "下行PRB平均利用率", "decimal": "2", "clickson": 1, unit: "%", yname: "利用率" },
                { key: "pjrrcljyhs8", value: "MRRC连接用户数", "decimal": "0", "clickson": 1, unit: "户", yname: "用户数" },
                { key: "pdcpxx8", value: "下行流量", "decimal": "0", "clickson": 1, unit: "MB", yname: "流量" },
                { key: "pdcpsx8", value: "上行流量", "decimal": "0", "clickson": 1, unit: "MB", yname: "流量" },
                { key: "pjjhyhs8", value: "4.19 平均激活用户数", "decimal": "0", "clickson": 1, unit: "%", yname: "用户数" }
		    ]
		}
    ]
}, {
    key: "KPI", value: "KPI指标", "url": "../services/KPIServices.ashx", childs: [{
        key: "jrxn", value: '接入性能', childs: [
            { key: "rrcljcgl",cs:"rrcsbcs", "chartindex": 1,"clickson": 1, value: "RRC连接建立成功率", "decimal": "2", unit: "%", yname: "次数" },

            { key: "xhysl", cs: "xhyscs", "chartindex": 1,"clickson": 1, value: "寻呼拥塞率", "decimal": "2", unit: "%", yname: "次数" },

            { key: "rrcljcjbl", cs: "rrcljcjcgcs", "chartindex": 1,"clickson": 1, value: "RRC连接重建比例", "decimal": "0", unit: "%", yname: "次数" }
        ]
    },
		{
		    key: "bcsx", value: '保持性能', childs: [
			{ key: "uesxwdxl", cs: "uesxycsfcs", "chartindex": 1,"clickson": 1, value: "UE上下文掉线率", "decimal": "2", unit: "%", yname: "次数" },

			{ key: "erabdxl", cs: "erabdxcs", "chartindex": 1,"clickson": 1, value: "E-RAB掉线率", "decimal": "2", unit: "%", yname: "次数" },

			{ key: "rrccjbl", cs: "rrccjqqcs", "chartindex": 1,"clickson": 1, value: "RRC重建比例", "decimal": "2", unit: "%", yname: "次数" },
		    ]
		},
		{
		    key: "ydxn", value: '移动性能', childs: [
			{ key: "xtnqhcgl", cs: "xtnqsbcs", "chartindex": 1,"clickson": 1, value: "系统内切换成功率", "decimal": "2", unit: "%", yname: "次数" },

			{ key: "cdsbl", cs: "ltecdxcs", "chartindex": 1,"clickson": 1, value: "4G重定向到3G的比例", "decimal": "2", unit: "%", yname: "比例(%)" },
		    ]
		},
		{
		    key: "qt", value: '其他', childs: [
			{ key: "cqibl", value: "CQI≥7的比例", "decimal": "2", unit: "%", yname: "比例" },
			{ key: "xxprbslzb", value: "11.1 下行PRB双流占比", "decimal": "2", unit: "%", yname: "占比" },
			{ key: "yhtysxpjsl", value: "8.13 用户体验上行平均速率(%)", "decimal": "2", unit: "Mbps", yname: "速率" },
			{ key: "yhtyxxpjsl", value: "8.14用户体验下行平均速率", "decimal": "2", unit: "Mbps", yname: "速率" },
			{ key: "xqtfsc", value: "小区退服时长", "decimal": "0", unit: "分", yname: "时长" },
			{ key: "xqzdl", value: "小区中断率", "decimal": "2", unit: "%", yname: "中断率" },
			{ key: "xqkyl", value: "小区可用率", "decimal": "2", unit: "%", yname: "可用率" },
			{ key: "grzspjz", value: "干扰噪声的平均值", "decimal": "2", unit: "dBm", yname: "均值" }
		    ]
		}
    ]
}, {
    key: "sbgj", value: '设备告警', "url": "jgwturl", childs: [{
        key: 'zhongxing', value: '中兴'
    }, {
        key: 'huawei', value: '华为'
    }, {
        key: 'nuojiya', value: '诺基亚'
    }]
}];
var curmenuarr;
var childsarr;
var dataactionurl;
function getcurmenuarr(id, tt) { 
    $("#showview").empty(); 
    if (id == "sbgj") {
        shtemplatehtml("zcxq");
        return;
    }
    if (id == "dwxx") { 
        dwtxxemplatehtml(); 
        document.getElementById("dwxxmap").style.width = 1024;
        return;
    }
    for (var i = 0; i < menuarr.length; i++) {
        console.log("menuarr[i].key :::", menuarr[i].key);
        console.log("id:::", id);
        if (menuarr[i].key == id) {
            console.log("子对象:::", menuarr[i].childs[0].childs)
            dataactionurl = menuarr[i].url;
            if (menuarr[i].childs[0].childs != undefined) {
                curmenuarr = (menuarr[i].childs[0].childs);
                childsarr = menuarr[i].childs;
                createadddiv(curmenuarr);
            } else {
                curmenuarr = (menuarr[i].childs);
                createadddiv(curmenuarr);
            }


        }
    }
}
function showson(id) {
    $("#showview").empty();
    createadddiv(childsarr[id].childs);

} 
function shtemplatehtml(id) {
    $("#showview").append($("#sbgjtemplate").html());
    showtableview(id);
}
function createadddiv(childsarr) {
    var ddd = 0;
    var str = '<div id="' + ddd + '" class="row" style="margin-top:10px">';
    for (var i = 0; i < childsarr.length; i++) {
        str += (creatediv(childsarr[i], i));
        if ((i + 1) % 3 == 0 && i != (childsarr.length - 1)) {
            str += '</div><div id="datediv' + ddd + '" border="1"  class="dayclasss"  >'
                + '<div><button type="button" class="close closebutton" data-dismiss="modal" aria-hidden="true">×</button></div>'
                + '<div id="datechartshow"  class="datechartshowclass" ></div>'
                + '</div><div id="' + (ddd + 1) + '" class="row" style="margin-top:10px">'
            ddd = ddd + 1;
        }
    }
    $("#showview").append(str + "</div>");
    $("#showview").append("<div border='1' id= 'datediv" + ddd + "'class='dayclasss' >"
           + '<div><button type="button" class="close closebutton" data-dismiss="modal "  aria-hidden="true">×</button></div>'
                + '<div id="datechartshow" class="datechartshowclass"  ></div>'
        + "</div>");
    getchartview(childsarr);
}


function creatediv(obj, i) {
    i = i + 1;
    console.log("obj::", obj);
    if (i % 3 == 1) {
        var str = '<div class="home_charts1_left csg-panel-fs csg-clink"   style="height: 360px">';

    } else if (i % 3 == 2) {
        var str = '<div class="home_charts1_center csg-panel-fs csg-clink"  style="height: 360px">';

    } else if (i % 3 == 0) {
        var str = '<div class="home_charts1_right csg-panel-fs csg-clink"  style="height: 360px">';
    }
    str = str + '<div class="charts_title" >' + obj.value + '</div>  ' + '<div id="' + obj.key + 'div" class="rect-410-308" ></div> ' + '<div class="showmessss"><a  onclick="javascript:void(0)" calss="text-decoration"></a></div></div>';

    return str;
}


var selectparam = parent.selectparam;
function getchartview(showarrdivs) {
    for (var i = 0; i < showarrdivs.length; i++) {
        var selectparamnew = deepClone(selectparam);
        selectparamnew.key = showarrdivs[i].key;
        createechart(dataactionurl, selectparamnew, showarrdivs[i]);
    }
}

var echartjosn = ["frame/echarts/chart/bar-color.json", "frame/echarts/chart/bar-line-color.json"];
var barColorJson = null;
var isindex = 0;
var getbarColorJson = function (index) {
    console.log(echartjosn[index]);
    $.ajax({ url: echartjosn[index], async: false }).success(function (geoJson) {
        geoJson.backgroundColor = new echarts.graphic.RadialGradient(0.3, 0.3, 0.8, [{ "offset": 0, "color": "#f7f8fa" }, { "offset": 1, "color": "#cdd0d5" }]);
        barColorJson = geoJson;
        console.log("geoJson:::", geoJson);
        return geoJson;
    });
    return barColorJson;
}
var datearr = [];
var sevendate=[];
var createechart = function (url, param, showarrdiv) {

    if (isindex != showarrdiv.chartindex) {
        barColorJson = null;
    }
    isindex = showarrdiv.chartindex == undefined ? 0 : 1;
    if (barColorJson == null) {
        deepClone(getbarColorJson(isindex));
    }
    var barColorJsonNew = deepClone(barColorJson);
    $.get(url, param, function (result) {
        console.log("showarrdiv", result);

        var maxval = "";
        var minval = "";
        if (result != undefined && result != "") {
            result = eval("(" + result + ")")
        } else {
            return;
        }

        var datanormal = [null, null, null, null, null, null, null];
        var dataerror = [null, null, null, null, null, null, null];
        var data3 = [null, null, null, null, null, null, null];
        
        if (isindex != 1) {
            for (var i = 0; i < result.data.length; i++) {
                var tempdate = new Date(result.data[i].SDATE); 
                if (datearr.length < 7) {
                    datearr.push(tempdate.Format("MM-dd")); 
                    sevendate.push({ date: tempdate.Format("yyyy-MM-dd"), "tempdate": tempdate.Format("MM-dd") }); 
                    console.log({ date: tempdate.Format("yyyy-MM-dd"), "tempdate": tempdate.Format("MM-dd") })

                } 
                if (result.data[i].value_q == "true") { 
                    datanormal[i] = result.data[i].VALUE;
                } else {
                    dataerror[i] = result.data[i].VALUE;
                }

            }
        } else {
            for (var i = 0; i < result.data2.length; i++) {
                var tempdate = new Date(result.data[i].SDATE);
                if (datearr.length < 7) {
                    datearr.push(tempdate.Format("MM-dd"));
                    sevendate.push({ date: tempdate.Format("yyyy-MM-dd"), "tempdate": tempdate.Format("MM-dd") });
                } 
                if (result.data2[i].value_q == "true" || result.data[i].value_q == "true") {
                    datanormal[i] = result.data2[i].VALUE;
                } else {
                    dataerror[i] = result.data2[i].VALUE;
                }
                data3[i] = (result.data[i].VALUE != null && result.data[i].VALUE>0 ? parseFloat(result.data[i].VALUE).toFixed(2) : result.data[i].VALUE);
            }
        } 
        barColorJsonNew.xAxis[0].data = datearr;
         
        if (datanormal.max() != null && datanormal.max() < 0 && datanormal.max() != -100) { 
            barColorJsonNew.yAxis[0].max = (parseInt(datanormal.max()));
        }
        if (dataerror.max() != null && dataerror.max() < 0 && dataerror.max() != -100 && dataerror.max() > datanormal.max()) {
            barColorJsonNew.yAxis[0].max = (parseInt(dataerror.max()));
        }
        if (datanormal.max() != null && datanormal.max().length > 3) {
            barColorJsonNew.grid.left = barColorJsonNew.grid.left + (datanormal.max().length - 3) * 3
        }
        var normax = datanormal.max() ? datanormal.max() : 0;
        var normin = datanormal.min() ? datanormal.min() : 0;
        var errmax = dataerror.max() ? dataerror.max() : 0;
        var errmin = dataerror.min() ? dataerror.min() : 0;
        if (normax != -100 && normax == 0 && normin!=-100 && normin < 0) {
            barColorJsonNew.yAxis[0].inverse = true;
        }
        if (errmax != -100 && errmax == 0 && errmin!=-100 && errmin < 0) {
            barColorJsonNew.yAxis[0].inverse = true;
        }
        
        barColorJsonNew.series[0].data = datanormal;
        barColorJsonNew.series[1].data = dataerror;
        console.log("showarrdiv:::::", showarrdiv)
        if (showarrdiv.chartindex == 1) {
            barColorJsonNew.series[2].data = data3;
            console.log("barColorJsonNew.series[2].data :::", barColorJsonNew.series[2].data);
            barColorJsonNew.grid.left = 30;
            barColorJsonNew.yAxis[0].name = showarrdiv.yname + "(个)";
        } else {
            barColorJsonNew.yAxis[0].name = showarrdiv.yname + "("+showarrdiv.unit+")";
        }
        console.log(barColorJsonNew);
        var myChart = echarts.init(document.getElementById(result.index + "div"));
        var message = "信息未空！！！！";
        if (result.message != undefined && result.message != "") {
            message = result.message
        }
        $("#" + result.index + "div").parent().find("div").find("a").html("定位信息:" + message);
        myChart.setOption(barColorJsonNew);
        myChart.on('click', function (params) { 
            if (showarrdiv.clickson == 1) {
                $(".dayclasss").hide(); 
                showcharts(showarrdiv, params);
            }
        });
    }).error(function (status) {
        console.log("statuts::::", status);
    });;
}
var linejson = null;
var showcharts = function (showarrdiv, param) {
    var pid = $("#" + showarrdiv.key + "div").parent().parent().attr("id");
    console.log("pid::", pid);
    if ($("#datediv" + pid).is(":hidden")) {
        $("#datediv" + pid).show();
    }
    var selectparamnew = deepClone(selectparam);
    //插入画日期图像  
    if (linejson == null) {
        $.ajax({ url: "frame/echarts/chart/line.json?d=" + new Date(), async: false }).success(function (geoJson) {
            linejson = geoJson
        })
    }
    console.log("sevendate:::", sevendate);
    for (var i = 0; i < sevendate.length; i++) {
   
        if (sevendate[i].tempdate == param.name) {
        
            selectparamnew.datePicker = sevendate[i].date;
            break;
        }
    } 
    selectparamnew.scale = "h"; 
    if (param.seriesType == "bar") { 
        selectparamnew.key = showarrdiv.cs == undefined ? showarrdiv.key : showarrdiv.cs; 
    } else { 
        selectparamnew.key = showarrdiv.key;
    } 
    $.get(dataactionurl, selectparamnew, function (result) {
        var dd = [];
        if (result != "") {
            result = eval("(" + result + ")");
            for (var i = 0; i < result.data.length; i++) {

                if (param.seriesType != "bar") {
                    dd.push(result.data[i].VALUE != null && result.data[i].VALUE > 0 ? parseFloat(result.data[i].VALUE).toFixed(2) : result.data[i].VALUE)
                } else {
                    dd.push(result.data[i].VALUE);
                } 
            } 
            linejson.series[0].data = dd; 
        }
        var myChart = echarts.init($("#datediv" + pid).find(".datechartshowclass")[0]);
   
        linejson.title.text = showarrdiv.value + (showarrdiv.cs != undefined?"(次数)": "" ) + "_" + param.name + " ";
        myChart.setOption(linejson);
    })
    

}

var showtableview = function (id) { 
    var selectparamnew = deepClone(selectparam);
    selectparamnew.key = id;
    $('#' + id).bootstrapTable({
        url: '../services/AlarmServices.ashx',
        striped: true,
        showExport: true,  //是否显示导出按钮  
        buttonsAlign: "right",  //按钮位置  
        exportTypes: ['excel'],  //导出文件类型  
        Icons: 'glyphicon-export',
        exportDataType: 'all',
        search: false,
        sortName: "id",    //排序相关  
        sortOrder: "desc",
        sortStable: true,
        queryParams: selectparamnew,
        export: 'glyphicon-export icon-share',
        sortName: ['id'],
        columns: [
         {
             field: 'CITY',
             title: '地市'
         }, {
             field: 'MANUFACTOR',
             title: '厂家'
         }, {
             field: 'CELLNAME',
             title: '小区名称',
             sortOrder: true,
             sortable: true
         }, {
             field: 'ECI',
             title: 'ECI',
             sortOrder: true,
             sortable: true
         }, {
             field: 'SC_ENBID',
             title: 'eNBID',
             sortable: true,
             formatter: function (value, row, index) {

                 //此处对value值做判断，不然value为空就会报错
                 value = value ? value : '';
                 var length = value.length;
                 if (length && length > 6) {
                     length = 6;
                     return "<span title ='" + value + "'>" + value.substring(0, length) + "...</span>"
                 }
                 return value;
             }
      }, 
            {
                field: 'ALARMCODE',
                title: '告警码'
            }, {
                field: 'ALARMCONTEXT',
                title: '告警内容'
            } , {
                field: 'CREATETIME',
                title: '发生时间'
            }, {
                field: 'CLEARTIME',
                title: '清除时间'
            }, {
                field: 'CLEARSTATIC',
                title: '是否清除'
            }] 
            });
}

function dwtxxemplatehtml() {
    $("#showview").empty();

    $("#showview").append($("#dwxxtemplate").html());
    $("#KQIINFO").html(rowOne.KQIINFO);
    $("#KQIINDEX").html(rowOne.KQIINDEX);
    $("#MEASURES").html( rowOne.MEASURES);
    $("#REASON").html(rowOne.REASON);

    $("#SFGTX").html(rowOne.SFGTX);
    $("#ANTENNAH").html(rowOne.ANTENNAH);
    $("#ANTENNAAZIMUTH").html(rowOne.ANTENNAAZIMUTH);
    $("#DIPANGLE").html(rowOne.DIPANGLE);
    $("#EDIPANGLE").html(rowOne.EDIPANGLE);
    $("#MPDIPANGLE").html(rowOne.MPDIPANGLE);
    $("#MRCLASS").html(rowOne.MRCLASS);
    $("#HOTSPOTCLASS").html(rowOne.HOTSPOTCLASS);
    $("#HOTSPOTNAME").html(rowOne.HOTSPOTNAME);
    // dwxxhtmltable("dwxxtable"); 
    // var t1 = window.setInterval(showradarChart, 1000);
    showCase();
    showradarChart();
}

function showCase() {
    $("#eciCase").html("");
    $.get("../services/CaseUpFile.ashx", selectparam, function (data) {
        console.log("result:::", data);
        var jdata = eval('(' + data + ')');
        var caseHtml = '';
        if (jdata.result) {
            $.each(jdata.data, function () {
                caseHtml += "<li><a href='"+this.url+"' target='_blank'>"+this.name+"</a></li>";
            });
            $("#eciCase").html(caseHtml);
        }
    });
}

var dwxxhtmltable = function (id) {
    $('#' + id).bootstrapTable({
        url: 'data2.json',
        striped: true,
        showExport: true,  //是否显示导出按钮  
        buttonsAlign: "right",  //按钮位置  
        exportTypes: ['excel'],  //导出文件类型  
        Icons: 'glyphicon-export',
        exportDataType: 'all',
        search: false,
        sortName: "id",    //排序相关  
        sortOrder: "desc",
        sortStable: true,
        export: 'glyphicon-export icon-share',
        sortName: ['id'],
        columns: [
          {
              field: 'id',
              title: '小区名称',
              sortOrder: true,
              sortable: true
          }, {
              field: 'id',
              title: 'ECI',
              sortOrder: true,
              sortable: true
          }, {
              field: 'KQIINFO',
              title: 'KQI信息'
          }, {
              field: 'KQIINDEX',
              title: 'KQI差指标'
          }, {
              field: 'REASON',
              title: '质差原因'
          }],
        onClickRow: function (row, $element) {
            console.log(row.id);
            $(".danger").removeClass("danger");
            $element.addClass("danger");

            $('#myModal').modal('show').css({
                width: '2200px',
                'margin-left': function () {
                    return -60;
                }
            });
        }
    });
}
var radarjson = null;
var radadatajson = null;
var strarr = [];
var showradarChart = function (id) {
    if (id == undefined) id = "dwxxmap" 
    var ddhig = $("#dwxxmap").parent().width();
    $("#" + id).width(ddhig);
    console.log("获得div::::", $("#" + id).attr("width"));
     
    var myChart = echarts.init(document.getElementById(id));
    myChart.showLoading();
    if (radarjson == null) {
        $.ajax({ url: "frame/echarts/chart/radar.json", async: false }).success(function (geoJson) { 

            geoJson.series[0].areaStyle = {
                normal: {
                    color: new echarts.graphic.RadialGradient(0.5, 0.5, 0.5, [{
                        offset: 0,
                        color: '#ccffff'
                    },
                        {
                            offset: 1,
                            color: '#ccffcc'
                        }
                    ], false)
                }
            }
            geoJson.radar.axisLabel = {
                show: true,
                textStyle: {
                    color: 'red'
                },
                formatter: function (value, index) {
                    if (!strarr[index] == value || strarr[index] == undefined) {
                        strarr.push(value);
                        str = value
                    } else {
                        str = " "
                    }
                    return str;
                }
            }
            radarjson = geoJson;
        });
    }
    console.log("radarjson:::", radarjson);

    
   $.get("../services/RadarChartServices.ashx", selectparam, function (data) {
            console.log("result:::", data);
            if (data != null) {
                var dataarr = data.split(",");
                console.log("dataarr::", dataarr);
                radarjson.series[0].data[0].value = dataarr; 
            }
            myChart.setOption(radarjson);
            myChart.hideLoading();

   });
 
}