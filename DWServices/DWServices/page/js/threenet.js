/**
 * 初始化图表
 */
var e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14;
var o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14;
var citydata,appdata;

Date.prototype.Format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份   
        "d+": this.getDate(), //日   
        "h+": this.getHours(), //小时   
        "m+": this.getMinutes(), //分   
        "s+": this.getSeconds(), //秒   
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
        "S": this.getMilliseconds() //毫秒   
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

function initChart() {
    e1 = echarts.init(document.getElementById("e1"));
    e2 = echarts.init(document.getElementById("e2"));
    e3 = echarts.init(document.getElementById("e3"));
    e4 = echarts.init(document.getElementById("e4"));
    e5 = echarts.init(document.getElementById("e5"));
    e6 = echarts.init(document.getElementById("e6"));
    e7 = echarts.init(document.getElementById("e7"));
    e8 = echarts.init(document.getElementById("e8"));
    e9 = echarts.init(document.getElementById("e9"));
    e10 = echarts.init(document.getElementById("e10"));
    e11 = echarts.init(document.getElementById("e11"));
    e12 = echarts.init(document.getElementById("e12"));
    e13 = echarts.init(document.getElementById("e13"));
    e14 = echarts.init(document.getElementById("e14"));

    o1 = {
        title: {
            text: "标题",
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        legend: {
            data: ["电信", "联通", "移动"],
        },
        xAxis: {
            data: []
        },
        yAxis: {
            type: 'value',
        },
        series: [
            {
                name: "电信",
                type: "bar",
                data: [],
                label: {
                    normal: {
                        show: true,
                        position: "top"
                    },
                    emphasis: {
                        show: true,
                        position: "top",
                    }
                }
            },
            {
                name: "联通",
                type: "bar",
                data: [],
                label: {
                    normal: {
                        show: true,
                        position: "top"
                    },
                    emphasis: {
                        show: true,
                        position: "top",
                    }
                }
            },
            {
                name: "移动",
                type: "bar",
                data: [],
                label: {
                    normal: {
                        show: true,
                        position: "top"
                    },
                    emphasis: {
                        show: true,
                        position: "top",
                    }
                }
            }
        ]
    }

    o2 = o1, o3 = o1, o4 = o1, o5 = o1, o6 = o1, o7 = o1, o8 = o1, o9 = o1, o10 = o1, o11 = o1, o12 = o1, o13 = o1, o14 = o1;

    e1.setOption(o1);
    e2.setOption(o2);
    e3.setOption(o3);
    e4.setOption(o4);
    e5.setOption(o5);
    e6.setOption(o6);
    e7.setOption(o7);
    e8.setOption(o8);
    e9.setOption(o9);
    e10.setOption(o10);
    e11.setOption(o11);
    e12.setOption(o12);
    e13.setOption(o13);
    e14.setOption(o14);

    o1 = {
        title: {
            text:"页面打开时延",
        }
    }
    o2 = {
        title: {
            text: "首包时延",
        }
    }
    o3 = {
        title: {
            text: "首屏时延",
        }
    }
    o4 = {
        title: {
            text: "视频平均速率",
        }
    }
    o5 = {
        title: {
            text: "视频卡顿频率",
        }
    }
    o6 = {
        title: {
            text: "即时通讯成功率",
        }
    }
    o7 = {
        title: {
            text: "游戏时延",
        }
    }
    o8 = o1, o9 = o2, o10 = o3, o11 = o4, o12 = o5, o13 = o6, o14 = o7;
    e1.setOption(o1);
    e2.setOption(o2);
    e3.setOption(o3);
    e4.setOption(o4);
    e5.setOption(o5);
    e6.setOption(o6);
    e7.setOption(o7);
    e8.setOption(o8);
    e9.setOption(o9);
    e10.setOption(o10);
    e11.setOption(o11);
    e12.setOption(o12);
    e13.setOption(o13);
    e14.setOption(o14);
}

/**
 * 默认加载数据
 */
function initData () {
    var date1 = $("#date1").val();
    var date2 = $("#date2").val();
    query(date1,date2);
}

///**
// * 点击查询按钮
// */
function search() {
    $("#search").click(function () {
        var date1 = $("#date1").val();
        var date2 = $("#date2").val();
        query(date1, date2);
    });
}

/**
 * 查询请求
 */
function query(date1,date2) {
    $.ajax({
        url: "../services/ThreeNet.ashx?type=query",
        type:"post",
        data: {"date1":date1,"date2":date2},
        dataType: "json",
        success: function (result) {
            if (result.ok) {
                citydata = makeCityData(result.city);
                //console.log(citydata);
                loadCityData(citydata, "页面打开时延", "pageOpenDelay", e1, o1);
                loadCityData(citydata, "首包时延", "firstByteDelay", e2, o2);
                loadCityData(citydata, "首屏时延", "firstScreenDelay", e3, o3);
                loadCityData(citydata, "视频平均速率", "videoAvgSpeed", e4, o4);
                loadCityData(citydata, "视频卡顿频率", "cacheRate", e5, o5);
                loadCityData(citydata, "即时通讯成功率", "imSendRate", e6, o6);
                loadCityData(citydata, "游戏时延", "ackDelay", e7, o7);
                appdata = makeAppData(result.app);
                //console.log(result.app);
                console.log(appdata["imSendRate"]["im"]);
                loadAppData(appdata, "页面打开时延", "pageOpenDelay","web", e8, o8);
                loadAppData(appdata, "首包时延", "firstByteDelay","web", e9, o9);
                loadAppData(appdata, "首屏时延", "firstScreenDelay","web", e10, o10);
                loadAppData(appdata, "视频平均速率", "videoAvgSpeed","video", e11, o11);
                loadAppData(appdata, "视频卡顿频率", "cacheRate","video", e12, o12);
                loadAppData(appdata, "即时通讯成功率", "imSendRate","im", e13, o13);
                loadAppData(appdata, "游戏时延", "ackDelay","game", e14, o14);
            }
        },
        error: function () {
            console.log("数据请求失败");
        }
    });
}

/**
 * 第一张图表加载数据
 * @param {any} data
 * @param {any} title
 * @param {any} type
 * @param {any} chart
 * @param {any} option
 */
function loadCityData(data, title, type, chart, option) {
    option = {
        title: {
            text: title,
        },
        xAxis: {
            data: data["cities"]
        },
        series: [
            {
                name: "电信",
                type: "bar",
                data: data[type]["ctcc"],
            },
            {
                name: "联通",
                type: "bar",
                data: data[type]["cucc"],
            },
            {
                name: "移动",
                type: "bar",
                data: data[type]["cmcc"],
            }
        ]
    }
    chart.setOption(option);
}

/**
 * 第一张图表数据处理
 */
function makeCityData(data) {
    //console.log(data);
    var cities = [];
    var pageOpenDelay = {};
    var firstByteDelay = {};
    var firstScreenDelay = {};
    var videoAvgSpeed = {};
    var cacheRate = {};
    var imSendRate = {};
    var ackDelay = {};
    for (var i = 0; i < data.length; i++) {
        var cityName = data[i].CITY;
        var operator = data[i].OPERATOR;
        if ($.inArray(cityName, cities) == -1) {
            cities.push(cityName);
        }
        pushCityData(pageOpenDelay, operator, data[i], "PAGEOPENDELAY");
        pushCityData(firstByteDelay, operator, data[i], "FIRSTBYTEDELAY");
        pushCityData(firstScreenDelay, operator, data[i], "FIRSTSCREENDELAY");
        pushCityData(videoAvgSpeed, operator, data[i], "VIDEOAVGSPEED");
        pushCityData(cacheRate, operator, data[i], "CACHERATE");
        pushCityData(imSendRate, operator, data[i], "IMSENDRATE");
        pushCityData(ackDelay, operator, data[i], "ACKDELAY");
    }
    return {
        "cities": cities, "pageOpenDelay": pageOpenDelay,
        "firstByteDelay": firstByteDelay, "videoAvgSpeed":videoAvgSpeed,
        "cacheRate": cacheRate, "imSendRate": imSendRate,
        "ackDelay": ackDelay, "firstScreenDelay": firstScreenDelay
        };
}

function pushCityData(m, operator, data, type) {
    if (!m[operator]) {
        m[operator] = [];
    }
    m[operator].push(data[type]);
}

function makeAppData(data) {
    var webx = [];
    var videox = [];
    var imx = [];
    var gamex = [];
    var pageOpenDelay = {};
    var firstByteDelay = {};
    var firstScreenDelay = {};
    var videoAvgSpeed = {};
    var cacheRate = {};
    var imSendRate = {};
    var ackDelay = {};
    for (var i = 0; i < data.length; i++) {
        var appName = data[i].PCONTENT;
        switch(data[i].PTYPE) {
            case "web":
                if ($.inArray(appName,webx) == -1) {
                    webx.push(appName);
                }
                break;
            case "im":
                if ($.inArray(appName, imx) == -1) {
                    imx.push(appName);
                }
                break;
            case "video":
                if ($.inArray(appName, videox) == -1) {
                    videox.push(appName);
                }
                break;
            case "game":
                if ($.inArray(appName, gamex) == -1) {
                    gamex.push(appName);
                }
        }
        //if (data[i].PCONTENT == "QQ") {
        //    console.log(data[i]);
        //}
    }
    for (var i = 0; i < data.length; i++) {
        var operator = data[i].OPERATOR;
        pushAppData(pageOpenDelay, operator, data[i], "PAGEOPENDELAY", webx);
        pushAppData(firstByteDelay, operator, data[i], "FIRSTBYTEDELAY", webx);
        pushAppData(firstScreenDelay, operator, data[i], "FIRSTSCREENDELAY", webx);
        pushAppData(videoAvgSpeed, operator, data[i], "VIDEOAVGSPEED", videox);
        pushAppData(cacheRate, operator, data[i], "CACHERATE", videox);
        pushAppData(imSendRate, operator, data[i], "IMSENDRATE",imx);
        pushAppData(ackDelay, operator, data[i], "ACKDELAY",gamex);
    }
    console.log(data);
    console.log(imSendRate);
    return {
        "webx": webx, "gamex": gamex, "imx": imx, "videox": videox,
        "pageOpenDelay": pageOpenDelay,
        "firstByteDelay": firstByteDelay, "videoAvgSpeed": videoAvgSpeed,
        "cacheRate": cacheRate, "imSendRate": imSendRate,
        "ackDelay": ackDelay, "firstScreenDelay": firstScreenDelay
    };
}

function loadAppData(data, title, type, ptype, chart, option) {
    option = {
        title: {
            text: title,
        },
        xAxis: {
            data: data[ptype+"x"]
        },
        series: [
            {
                name: "电信",
                type: "bar",
                data: data[type][ptype]["ctcc"],
            },
            {
                name: "联通",
                type: "bar",
                data: data[type][ptype]["cucc"],
            },
            {
                name: "移动",
                type: "bar",
                data: data[type][ptype]["cmcc"],
            }
        ]
    }
    chart.setOption(option);
}

function pushAppData(m, operator, data, type, x) {
    if (!m.init) {
        m.init = true;
        m["web"] = {};
        m["game"] = {};
        m["im"] = {};
        m["video"] = {};
    }
    var ptype = data.PTYPE;
    var name = data.PCONTENT;
    var index = getIndex(x, name);
    if (ptype == "web") {
        if (!m["web"][operator]) {
            m["web"][operator] = new Array(x.length);
        }
        m["web"][operator][index] = data[type];
    }
    if (ptype == "game") {
        if (!m["game"][operator]) {
            m["game"][operator] =new Array(x.length);
        }
        m["game"][operator][index] = data[type];
    }
    if (ptype == "im") {
        if (!m["im"][operator]) {
            m["im"][operator] = new Array(x.length);
        }
        m["im"][operator][index] = data[type];
    }
    if (ptype == "video") {
        if (!m["video"][operator]) {
            m["video"][operator] = new Array(x.length);
        }
        m["video"][operator][index] = data[type];
    }
}

function getIndex(arr, n) {
    for (var i = 0; i < arr.length;i++) {
        if (arr[i] == n) {
            return i;
        }
    }
}
/**
 * 获取服务器时间
 */
function getServerDateTime() {
    $.ajax({
        url: "../services/Citygoodrate.ashx?type=datetime",
        type: "get",
        dataType: "json",
        async: false,
        success: function (result) {
            if (result) {
                var res = new Date(result.year);
                res.setDate(res.getDate() - 1);
                result.year = res.Format("yyyy-MM-dd");
                $("#date1").val(result.year);
                $("#date2").val(result.year);
            }
        },
        error: function () {
            console.log("服务器时间获取失败");
        }
    });
}